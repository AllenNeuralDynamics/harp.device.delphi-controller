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

# Open serial connection with the first Valve Controller.
com_port = None
ports = serial.tools.list_ports.comports()
for port, desc, hwid in sorted(ports):
    if desc.startswith("delphi-controller"):
        print("{}: {} [{}]".format(port, desc, hwid))
        com_port = port
        break
device = Device(com_port)
device.info()                           # Display device's info on screen

print()
print("Enabling all aux gpios as inputs.")
gpio_dir = 0b00000000
reply = device.send(HarpMessage.WriteU8(AppRegs.AuxGPIODir, gpio_dir).frame)
print(f"reply: {reply.payload[0]:08b}")
print()
reply = device.send(HarpMessage.ReadU8(DelphiOnlyAppRegs.PokeDometer).frame)
print(f"Current pokedometer count is: {reply.payload}.")
print(f"Setting next odor.")
reply = device.send(HarpMessage.WriteS8(DelphiOnlyAppRegs.NextOdorIndex, 0).frame)
print(f"Assigning poke pin.")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePin, 22).frame)
print(f"Inverting poke pin.")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.PokePinInverted, 1).frame)
print("Enabling FSM")
reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 1).frame)
print()

try:
    while True:
        for msg in device.get_events():
            print(msg)
            print()
except KeyboardInterrupt:
    print("Disabling FSM.")
    reply = device.send(HarpMessage.WriteU8(DelphiOnlyAppRegs.FSMEnabledState, 0).frame)
    device.disconnect()
