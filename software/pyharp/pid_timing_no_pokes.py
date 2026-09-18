#!/usr/bin/env python3
import argparse
import csv
import os
import struct
import time
from datetime import datetime
from typing import Iterable, Tuple

from pyharp.device import Device
from pyharp.messages import HarpMessage, WriteHarpMessage, PayloadType
from app_registers_refactor import DelphiOnlyAppRegs

import logging

logger = logging.getLogger()
logger.addHandler(logging.StreamHandler())


"""Helper functions"""


def read_uint16_struct_from_u8(
    u8_array: Iterable[int | bytes | bytearray],
    bytes_expected: int = 16,
) -> Tuple[int, int, int, int, int, int, int, int]:
    """
    Decode a U8 array that represents a packed struct of eight 16-bit unsigned integers.
    Assumes little-endian encoding (<8H).
    """
    if isinstance(u8_array, (bytes, bytearray)):
        buf = bytes(u8_array)
    else:
        buf = bytes(bytearray(u8_array))

    if len(buf) != bytes_expected:
        raise ValueError(
            f"Expected {bytes_expected} bytes for 8 uint16, got {len(buf)}"
        )

    return struct.unpack("<8H", buf)


def pack_pid_config(kp: float, ki: float, kd: float):
    """
    Convert (float, float, float) into a 12-byte U8 array.
    Layout matches C struct: <float, float, float>.
    """
    return struct.pack("<fff", kp, ki, kd)


"""Parse arguments"""

parser = argparse.ArgumentParser(description="Log raw ADC samples from Delphi controller to CSV.")
parser.add_argument("com_port", help="Serial port, e.g. COM4")
parser.add_argument(
    "--rate",
    type=float,
    default=1020.0,
    help="Poll rate in Hz (default: 1020)",
)
parser.add_argument(
    "--poke-on",
    type=float,
    default=0.5,
    help="Simulated poke on duration in seconds (default: 1)",
)
parser.add_argument(
    "--poke-off",
    type=float,
    default=3,
    help="Simulated poke off duration in seconds (default: 1)",
)
parser.add_argument(
    "--ch7-threshold",
    type=int,
    default=50,
    help="Raw ADC threshold for ch7 signal detection (default: 100)",
)
parser.add_argument(
    "--ch7-timeout",
    type=float,
    default=0.5,
    help="Seconds after odor1 poke-on to stop watching ch7 (default: 0.5)",
)
parser.add_argument(
    "--pre-arm-advance",
    type=float,
    default=2.0,
    help="Seconds before end valve activation to turn on the next odor (default: 2.0)",
)
args = parser.parse_args()

interval = 1.0 / args.rate

"""Connect to device"""

device = Device(args.com_port)
device.info()

"""Set Registers"""

# Poke pin parameters
#device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePin, 22).frame)
#device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePinInverted, 1).frame)
#device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.MinimumPokeTimeUS, 50000).frame)

# Valve parameters
#device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.OdorDwellTimeUS, 500000).frame)
#device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 1).frame)
device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableValveLeds, 1).frame)

# Set up simulated pokes
#valve_on_duration = 300000 # microseconds 
#reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.MinOdorDeliveryTimeUS, valve_on_duration).frame)

# Flow meter / ADC
device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableAdcSampling, 1).frame)
device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.ProportionalValve0Adc, 0).frame)

# PID
device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve0EnablePid, 1).frame)

pid_gain_array = pack_pid_config(2.0, 0.75, 0.18)
msg = WriteHarpMessage(
    PayloadType.U8,
    pid_gain_array,
    DelphiOnlyAppRegs.PidGains,
    offset=len(pid_gain_array) - 1,
)
device.send(msg.frame)

device.send(HarpMessage.WriteFloat(DelphiOnlyAppRegs.PidUpdateFrequency, 200.0).frame)
device.send(HarpMessage.WriteFloat(DelphiOnlyAppRegs.ProportionalValve0TargetFlowRate, 75.0).frame)


"""Valve bitmasks"""

VALVE_END   = 0x08  # valve 3 — end valve
VALVE_ODOR1 = 0x10  # valve 4 — odor A
VALVE_ODOR2 = 0x20  # valve 5 — odor B

def odor_mask(odor: int) -> int:
    return VALVE_ODOR1 if odor == 1 else VALVE_ODOR2

"""CSV setup"""

os.makedirs("data", exist_ok=True)
timestamp_str = datetime.now().strftime("%Y%m%d_%H%M%S")

csv_filename = os.path.join("data", f"analog_values_{timestamp_str}.csv")
csvfile = open(csv_filename, "w", newline="")
writer = csv.writer(csvfile)
#writer.writerow(["harp_timestamp", "ch0", "ch1", "ch2", "ch3", "ch4", "ch5", "ch6", "ch7"])
writer.writerow(["harp_timestamp", "In_Prop", "In", "Exhaust", "BottleOut", "BottleIn", "Alt", "NC", "miniPID"])

poke_csv_filename = os.path.join("data", f"poke_{timestamp_str}.csv")
poke_csvfile = open(poke_csv_filename, "w", newline="")
poke_writer = csv.writer(poke_csvfile)
poke_writer.writerow(["harp_timestamp", "odor1", "odor2", "poke"])

print(f"Logging ADC to {csv_filename} at {args.rate} Hz.")
print(f"Logging poke events to {poke_csv_filename} ({args.poke_on}s on / {args.poke_off}s off).")
print("Press Ctrl-C to stop.")

"""Poke state — odor 1 on at start"""

current_odor = 1   # which odor valve is currently active (1 or 2)
poke_active = False
pre_armed = False  # True once next odor is turned on ahead of the end valve
poke_count = 0     # increments each time the end valve opens

# Turn on initial odor
reply = device.send(HarpMessage.WriteU16(33, odor_mask(current_odor)).frame)
poke_writer.writerow([reply.timestamp, int(current_odor == 1), int(current_odor == 2), 0])

# How far in advance (clamped so it never exceeds poke_off)
pre_arm_advance = min(args.pre_arm_advance, args.poke_off)

"""Polling loop"""

PRINT_INTERVAL = 5.0  # seconds

last_sample = time.perf_counter()
last_print = time.perf_counter()
last_poke_change = time.perf_counter()
samples_since_print = 0
last_raw = None

active_poke_harp_ts = None  # Harp timestamp of the most recent poke-on (any odor)
active_poke_odor = None     # "odor1" or "odor2" for the poke being watched
waiting_for_ch7 = False     # True while watching ch7 for a rise after poke-on

flow_scale = 49.944*3.3/4096
flow_offset = 24.864 #To subtract from flow
try:
    while True:
        now = time.perf_counter()
        '''print('basic test')
        device.send(HarpMessage.WriteU16(34, 0x28).frame) #clear
        device.send(HarpMessage.WriteU16(33, 0x10).frame) #set
        time.sleep(1)
        device.send(HarpMessage.WriteU16(34, 0x10).frame)
        device.send(HarpMessage.WriteU16(33, 0x28).frame)
        time.sleep(1)
        print('end basic test') #'''
        # Poke state machine — evaluated before ADC so valve state is settled this iteration
        elapsed = now - last_poke_change

        if not poke_active:
            # Stage 1: pre-arm — swap to the next odor (off current, on next) atomically
            if not pre_armed and elapsed >= args.poke_off - pre_arm_advance:
                next_odor = 2 if current_odor == 1 else 1
                device.send(HarpMessage.WriteU16(34, odor_mask(current_odor)).frame)  # off current
                pre_arm_reply = device.send(HarpMessage.WriteU16(33, odor_mask(next_odor)).frame)  # on next
                current_odor = next_odor
                pre_armed = True
                poke_writer.writerow([pre_arm_reply.timestamp, int(current_odor == 1), int(current_odor == 2), 0])
                print(f"  [pre-arm] switched to odor{current_odor} ({pre_arm_advance:.1f}s before end valve)")

            # Stage 2: open end valve
            if elapsed >= args.poke_off:
                poke_reply = device.send(HarpMessage.WriteU16(33, VALVE_END).frame)
                poke_active = True
                poke_count += 1
                last_poke_change = now
                active_poke_harp_ts = poke_reply.timestamp
                active_poke_odor = f"odor{current_odor}"
                waiting_for_ch7 = True
                poke_writer.writerow([poke_reply.timestamp, int(current_odor == 1), int(current_odor == 2), 1])
                print(f"  [poke {poke_count}] end valve open — {active_poke_odor} active, armed ch7 detection")

        elif poke_active and elapsed >= args.poke_on:
            # Stage 3: close end valve only — odor already switched at pre-arm
            poke_reply = device.send(HarpMessage.WriteU16(34, VALVE_END).frame)
            pre_armed = False
            poke_active = False
            last_poke_change = now
            poke_writer.writerow([poke_reply.timestamp, int(current_odor == 1), int(current_odor == 2), 0])
            print(f"  [poke {poke_count}] end valve closed — odor{current_odor} continues")

        # ADC sampling — runs after poke state is settled for this iteration
        if now - last_sample >= interval:
            reply = device.send(
                HarpMessage.ReadFloat(DelphiOnlyAppRegs.LatestRawAdcSample).frame
            )
            raw = read_uint16_struct_from_u8(reply.payload, bytes_expected=16)
            harp_ts = reply.timestamp
            #writer.writerow([harp_ts, *raw])
            writer.writerow([harp_ts, *[v * flow_scale - flow_offset for v in raw[:6]], *raw[6:]]) # Attempt to write flows
            last_sample = now
            samples_since_print += 1
            last_raw = raw

            # Ch7 onset detection — active within timeout window after any poke-on
            if waiting_for_ch7:
                elapsed = harp_ts - active_poke_harp_ts
                if elapsed > args.ch7_timeout:
                    waiting_for_ch7 = False  # timed out, no signal detected
                elif raw[7] > args.ch7_threshold:
                    print(f"Ch7 signal detected: {elapsed*1000:.1f} ms after {active_poke_odor} poke-on (ch7={raw[7]})")
                    waiting_for_ch7 = False

        # Periodic status print
        if now - last_print >= PRINT_INTERVAL:
            achieved_hz = samples_since_print / PRINT_INTERVAL
            print(f"[{achieved_hz:.1f} Hz] latest_raw: {*[round(v * flow_scale - flow_offset, 2) for v in last_raw[:6]], *raw[6:]} | pokes={poke_count}, waiting_ch7={waiting_for_ch7}") #last_raw
            samples_since_print = 0
            last_print = now

except KeyboardInterrupt:
    print("Disabling FSM.")
    #device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 0).frame)
    #device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableAdcSampling, 0).frame)
    device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve0EnablePid, 0).frame)
    device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableAdcSampling, 0).frame)
    csvfile.close()
    poke_csvfile.close()
    device.disconnect()
