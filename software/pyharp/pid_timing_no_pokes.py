#!/usr/bin/env python3
import argparse
import csv
import os
import struct
import threading
import time
from datetime import datetime
from typing import Iterable, Tuple

from pyharp.device import Device
from pyharp.messages import HarpMessage, WriteHarpMessage, PayloadType
from app_registers_refactor import DelphiOnlyAppRegs

import logging

logger = logging.getLogger()
logger.addHandler(logging.StreamHandler())

odor_2_extra = 1  ###CONTROL EXTRA ODOR 2 TIME

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
    default=5,
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
parser.add_argument(
    "--end-valve-always-open",
    action="store_true",
    help="Keep end valve open continuously; only odor valves toggle",
)
parser.add_argument(
    "--fixed-odor",
    type=int,
    choices=[1, 2],
    default=None,
    help="Keep one odor always on and toggle only the end valve (1 or 2)",
)
parser.add_argument(
    "--purge-pulse-ms",
    type=float,
    default=0.0,
    help="If >0, pulse solenoid valve 15 alone for this many ms during odor swaps when the end valve is closed (default: 0 = disabled)",
)
parser.add_argument(
    "--post-purge-dead-ms",
    type=float,
    default=10.0,
    help="After the purge pulse, hold all solenoid valves closed for this many ms before opening the next odor (default: 10)",
)
args = parser.parse_args()

interval = 1.0 / args.rate
# Clamped so the pre-arm window never exceeds the inter-trial gap
pre_arm_advance = min(args.pre_arm_advance, args.poke_off)

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
device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.ProportionalValve1Adc, 1).frame)

# PID
device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve0EnablePid, 1).frame)

device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.ProportionalValve1EnablePid, 1).frame)

pid_gain_array = pack_pid_config(2.0, 0.75, 0.18)
msg = WriteHarpMessage(
    PayloadType.U8,
    pid_gain_array,
    DelphiOnlyAppRegs.PidGains,
    offset=len(pid_gain_array) - 1,
)
device.send(msg.frame)

device.send(HarpMessage.WriteFloat(DelphiOnlyAppRegs.PidUpdateFrequency, 200.0).frame)

prop_valve_0_target = 15
prop_valve_1_target = 60
device.send(HarpMessage.WriteFloat(DelphiOnlyAppRegs.ProportionalValve0TargetFlowRate, prop_valve_0_target).frame)

device.send(HarpMessage.WriteFloat(DelphiOnlyAppRegs.ProportionalValve1TargetFlowRate, prop_valve_1_target).frame)


"""Valve bitmasks"""

VALVE_END   = 0x08    # valve 3 — end valve
VALVE_ODOR1 = 0x10    # valve 4 — odor A
VALVE_ODOR2 = 0x20    # valve 5 — odor B
VALVE_PURGE = 1 << 15 # valve 15 — purge between odor swaps

def odor_mask(odor: int) -> int:
    return VALVE_ODOR1 if odor == 1 else VALVE_ODOR2

"""CSV setup"""

timestamp_str = datetime.now().strftime("%Y%m%d_%H%M%S")
run_dir = os.path.join("data", timestamp_str)
os.makedirs(run_dir, exist_ok=True)

csv_filename = os.path.join(run_dir, f"analog_values_{timestamp_str}.csv")
csvfile = open(csv_filename, "w", newline="")
writer = csv.writer(csvfile)
#writer.writerow(["harp_timestamp", "ch0", "ch1", "ch2", "ch3", "ch4", "ch5", "ch6", "ch7"])
writer.writerow(["harp_timestamp", "In_Prop", "In_Dil", "Exhaust", "BottleOut", "BottleIn", "Alt", "NC", "miniPID"])

poke_csv_filename = os.path.join(run_dir, f"poke_{timestamp_str}.csv")
poke_csvfile = open(poke_csv_filename, "w", newline="")
poke_writer = csv.writer(poke_csvfile)
poke_writer.writerow(["harp_timestamp", "odor1", "odor2", "poke"])

info_filename = os.path.join(run_dir, "info.txt")

print(f"Logging to folder {run_dir}/ at {args.rate} Hz.")
print(f"Logging poke events to {poke_csv_filename} ({args.poke_on}s on / {args.poke_off}s off).")
print("Press Ctrl-C to stop.")

"""Experiment notes — collected in background thread"""

notes_lines = []
notes_saved = False

def _write_notes():
    if args.fixed_odor:
        mode = f"fixed-odor (odor{args.fixed_odor})"
    elif args.end_valve_always_open:
        mode = "end-valve-always-open (odors pre-swap, end valve held open)"
    else:
        mode = "alternating odors with pre-arm swap"

    odor2_on_s = args.poke_on * odor_2_extra

    with open(info_filename, "w") as f:
        f.write(f"Timestamp: {timestamp_str}\n")
        f.write(f"Mode: {mode}\n\n")

        f.write("Target flow rates:\n")
        f.write(f"  Proportional valve 0: {prop_valve_0_target} (units per register)\n")
        f.write(f"  Proportional valve 1: {prop_valve_1_target} (units per register)\n\n")

        f.write("Odor solenoid / end valve sequence per cycle:\n")
        if args.fixed_odor:
            f.write(f"  Phase 1 — odor{args.fixed_odor} on, end valve CLOSED: {args.poke_off:.3f} s\n")
            f.write(f"  Phase 2 — odor{args.fixed_odor} on, end valve OPEN:   {args.poke_on:.3f} s\n")
        elif args.end_valve_always_open:
            f.write(f"  Phase 1 — current odor on, end valve OPEN (hold):              {args.poke_off - pre_arm_advance:.3f} s\n")
            f.write(f"  Phase 2 — swap to next odor, end valve OPEN (pre-arm window): {pre_arm_advance:.3f} s\n")
            f.write(f"  Phase 3 — next odor on, end valve OPEN (trial window):\n")
            f.write(f"             odor 1: {args.poke_on:.3f} s\n")
            f.write(f"             odor 2: {odor2_on_s:.3f} s (poke_on x odor_2_extra={odor_2_extra})\n")
        else:
            f.write(f"  Phase 1 — current odor on, end valve CLOSED (inter-trial): {args.poke_off - pre_arm_advance:.3f} s\n")
            f.write(f"  Phase 2 — swap to next odor, end valve CLOSED (pre-arm):  {pre_arm_advance:.3f} s\n")
            f.write(f"  Phase 3 — next odor on, end valve OPEN (trial):\n")
            f.write(f"             odor 1: {args.poke_on:.3f} s\n")
            f.write(f"             odor 2: {odor2_on_s:.3f} s (poke_on x odor_2_extra={odor_2_extra})\n")
        if args.purge_pulse_ms > 0 and not args.fixed_odor and not args.end_valve_always_open:
            f.write(f"  Purge pulse: valve 15 pulsed alone for {args.purge_pulse_ms:.1f} ms during each odor swap (end valve closed)\n")
            f.write(f"  Post-purge dead time: all solenoid valves closed for {args.post_purge_dead_ms:.1f} ms after the pulse, before opening the next odor\n")
        else:
            f.write(f"  Purge pulse: disabled\n")
        f.write(f"\n  Raw params: poke_off={args.poke_off}s, poke_on={args.poke_on}s, "
                f"pre_arm_advance={pre_arm_advance}s (requested {args.pre_arm_advance}s), "
                f"odor_2_extra={odor_2_extra}\n")
        f.write(f"  ADC poll rate: {args.rate} Hz   ch7 threshold: {args.ch7_threshold}   ch7 timeout: {args.ch7_timeout} s\n\n")

        f.write("Notes: ")
        f.write(" ".join(notes_lines))
        f.write("\n")

def _collect_notes():
    global notes_saved
    print("\nEnter experiment notes (blank line to finish):")
    while True:
        try:
            line = input()
        except (EOFError, KeyboardInterrupt):
            break
        if line == "":
            break
        notes_lines.append(line)
    _write_notes()
    notes_saved = True
    print(f"[notes saved to {info_filename}]")

threading.Thread(target=_collect_notes, daemon=True).start()

"""Poke state — odor 1 on at start"""

current_odor = args.fixed_odor if args.fixed_odor else 1   # which odor valve is currently active (1 or 2)
poke_active = False
pre_armed = False  # True once next odor is turned on ahead of the end valve
poke_count = 0     # increments each time the end valve opens
purging = False              # True while a purge sequence is in progress (pulse + dead time)
purge_phase = None           # None | 'pulse' (valve 15 on) | 'dead' (all valves closed)
purge_phase_started_at = None  # perf_counter when the current purge phase began
pending_next_odor = None     # odor to turn on after the purge sequence ends

# Turn on initial odor (and end valve if always-open mode)
init_mask = odor_mask(current_odor) | (VALVE_END if args.end_valve_always_open else 0)
reply = device.send(HarpMessage.WriteU16(33, init_mask).frame)
poke_writer.writerow([reply.timestamp, int(current_odor == 1), int(current_odor == 2), int(args.end_valve_always_open)])
if args.end_valve_always_open:
    print("End valve always-open mode: end valve opened at start.")

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
        # Poke state machine — evaluated before ADC so valve state is settled this iteration
        elapsed = now - last_poke_change

        if not poke_active:
            # Stage 1: pre-arm — swap odors (skipped in fixed-odor mode)
            if not args.fixed_odor and not pre_armed and not purging and elapsed >= args.poke_off - pre_arm_advance:
                next_odor = 2 if current_odor == 1 else 1
                device.send(HarpMessage.WriteU16(34, odor_mask(current_odor)).frame)  # off current
                if args.purge_pulse_ms > 0 and not args.end_valve_always_open:
                    # Begin non-blocking purge pulse — ADC sampling continues during the pulse
                    purge_on_reply = device.send(HarpMessage.WriteU16(33, VALVE_PURGE).frame)
                    poke_writer.writerow([purge_on_reply.timestamp, 0, 0, 0])
                    purging = True
                    purge_phase = 'pulse'
                    purge_phase_started_at = now
                    pending_next_odor = next_odor
                    pre_armed = True  # block re-entry; the deferred swap below finishes the pre-arm
                    print(f"  [purge] valve 15 on (odor{current_odor}->odor{next_odor})")
                else:
                    pre_arm_reply = device.send(HarpMessage.WriteU16(33, odor_mask(next_odor)).frame)  # on next
                    current_odor = next_odor
                    pre_armed = True
                    poke_writer.writerow([pre_arm_reply.timestamp, int(current_odor == 1), int(current_odor == 2), int(args.end_valve_always_open)])
                    print(f"  [pre-arm] switched to odor{current_odor} ({pre_arm_advance:.1f}s before end valve)")
                    # Arm ch7 on B->A transition (always-open mode: odor flows immediately)
                    if current_odor == 1:
                        active_poke_harp_ts = pre_arm_reply.timestamp
                        active_poke_odor = "odor1"
                        waiting_for_ch7 = True
                        print(f"  [ch7] armed at odor2->odor1 switch")

            # Purge pulse end — close valve 15, then hold all solenoid valves closed for the dead-time window
            if purging and purge_phase == 'pulse' and (now - purge_phase_started_at) * 1000.0 >= args.purge_pulse_ms:
                purge_off_reply = device.send(HarpMessage.WriteU16(34, VALVE_PURGE).frame)
                poke_writer.writerow([purge_off_reply.timestamp, 0, 0, 0])
                if args.post_purge_dead_ms > 0:
                    purge_phase = 'dead'
                    purge_phase_started_at = now
                    print(f"  [purge] valve 15 off; all valves closed for {args.post_purge_dead_ms:.1f} ms dead time")
                else:
                    purge_phase = 'dead'
                    purge_phase_started_at = now  # zero-length dead time finishes on the next condition check below

            # Dead-time end — open the pending next odor once all-valves-closed window has elapsed
            if purging and purge_phase == 'dead' and (now - purge_phase_started_at) * 1000.0 >= args.post_purge_dead_ms:
                pre_arm_reply = device.send(HarpMessage.WriteU16(33, odor_mask(pending_next_odor)).frame)
                current_odor = pending_next_odor
                pending_next_odor = None
                purging = False
                purge_phase = None
                purge_phase_started_at = None
                poke_writer.writerow([pre_arm_reply.timestamp, int(current_odor == 1), int(current_odor == 2), int(args.end_valve_always_open)])
                print(f"  [pre-arm] switched to odor{current_odor} after {args.purge_pulse_ms:.1f} ms purge + {args.post_purge_dead_ms:.1f} ms dead time")
                if current_odor == 1:
                    active_poke_harp_ts = pre_arm_reply.timestamp
                    active_poke_odor = "odor1"
                    waiting_for_ch7 = True
                    print(f"  [ch7] armed at odor2->odor1 switch")

            # Stage 2: open end valve — held off while a purge is still in progress
            if not purging and elapsed >= args.poke_off:
                if args.fixed_odor or not args.end_valve_always_open:
                    poke_reply = device.send(HarpMessage.WriteU16(33, VALVE_END).frame)
                    poke_writer.writerow([poke_reply.timestamp, int(current_odor == 1), int(current_odor == 2), 1])
                    print(f"  [poke {poke_count + 1}] end valve open — odor{current_odor} active")
                    # Arm ch7 from end valve open timestamp in fixed-odor mode
                    if args.fixed_odor:
                        active_poke_harp_ts = poke_reply.timestamp
                        waiting_for_ch7 = True
                        print(f"  [ch7] armed at end valve open")
                poke_active = True
                poke_count += 1
                last_poke_change = now

        elif poke_active and elapsed >= (args.poke_on * odor_2_extra if current_odor == 2 else args.poke_on):
            # Stage 3: close end valve
            if args.fixed_odor or not args.end_valve_always_open:
                poke_reply = device.send(HarpMessage.WriteU16(34, VALVE_END).frame)
                poke_writer.writerow([poke_reply.timestamp, int(current_odor == 1), int(current_odor == 2), 0])
                print(f"  [poke {poke_count}] end valve closed — odor{current_odor} continues")
            pre_armed = False
            poke_active = False
            last_poke_change = now

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

            # Ch7 onset detection — stays armed until signal is seen
            if waiting_for_ch7:
                elapsed_ch7 = harp_ts - active_poke_harp_ts
                if raw[7] > args.ch7_threshold:
                    print(f"Ch7 signal detected: {elapsed_ch7*1000:.1f} ms after odor2→odor1 switch (ch7={raw[7]})")
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
    if not notes_saved:
        _write_notes()
        print(f"[notes saved to {info_filename}]")
    device.disconnect()
