"""
DeviceManager — owns the Device connection and background polling thread.

Background thread polls registers at POLL_RATE_HZ and puts results into
self.queue as dicts.  App._drain_queue() consumes these on the GUI thread.
"""

import queue
import struct
import threading
import time
import logging

from pyharp.device import Device
from pyharp.messages import HarpMessage
from app_registers_refactor import AppRegs, DelphiOnlyAppRegs

logger = logging.getLogger(__name__)

POLL_RATE_HZ = 5


def _decode_flow_rates(payload) -> list[float]:
    """Decode a U8 payload that packs 4 little-endian 32-bit floats."""
    buf = bytes(bytearray(payload)) if not isinstance(payload, (bytes, bytearray)) else bytes(payload)
    n_floats = len(buf) // 4
    if n_floats < 4:
        raise ValueError(f"Expected at least 16 bytes for 4 floats, got {len(buf)}")
    return list(struct.unpack(f"<{n_floats}f", buf)[:4])


class DeviceManager:
    def __init__(self):
        self._device: Device | None = None
        self._poll_thread: threading.Thread | None = None
        self._stop_event = threading.Event()
        self.queue: queue.Queue[dict] = queue.Queue()
        self._connected = False

    # ── Connection lifecycle ───────────────────────────────────────────────────

    @property
    def is_connected(self) -> bool:
        return self._connected

    def connect(self, port: str) -> None:
        if self._connected:
            return
        self._device = Device(port)
        self._connected = True
        self._stop_event.clear()
        self._poll_thread = threading.Thread(
            target=self._poll_loop, daemon=True, name="DeviceManager-poll"
        )
        self._poll_thread.start()
        logger.info("Connected to %s", port)

    def disconnect(self) -> None:
        if not self._connected:
            return
        self._stop_event.set()
        if self._poll_thread is not None:
            self._poll_thread.join(timeout=2.0)
            self._poll_thread = None
        try:
            self._device.disconnect()
        except Exception as exc:
            logger.warning("Error during device disconnect: %s", exc)
        self._device = None
        self._connected = False
        logger.info("Disconnected")

    # ── One-off write/read ─────────────────────────────────────────────────────

    def send(self, harp_message):
        """Send a HarpMessage and return the reply.  Raises if not connected."""
        if not self._connected or self._device is None:
            raise RuntimeError("DeviceManager is not connected")
        return self._device.send(harp_message.frame)

    # ── Background polling ─────────────────────────────────────────────────────

    def _poll_loop(self) -> None:
        interval = 1.0 / POLL_RATE_HZ
        while not self._stop_event.is_set():
            t0 = time.monotonic()
            try:
                result = self._poll_once()
                if result is not None:
                    self.queue.put_nowait(result)
            except Exception as exc:
                logger.warning("Poll error: %s", exc)
            elapsed = time.monotonic() - t0
            remaining = interval - elapsed
            if remaining > 0:
                self._stop_event.wait(remaining)

    def _poll_once(self) -> dict | None:
        device = self._device
        if device is None:
            return None

        # Flow rates — 4 floats packed as U8 payload
        reply = device.send(HarpMessage.ReadFloat(DelphiOnlyAppRegs.LatestFlowRate).frame)
        flow_rates = _decode_flow_rates(reply.payload)

        # Raw ADC samples — U16 array (8 channels)
        reply = device.send(HarpMessage.ReadU16(DelphiOnlyAppRegs.LatestRawAdcSample).frame)
        adc_samples = list(reply.payload) if reply.payload is not None else []

        # Leak state — U8
        reply = device.send(HarpMessage.ReadU8(DelphiOnlyAppRegs.LeakState).frame)
        leak_state = int(reply.payload[0]) if reply.payload else 0

        # Valves state — U16 bitmask
        reply = device.send(HarpMessage.ReadU16(AppRegs.ValvesState).frame)
        valves_state = int(reply.payload[0]) if reply.payload else 0

        # Proportional valve duty cycles (valves 0–2)
        duty_cycles = []
        for reg in (
            DelphiOnlyAppRegs.ProportionalValve0DutyCycle,
            DelphiOnlyAppRegs.ProportionalValve1DutyCycle,
            DelphiOnlyAppRegs.ProportionalValve2DutyCycle,
        ):
            reply = device.send(HarpMessage.ReadFloat(reg).frame)
            duty_cycles.append(float(reply.payload[0]) if reply.payload else 0.0)

        return {
            "flow_rates": flow_rates,
            "adc_samples": adc_samples,
            "leak_state": leak_state,
            "valves_state": valves_state,
            "duty_cycles": duty_cycles,
            "timestamp": time.time(),
        }
