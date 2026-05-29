#!/usr/bin/env python3
# /// script
# requires-python = ">=3.10, <3.14"
# dependencies = [
#   "pyharp",
#   "alicat",
# ]
#
# [tool.uv.sources]
# pyharp = { git = "https://github.com/AllenNeuralDynamics/pyharp.git", rev = "368e346dd66c17516974a339a0cf3f4807c02874" }
# alicat = { path = "../alicat" }
# ///
"""
flowtest.py — logs Alicat + Delphi analog flow meter (and optionally controls
proportional valve 1 via PID) to a timestamped CSV at a configurable rate.

Usage:
    uv run flowtest.py DELPHI_PORT ALICAT_PORT [options]
"""
import argparse
import asyncio
import csv
import os
import struct
import time
from datetime import datetime
from typing import Iterable, Tuple

from pyharp.device import Device
from pyharp.messages import HarpMessage, WriteHarpMessage, PayloadType
from app_registers_refactor import DelphiOnlyAppRegs
from alicat.driver import FlowMeter


def read_uint16_struct_from_u8(
    u8_array: Iterable,
    bytes_expected: int = 16,
) -> Tuple[int, ...]:
    if isinstance(u8_array, (bytes, bytearray)):
        buf = bytes(u8_array)
    else:
        buf = bytes(bytearray(u8_array))
    if len(buf) != bytes_expected:
        raise ValueError(f"Expected {bytes_expected} bytes, got {len(buf)}")
    return struct.unpack("<8H", buf)


def pack_pid_config(kp: float, ki: float, kd: float) -> bytes:
    return struct.pack("<fff", kp, ki, kd)


FLOW_SCALE = 49.944 * 3.3 / 4096
FLOW_OFFSET = 24.864
DISPLAY_INTERVAL = 10.0


def abbrev_col(name: str) -> str:
    _MAP = {
        "harp_timestamp": "ts",
        "Alicat_volumetric_flow_mL_min": "vol",
        "Alicat_mass_flow_mL_min": "mass",
        "Alicat_temperature_C": "t",
        "Alicat_pressure": "p",
        "valve0_duty_pct": "duty",
    }
    if name in _MAP:
        return _MAP[name]
    if name.startswith("Analog_flow_ch") and name.endswith("_mL_min"):
        return name[len("Analog_flow_") : name.index("_mL_min")]
    return name


def parse_args() -> argparse.Namespace:
    p = argparse.ArgumentParser(
        description="Log Alicat + Delphi analog flow meter to CSV."
    )
    p.add_argument("delphi_port", help="Delphi board serial port, e.g. COM4")
    p.add_argument("alicat_port", help="Alicat serial port, e.g. COM5")
    p.add_argument(
        "--adc-channels", type=int, nargs="+", default=[0, 1, 2, 3, 5],
        help="ADC channels to record (default: 0 1 2 3 5)",
    )
    p.add_argument(
        "--rate", type=float, default=10.0,
        help="Recording rate in Hz (default: 10)",
    )
    p.add_argument(
        "--valve", action="store_true",
        help="Enable PID control on proportional valve 0 and log duty cycle",
    )
    p.add_argument(
        "--valve-target", type=float, default=10.0,
        help="Target flow rate for valve 0 PID in mL/min (default: 70)",
    )
    p.add_argument(
        "--alicat-unit", default="A",
        help="Alicat device unit ID letter (default: A)",
    )
    p.add_argument(
        "--alicat-scale", type=float, default=1000.0,
        help="Multiply Alicat mass_flow by this to get mL/min (default: 1000 for SLPM; use 1 for SCCM)",
    )
    return p.parse_args()


async def main() -> None:
    args = parse_args()
    interval = 1.0 / args.rate

    device = Device(args.delphi_port)
    device.info()

    device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableAdcSampling, 1).frame)

    if args.valve:
        device.send(HarpMessage.WriteS8(
            DelphiOnlyAppRegs.ProportionalValve0Adc, args.adc_channels[0]).frame)
        device.send(HarpMessage.WriteU8(
            DelphiOnlyAppRegs.ProportionalValve0EnablePid, 1).frame)
        pid_bytes = pack_pid_config(2.0, 0.75, 0.18)
        device.send(WriteHarpMessage(
            PayloadType.U8, pid_bytes, DelphiOnlyAppRegs.PidGains,
            offset=len(pid_bytes) - 1).frame)
        device.send(HarpMessage.WriteFloat(
            DelphiOnlyAppRegs.PidUpdateFrequency, 200.0).frame)
        device.send(HarpMessage.WriteFloat(
            DelphiOnlyAppRegs.ProportionalValve0TargetFlowRate, args.valve_target).frame)

    os.makedirs("data", exist_ok=True)
    ts_str = datetime.now().strftime("%Y%m%d_%H%M%S")
    csv_path = os.path.join("data", f"flowtest_{ts_str}.csv")

    header = [
        "harp_timestamp",
        "Alicat_volumetric_flow_mL_min",
        "Alicat_mass_flow_mL_min",
        "Alicat_temperature_C",
        "Alicat_pressure",
        *[f"Analog_flow_ch{ch}_mL_min" for ch in args.adc_channels],
    ]
    if args.valve:
        header.append("valve0_duty_pct")

    print(f"Logging to {csv_path} at {args.rate} Hz.")
    if args.valve:
        print(f"Valve 0 PID enabled, target {args.valve_target} mL/min.")
    print("Press Ctrl-C to stop.")

    col_w = 10
    display_cols = [abbrev_col(h) for h in header]
    title_line = "  ".join(f"{c:>{col_w}}" for c in display_cols)
    print(title_line)
    print(" " * len(title_line))
    last_display = time.perf_counter() - DISPLAY_INTERVAL

    with open(csv_path, "w", newline="") as csvfile:
        writer = csv.writer(csvfile)
        writer.writerow(header)

        async with FlowMeter(address=args.alicat_port, unit=args.alicat_unit) as meter:
            try:
                while True:
                    t0 = time.perf_counter()

                    alicat_data = await meter.get()
                    alicat_vol_flow = alicat_data["volumetric_flow"] * args.alicat_scale
                    alicat_mass_flow = alicat_data["mass_flow"] * args.alicat_scale
                    alicat_temp = alicat_data["temperature"]
                    alicat_pressure = alicat_data["pressure"]

                    adc_reply = device.send(
                        HarpMessage.ReadFloat(DelphiOnlyAppRegs.LatestRawAdcSample).frame)
                    raw = read_uint16_struct_from_u8(adc_reply.payload, bytes_expected=16)
                    harp_ts = adc_reply.timestamp

                    row = [
                        harp_ts,
                        alicat_vol_flow,
                        alicat_mass_flow,
                        alicat_temp,
                        alicat_pressure,
                        *[round(raw[ch] * FLOW_SCALE - FLOW_OFFSET, 3) for ch in args.adc_channels],
                    ]

                    if args.valve:
                        dc_reply = device.send(HarpMessage.ReadFloat(
                            DelphiOnlyAppRegs.ProportionalValve0DutyCycle).frame)
                        duty = dc_reply.payload[0]
                        row.append(round(duty, 3))

                    writer.writerow(row)

                    if t0 - last_display >= DISPLAY_INTERVAL:
                        last_display = t0
                        data_line = "  ".join(
                            f"{v:.1f}".rjust(col_w) if isinstance(v, float) else f"{v!s:>{col_w}}"
                            for v in row
                        )
                        print(f"\033[1A\r{data_line}", end="\n", flush=True)

                    elapsed = time.perf_counter() - t0
                    await asyncio.sleep(max(0.0, interval - elapsed))

            except KeyboardInterrupt:
                pass
            finally:
                if args.valve:
                    device.send(HarpMessage.WriteU8(
                        DelphiOnlyAppRegs.ProportionalValve0EnablePid, 0).frame)
                device.send(HarpMessage.WriteU8(
                    DelphiOnlyAppRegs.EnableAdcSampling, 0).frame)
                device.disconnect()
                print(f"\nSaved {csv_path}")


asyncio.run(main())
