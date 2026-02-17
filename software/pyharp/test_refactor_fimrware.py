#!/usr/bin/env python3
import time
import struct
from pyharp.device import Device
from pyharp.messages import HarpMessage
from app_registers_refactor import DelphiOnlyAppRegs
from typing import Iterable, Tuple

import logging

logger = logging.getLogger()
logger.addHandler(logging.StreamHandler())


# functions
def print_poke_counts(
    device,
):
    reply = device.send(HarpMessage.ReadU8(DelphiOnlyAppRegs.PokeDometer).frame)
    print(f"Current pokedometer count is: {reply.payload}.")
    return None


# Open serial connection with the first Valve Controller.
com_port = "COM8"  #'COM3' #None
device = Device(com_port)
device.info()  # Display device's info on screen

print()
print("Enabling aux gpios 25-29 as outputs")
# gpio_dir = 32
# reply = device.send(HarpMessage.WriteU8(AppRegs.AuxGPIODir, gpio_dir).frame)
# print(f"reply: {reply.payload[0]:08b}")
# gpio_set = 0b00100000
# reply = device.send(HarpMessage.WriteU8(AppRegs.AuxGPIOSet, gpio_set).frame)
# print(f"reply: {reply}")
# print(f"reply: {reply.payload[0]:08b}")
# reply = device.send(HarpMessage.WriteU8(AppRegs.AuxGPIOClear, gpio_set).frame)
# print(f"reply: {reply.payload[0]:08b}")


# Read from U8 Array
def read_float4_from_u8(
    u8_array: Iterable[int | bytes | bytearray],
) -> Tuple[float, float, float, float]:
    """
    Decode a U8 array that represents a packed struct of four 32-bit floats.
    Assumes little-endian encoding (<4f).

    Accepts: list[int], bytes, or bytearray.
    Returns: (f0, f1, f2, f3)

    Raises:
        ValueError if the input length is not exactly 16 bytes.
    """
    # Normalize to bytes
    if isinstance(u8_array, (bytes, bytearray)):
        buf = bytes(u8_array)
    else:
        buf = bytes(bytearray(u8_array))

    if len(buf) != 16:
        raise ValueError(f"Expected 16 bytes for 4 floats, got {len(buf)}")

    return struct.unpack("<4f", buf)


print()
print_poke_counts(device)
print("Setting odor.")
reply = device.send(
    HarpMessage.WriteU16(DelphiOnlyAppRegs.QueuedOdorMask, 0x0100).frame
)
print("Assigning poke pin.")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePin, 22).frame)
print("Inverting poke pin.")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePinInverted, 1).frame)
print("Enabling FSM")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 1).frame)
print("Camera 0 FPS")
reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.Cam0FrameRate, 100).frame)
print("Camera 0 Enabled")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableCam0Trigger, 1).frame)
print("Camera 1 FPS")
reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.Cam1FrameRate, 30).frame)
print("Camera 1 Enabled")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableCam1Trigger, 1).frame)
print("Enable Valve LEDS")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableValveLeds, 1).frame)
print("Enable ADC Sampling")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableAdcSampling, 1).frame)
print("ADC Sampling Rate")
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.AdcSamplingRate, 10.0).frame
)
print("Select Leak ADC Channel")
reply = device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.LeakAdcChannel, 1).frame)

print("Select Leak Threshold")
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.LeakThreshold, 60.0).frame
)  # ~75 mL/min flow rate

print("Select Manual Flow Meter")
reply = device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.ManualFlowMeter, 0).frame)

print("Nominal Flow Rate")
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.NominalFlowRate, 75.0).frame
)

print("Flow Rate Tolerance")
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.FlowRateTolerance, 5.0).frame
)

"""Set Timings"""
print("Min Poke Time")
reply = device.send(
    HarpMessage.WriteU32(DelphiOnlyAppRegs.MinimumPokeTimeUS, 10000).frame
)

"""Set Calibrations"""
print("Calibrate Slope")
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.CalibrateSlope, 0.02).frame
)

print("Calibrate Offset")
reply = device.send(
    HarpMessage.WriteFloat(DelphiOnlyAppRegs.CalibrateOffset, 0.5).frame
)

print()
odor_masks = [0x0001, 0x0002, 0x0004, 0x0008]
print(odor_masks)
odor_i = -1
last_print = 0.0
try:
    while True:
        for msg in device.get_events():
            # print(msg)
            # print()
            # print_poke_counts(device)
            # print(f"event address: {msg.address}")
            # print(f"event payload: {msg.payload[0]}")

            """EVENT BASED ODOR UPDATING"""
            event_address = msg.address
            if event_address == 66:
                event_payload = msg.payload[0]
                if event_payload == 0:  # -1 previously
                    odor_i += 1
                    if odor_i > len(odor_masks) - 1:
                        odor_i = 0
                    # print(f"New odor index: {odor_masks[odor_i]}")
                    reply = device.send(
                        HarpMessage.WriteU16(
                            DelphiOnlyAppRegs.QueuedOdorMask, odor_masks[odor_i]
                        ).frame
                    )

            # """VALVE STATE EVENT"""
            # if event_address == 32:
            #     event_payload = msg.payload[0]
            #     print(f"Valves State: {event_payload}")

            # """TRIGGER STATE EVENT"""
            # if event_address == 71:
            #     event_payload = msg.payload[0]
            #     print(f"Trigger State: {event_payload}")

            """LEAK STATE EVENT"""
            if event_address == 85:
                event_payload = msg.payload[0]
                print(f"Leak State: {event_payload}")

            """MANUAL FLOW METER STATE EVENT"""
            if event_address == 89:
                event_payload = msg.payload[0]
                print(f"Manual Flow Meter State: {event_payload}")

        """Read ADC"""
        reply = device.send(
            HarpMessage.ReadFloat(DelphiOnlyAppRegs.LatestFlowRate).frame
        )
        # print(f"Latest ADC Sample: {reply.payload}")

        now = time.monotonic()
        if now - last_print >= 1.0:  # print and change the valve every 1 second
            print(read_float4_from_u8(reply.payload))
            last_print = now

            # # Force Poke
            # print("Forcing poke event.")
            # reply = device.send(
            #     HarpMessage.WriteU8(DelphiOnlyAppRegs.ForceFSM, 1).frame
            # )

        # # Expect 20 bytes: <Iffff  (little-endian: uint32 + 4 floats)
        # EXPECTED = struct.calcsize("<Iffff")  # 20
        # if len(reply.payload) != EXPECTED:
        #     raise ValueError(
        #         f"Unexpected payload size {len(reply.payload)} (expected {EXPECTED})"
        #     )

        # t_us, v0, v1, v2, v3 = struct.unpack("<Iffff", reply.payload)
        # print(f"t_us={t_us}, v=[{v0:.6f}, {v1:.6f}, {v2:.6f}, {v3:.6f}]")


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
    device.disconnect()
