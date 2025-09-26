#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import HarpMessage
from pyharp.messages import MessageType
from app_registers import AppRegs, DelphiOnlyAppRegs
from struct import pack, unpack
from time import sleep
import os
import serial.tools.list_ports

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
com_port = 'COM4' #'COM3' #None
device = Device(com_port)
device.info()                           # Display device's info on screen

print()
print("Enabling all aux gpios as inputs.")
gpio_dir = 0b00000000
reply = device.send(HarpMessage.WriteU8(AppRegs.AuxGPIODir, gpio_dir).frame)
print(f"reply: {reply.payload[0]:08b}")
print()
print_poke_counts(device)
print(f"Setting odor.")
reply = device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.QueuedOdorIndex, 0).frame)
print(f"Assigning poke pin.")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePin, 22).frame)
print(f"Inverting poke pin.")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePinInverted, 1).frame)
print("Enabling FSM")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 1).frame)
print("Camera Pin")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.CamPin, 26).frame)
print("FPS")
reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.FrameRate, 10).frame)
print("Duty Cycle")
reply = device.send(HarpMessage.WriteFloat(DelphiOnlyAppRegs.DutyCycle, 0.5).frame)
print("Enable")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableCamTrigger, 1).frame)
print("Enable Valve LEDS")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.EnableValveLeds, 0).frame)

'''Set Timings'''
# print("Set Odor Delivery Time")
# reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.OdorDeliveryTimeUS, 1000000).frame)
# print("Set Final Valve Energized Time")
# reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.FinalValveEnergizedTimeUS, 20000).frame)
# print("Set Vacumm Setup Time")
# reply = device.send(HarpMessage.WriteU32(DelphiOnlyAppRegs.VacuumSetupTimeUS, 20000).frame)

print()
odor_i = 0
try:
    while True:
        for msg in device.get_events():
            print(msg)
            print()
            print_poke_counts(device)
        
        # odor is depleted -- assign a new one
        # fps = device.send(HarpMessage.ReadU32(DelphiOnlyAppRegs.FrameRate).frame)
        # print(f'FPS: {fps.payload[0]}')

        # cam = device.send(HarpMessage.ReadU8(DelphiOnlyAppRegs.CamPin).frame)
        # print(f'CAM: {cam.payload[0]}')
        # if poke_status.payload[0] != 0:
        #     print(f'Poke State: {poke_status.payload[0]}')
            
        reply = device.send(HarpMessage.ReadU8(DelphiOnlyAppRegs.QueuedOdorIndex).frame)
        if reply.payload[0] == -1:
            odor_i+=1
            if odor_i > 3:
                odor_i = 0
            print(f'New odor index: {odor_i}')
            reply = device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.QueuedOdorIndex, odor_i).frame)

except KeyboardInterrupt:
    print("Disabling FSM.")
    reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 0).frame)
    device.disconnect()
