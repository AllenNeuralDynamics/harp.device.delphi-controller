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
    LatestFlowRate = 81
    LatestRawAdcSample = 82
    EnableAdcSampling = 83
    LeakAdcChannel = 84
    LeakThreshold = 85
    LeakState = 86
    ManualFlowMeter = 87
    NominalFlowRate = 88
    FlowRateTolerance = 89
    ManualFlowMeterState = 90
    FlowMeterCalibrations = 91
    PidUpdateFrequency = 92
    PidGains = 93
    ProportionalValve0Adc = 94
    ProportionalValve0EnablePid = 95
    ProportionalValve0DutyCycle = 96
    ProportionalValve0TargetFlowRate = 97
    ProportionalValve1Adc = 98
    ProportionalValve1EnablePid = 99
    ProportionalValve1DutyCycle = 100
    ProportionalValve1TargetFlowRate = 101
    ProportionalValve2Adc = 102
    ProportionalValve2EnablePid = 103
    ProportionalValve2DutyCycle = 104
    ProportionalValve2TargetFlowRate = 105
    FreezePidUpdates = 106


DelphiAppRegs = IntEnum(
    "DelphiAppRegs", [(i.name, i.value) for i in chain(AppRegs, DelphiOnlyAppRegs)]
)
