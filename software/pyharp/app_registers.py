"""ValveController app registers. Later these will be extracted from the device.yaml"""
from enum import IntEnum
from itertools import chain



class AppRegs(IntEnum):
    ValvesState = 32
    ValvesSet = 33
    ValvesClear = 34
    ValveConfigs0 = 35
    ValveConfigs1 = 36
    ValveConfigs2 = 37
    ValveConfigs3 = 38
    ValveConfigs4 = 39
    ValveConfigs5 = 40
    ValveConfigs6 = 41
    ValveConfigs7 = 42
    ValveConfigs8 = 43
    ValveConfigs9 = 44
    ValveConfigs10 = 45
    ValveConfigs11 = 46
    ValveConfigs12 = 47
    ValveConfigs13 = 48
    ValveConfigs14 = 49
    ValveConfigs15 = 50
    AuxGPIODir = 51
    AuxGPIOState = 52
    AuxGPIOSet = 53
    AuxGPIOClear = 54

    AuxGPIOInputRiseEvent = 55
    AuxGPIOInputFallEvent = 56
    AuxGPIOInputRisingInputs = 57
    AuxGPIOFallingInputs = 58


class DelphiOnlyAppRegs(IntEnum):
    PokePin = 59
    PokePinInverted = 60
    PokeState = 61
    PokeDometer = 62
    FSMEnabledState = 63
    ForceFSM = 64
    QueuedOdorIndex = 65
    VacuumCloseTimeUS = 66
    MinOdorDeliveryTimeUS = 67
    MaxOdorDeliveryTimeUS = 68
    OdorTransitionTimeUS = 69
    VacuumSetupTimeUS = 70
    FinalValveEnergizedTimeUS = 71
    MinimumPokeTimeUS = 72


DelphiAppRegs = IntEnum("DelphiAppRegs",
    [(i.name, i.value) for i in chain(AppRegs, DelphiOnlyAppRegs)])
