"""ValveController app registers. Later these will be extracted from the device.yaml"""
from enum import IntEnum



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


class DelphiAppRegs(AppRegs):
    PokePinMask = 59
    PokeEvent = 60
    PokeDometers = 61
    FSMState = 62
    ForceFSM = 63
    CurrentOdorIndex = 64
    NextOdorIndex = 65
    VacuumCloseTimeUS = 66
    OdorDeliveryTimeUS = 67
    OdorTransitionTimeUS = 68
    VacuumSetupTimeUS = 69
    FinalValveEnergizedTimeUS = 70
    MinimumPokeTimeUS = 71
