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
    OdorDwellTimeUS = 71
    Cam0PinState = 72
    Cam0FrameRate = 73
    Cam0DutyCycle = 74
    EnableCam0Trigger = 75
    Cam1PinState = 76
    Cam1FrameRate = 77
    Cam1DutyCycle = 78
    EnableCam1Trigger = 79
    EnableValveLeds = 80
    AdcMask = 81
    LatestFlowRate = 82
    LatestRawAdcSample = 83
    EnableAdcSampling = 84
    AdcSamplingRate = 85
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


DelphiAppRegs = IntEnum(
    "DelphiAppRegs", [(i.name, i.value) for i in chain(AppRegs, DelphiOnlyAppRegs)]
)
