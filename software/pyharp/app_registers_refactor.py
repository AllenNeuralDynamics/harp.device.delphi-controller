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
    RawPokeState = 62
    PokeDometer = 63
    FSMEnabledState = 64
    ForceFSM = 65
    QueuedOdorMask = 66
    OdorSetupTimeUS = 67
    MinOdorDeliveryTimeUS = 68
    MaxOdorDeliveryTimeUS = 69
    MinimumPokeTimeUS = 70
    CamPin = 71
    CamPinState = 72
    FrameRate = 73
    DutyCycle = 74
    EnableCamTrigger = 75
    EnableValveLeds = 76
    LatestAdcSample = 77
    EnableAdcSampling = 78
    AdcSamplingRate = 79
    LeakAdcChannel = 80
    LeakThreshold = 81


DelphiAppRegs = IntEnum(
    "DelphiAppRegs", [(i.name, i.value) for i in chain(AppRegs, DelphiOnlyAppRegs)]
)
