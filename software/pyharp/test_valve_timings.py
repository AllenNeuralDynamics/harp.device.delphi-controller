#!/usr/bin/env python3
from pyharp.device import Device
from pyharp.messages import HarpMessage
from app_registers import AppRegs, DelphiOnlyAppRegs

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
com_port = "COM5"  #'COM3' #None
device = Device(com_port)
device.info()  # Display device's info on screen

print()
print("Enabling aux gpios 25-29 as outputs")
# gpio_dir = 32
# reply = device.send(HarpMessage.WriteU8(AppRegs.AuxGPIODir, gpio_dir).frame)
# print(f"reply: {reply.payload[0]:08b}")
gpio_set = 0b00100000
reply = device.send(HarpMessage.WriteU8(AppRegs.AuxGPIOSet, gpio_set).frame)
print(f"reply: {reply}")
print(f"reply: {reply.payload[0]:08b}")
# reply = device.send(HarpMessage.WriteU8(AppRegs.AuxGPIOClear, gpio_set).frame)
# print(f"reply: {reply.payload[0]:08b}")


print()
print_poke_counts(device)
print("Setting odor.")
reply = device.send(
    HarpMessage.WriteU16(DelphiOnlyAppRegs.QueuedOdorMask, 0x0001).frame
)
print("Assigning poke pin.")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePin, 22).frame)
print("Inverting poke pin.")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePinInverted, 1).frame)
print("Enabling FSM")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 1).frame)
print("Camera Pin")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.CamPin, 26).frame)
print("FPS")
reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.FrameRate, 100).frame)
print("Duty Cycle")
reply = device.send(HarpMessage.WriteFloat(DelphiOnlyAppRegs.DutyCycle, 0.5).frame)
print("Enable")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableCamTrigger, 1).frame)
print("Enable Valve LEDS")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableValveLeds, 0).frame)

"""Set Timings"""
# print("Set Odor Delivery Time")
# reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.OdorDeliveryTimeUS, 1000000).frame)
# print("Set Final Valve Energized Time")
# reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.FinalValveEnergizedTimeUS, 20000).frame)
print("Min Poke Time")
reply = device.send(
    HarpMessage.WriteU32(DelphiOnlyAppRegs.MinimumPokeTimeUS, 10000).frame
)

print()
odor_masks = [0x0002, 0x0004, 0x0008, 0x0003, 0x000F]
print(odor_masks)
odor_i = -1
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
                    print(f"New odor index: {odor_masks[odor_i]}")
                    reply = device.send(
                        HarpMessage.WriteU16(
                            DelphiOnlyAppRegs.QueuedOdorMask, odor_masks[odor_i]
                        ).frame
                    )

            """READ BASED ODOR UPDATING"""
            if event_address == 32:
                event_payload = msg.payload[0]
                print(f"Valves State: {event_payload}")
        # reply = device.send(HarpMessage.ReadU16(AppRegs.ValvesState).frame)
        # if reply.payload[0] != 0:
        # print(f"Valves State: {reply.payload[0]:16b}")
        # if reply.payload[0] == -1:
        # odor_i+=1
        # if odor_i > 3:
        #     odor_i = 0
        # print(f'New odor index: {odor_i}')
        # reply = device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.QueuedOdorIndex, odor_i).frame)

except KeyboardInterrupt:
    print("Disabling FSM.")
    reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 0).frame)
    device.disconnect()
