#!/usr/bin/env python3
import time
import struct
from pyharp.device import Device
from pyharp.messages import HarpMessage, WriteHarpMessage, PayloadType
from app_registers_refactor import DelphiOnlyAppRegs
from typing import Iterable, Tuple

import logging

logger = logging.getLogger()
logger.addHandler(logging.StreamHandler())

"""Helper functions"""


# functions
def print_poke_counts(
    device,
):
    reply = device.send(HarpMessage.ReadU8(DelphiOnlyAppRegs.PokeDometer).frame)
    print(f"Current pokedometer count is: {reply.payload}.")
    return None


# # Open serial connection of Delphi controller
# com_port = "COM20"
com_port = "COM119"
device = Device(com_port)
device.info()  # Display device's info on screen


# Read from U8 Array
def read_float_struct_from_u8(
    u8_array: Iterable[int | bytes | bytearray],
    bytes_expected: int = 32,
) -> Tuple[float, float, float, float]:
    """
    Decode a U8 array that represents a packed struct of four 32-bit floats.
    Assumes little-endian encoding (<4f).

    Accepts: list[int], bytes, or bytearray.
    Returns: (f0, f1, f2, f3)

    Raises:
        ValueError if the input length is not exactly `bytes_expected` bytes.
    """
    # Normalize to bytes
    if isinstance(u8_array, (bytes, bytearray)):
        buf = bytes(u8_array)
    else:
        buf = bytes(bytearray(u8_array))

    if len(buf) != bytes_expected:
        raise ValueError(
            f"Expected {bytes_expected} bytes for 8 floats, got {len(buf)}"
        )

    return struct.unpack("<8f", buf)


def read_uint16_struct_from_u8(
    u8_array: Iterable[int | bytes | bytearray],
    bytes_expected: int = 16,
) -> Tuple[int, int, int, int]:
    """
    Decode a U8 array that represents a packed struct of four 16-bit unsigned integers.
    Assumes little-endian encoding (<4H).

    Accepts: list[int], bytes, or bytearray.
    Returns: (f0, f1, f2, f3)

    Raises:
        ValueError if the input length is not exactly `bytes_expected` bytes.
    """
    # Normalize to bytes
    if isinstance(u8_array, (bytes, bytearray)):
        buf = bytes(u8_array)
    else:
        buf = bytes(bytearray(u8_array))

    if len(buf) != bytes_expected:
        raise ValueError(
            f"Expected {bytes_expected} bytes for 8 uint16, got {len(buf)}"
        )

    return struct.unpack("<8H", buf)


# Write PID gain values
def pack_pid_config(kp: float, ki: float, kd: float):
    """
    Convert (float, float, float) into a 12-byte U8 array (list of ints).
    Layout matches C struct: <float, float, float>.
    """
    payload = struct.pack("<fff", kp, ki, kd)
    return payload  # convert bytes → list of uint8


"""Set Registers"""
# Poke pin parameters
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePin, 22).frame)
reply = device.send(
    HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePinInverted, 1).frame
)  # Set starting odor -- Odor index 1 (valve 3)
reply = device.send(
    HarpMessage.WriteU32(DelphiOnlyAppRegs.MinimumPokeTimeUS, 50000).frame
)
# Valve parameters
reply = device.send(
    HarpMessage.WriteU16(DelphiOnlyAppRegs.QueuedOdorMask, 0x0001).frame
)
reply = device.send(
    HarpMessage.WriteU32(DelphiOnlyAppRegs.OdorDwellTimeUS, 500000).frame
)
reply = device.send(
    HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 1).frame
)  # Enable odor state machine
reply = device.send(
    HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableValveLeds, 1).frame
)  # Enable valve LEDs


# Camera parameters
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableCam0Trigger, 0).frame)
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableCam1Trigger, 0).frame)
reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.Cam0FrameRate, 100).frame)
reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.Cam1FrameRate, 30).frame)


# Flow meter parameters
reply = device.send(
    HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableAdcSampling, 1).frame
)  # Enable Flow Rate Sampling

# Assign Flow meters to ADC channels (0-3 -> pins 26-29)
reply = device.send(
    HarpMessage.WriteS8(DelphiOnlyAppRegs.ProportionalValve0Adc, 0).frame
)
reply = device.send(
    HarpMessage.WriteS8(DelphiOnlyAppRegs.ProportionalValve1Adc, 1).frame
)
reply = device.send(
    HarpMessage.WriteS8(DelphiOnlyAppRegs.ProportionalValve2Adc, 2).frame
)


reply = device.send(
    HarpMessage.WriteS8(DelphiOnlyAppRegs.ManualFlowMeter, 3).frame
)  # By default valve is OFF and is ON when assigning an index
reply = device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.LeakAdcChannel, 4).frame)

reply = device.send(
    HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve0EnablePid, 1).frame
)
reply = device.send(
    HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve1EnablePid, 1).frame
)
reply = device.send(
    HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve2EnablePid, 1).frame
)


# PID parameters
pid_gain_array = pack_pid_config(2, 0.75, 0.18)  # validated PID gains
# pid_gain_bytes = struct.pack("<3f", 2.0, 0.75, 0.18)

# print("Energize valve 0 and set hit and hold duty cycle")
msg = WriteHarpMessage(
    PayloadType.U8,
    pid_gain_array,
    DelphiOnlyAppRegs.PidGains,
    offset=len(pid_gain_array) - 1,
)
device.send(msg.frame)

reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.PidUpdateFrequency, 200.0).frame
)
reply = device.send(
    HarpMessage.WriteFloat(
        DelphiOnlyAppRegs.ProportionalValve0TargetFlowRate, 75.0
    ).frame
)
reply = device.send(
    HarpMessage.WriteFloat(
        DelphiOnlyAppRegs.ProportionalValve1TargetFlowRate, 75.0
    ).frame
)
reply = device.send(
    HarpMessage.WriteFloat(
        DelphiOnlyAppRegs.ProportionalValve2TargetFlowRate, 75.0
    ).frame
)


# Leak detection parameters
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.LeakThreshold, 60.0).frame
)  # ~75 mL/min flow rate


# Manual flow meter parameters
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.NominalFlowRate, 75.0).frame
)
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.FlowRateTolerance, 5.0).frame
)

"""Run system"""
# Odor initialization
odor_masks = [0x0001, 0x0002]  # Odor valve sequence
odor_i = -1

# Visualization initialization
last_print = 0.0

# Force/simulated poke parameters
enable_simulated_poke = False  # [bool]
simulated_poke_period = 10  # seconds
valve_on_duration = 10000  # int(4 * 1e6) # microseconds
reply = device.send(
    HarpMessage.WriteU32(
        DelphiOnlyAppRegs.MinOdorDeliveryTimeUS, valve_on_duration
    ).frame
)
last_poke_t = 0

try:
    while True:
        for msg in device.get_events():
            """EVENT BASED ODOR UPDATING"""
            event_address = msg.address
            if event_address == 66:
                event_payload = msg.payload[0]
                if event_payload == 0:  # need new odor
                    odor_i += 1
                    if odor_i > len(odor_masks) - 1:
                        odor_i = 0
                    print(f"New odor index: {odor_masks[odor_i]}")
                    reply = device.send(
                        HarpMessage.WriteU16(
                            DelphiOnlyAppRegs.QueuedOdorMask, odor_masks[odor_i]
                        ).frame
                    )

            """LEAK STATE EVENT"""
            if event_address == 86:
                event_payload = msg.payload[0]
                print(f"Leak State: {event_payload}")  # Turn on to see leak states

            """MANUAL FLOW METER STATE EVENT"""
            if event_address == 90:
                event_payload = msg.payload[0]
                print(f"Manual Flow Meter State: {event_payload}")

        now = time.monotonic()
        if now - last_print >= 0.5:  # print and change the valve every 1 second
            # Read flow rate
            reply = device.send(
                HarpMessage.ReadFloat(DelphiOnlyAppRegs.LatestFlowRate).frame
            )
            latest_flow_rate = read_float_struct_from_u8(
                reply.payload, bytes_expected=32
            )

            print(f"latest_flow_rate: {latest_flow_rate}")

            reply = device.send(
                HarpMessage.ReadFloat(DelphiOnlyAppRegs.LatestRawAdcSample).frame
            )
            latest_raw = read_uint16_struct_from_u8(reply.payload, bytes_expected=16)

            print(f"latest_raw: {latest_raw}")
            # print(
            #     f"{now:.2f}, Odor flow rate: {latest_flow_rate[0]:.2f} mLpm,\t Exhaust flow rate: {latest_flow_rate[-1]:.2f} mLpm,\t Flow A: {latest_flow_rate[1]:.2f}mLpm,\t Flow B: {latest_flow_rate[2]:.2f}mLpm"
            # )
            last_print = now

        # Simulate pokes at a given duration
        if enable_simulated_poke:
            now = time.monotonic()
            if now - last_poke_t > simulated_poke_period:
                # Force Poke
                print("Forcing poke event.")
                reply = device.send(
                    HarpMessage.WriteU8(DelphiOnlyAppRegs.ForceFSM, 1).frame
                )
                last_poke_t = now

except KeyboardInterrupt:
    print("Disabling FSM.")
    reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 0).frame)
    reply = device.send(
        HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableAdcSampling, 0).frame
    )
    reply = device.send(
        HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableCam0Trigger, 0).frame
    )
    reply = device.send(
        HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableCam1Trigger, 0).frame
    )
    reply = device.send(
        HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve0EnablePid, 0).frame
    )
    reply = device.send(
        HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve1EnablePid, 0).frame
    )
    reply = device.send(
        HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve2EnablePid, 0).frame
    )
    # Give some time for the device to process the commands before disconnecting
    reply = device.send(
        HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableAdcSampling, 0).frame
    )

    device.disconnect()
