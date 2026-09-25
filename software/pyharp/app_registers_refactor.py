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
    OdorBuffer = 67
    ClearOdorBuffer = 68
    OdorSetupTimeUS = 69
    MinOdorDeliveryTimeUS = 70
    MaxOdorDeliveryTimeUS = 71
    MinimumPokeTimeUS = 72
    OdorDwellTimeUS = 73
    Cam0PinState = 74
    Cam0FrameRate = 75
    Cam0DutyCycle = 76
    EnableCam0Trigger = 77
    Cam1PinState = 78
    Cam1FrameRate = 79
    Cam1DutyCycle = 80
    EnableCam1Trigger = 81
    EnableValveLeds = 82
    LatestFlowRate = 83
    LatestRawAdcSample = 84
    EnableAdcSampling = 85
    LeakAdcChannel = 86
    LeakThreshold = 87
    LeakState = 88
    ManualFlowMeter = 89
    NominalFlowRate = 90
    FlowRateTolerance = 91
    ManualFlowMeterState = 92
    FlowMeterCalibrations = 93
    PidUpdateFrequency = 94
    PidGains = 95
    ProportionalValve0Adc = 96
    ProportionalValve0EnablePid = 97
    ProportionalValve0DutyCycle = 98
    ProportionalValve0TargetFlowRate = 99
    ProportionalValve1Adc = 100
    ProportionalValve1EnablePid = 101
    ProportionalValve1DutyCycle = 102
    ProportionalValve1TargetFlowRate = 103
    ProportionalValve2Adc = 104
    ProportionalValve2EnablePid = 105
    ProportionalValve2DutyCycle = 106
    ProportionalValve2TargetFlowRate = 107
    FreezePidUpdates = 108


DelphiAppRegs = IntEnum(
    "DelphiAppRegs", [(i.name, i.value) for i in chain(AppRegs, DelphiOnlyAppRegs)]
)
