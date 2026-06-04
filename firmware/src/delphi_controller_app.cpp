#include <delphi_controller_app.h>

app_regs_t app_regs;
uint8_t old_aux_gpio_inputs;
uint8_t led_state;


// Create function aliases for readability.
void (&read_aux_gpio_dir)(uint8_t reg_address) = HarpCore::read_reg_generic;
void (&read_aux_gpio_set)(uint8_t reg_address) = HarpCore::read_reg_generic;
void (&read_aux_gpio_clear)(uint8_t reg_address) = HarpCore::read_reg_generic;

void (&read_aux_gpio_rise_event)(uint8_t reg_address) = HarpCore::read_reg_generic;
void (&read_aux_gpio_fall_event)(uint8_t reg_address) = HarpCore::read_reg_generic;
void (&write_aux_gpio_rise_event)(msg_t& msg) = HarpCore::write_reg_generic;
void (&write_aux_gpio_fall_event)(msg_t& msg) = HarpCore::write_reg_generic;

void (&read_aux_gpio_rise_input)(uint8_t reg_address) = HarpCore::read_reg_generic;
void (&read_aux_gpio_fall_input)(uint8_t reg_address) = HarpCore::read_reg_generic;


void (&read_force_fsm)(uint8_t reg_address) = HarpCore::read_reg_generic;

/// Create Hit-and-Hold Valve Drivers.
/// The underlying PWM peripheral, aka: a PWM Slice, controls two adjacent PWM
/// pins and must be configured with the same settings. This is OK since we are
/// enforcing the same underlying peripheral settings (i.e: frequency) across
/// all Slices.
ValveDriver valve_drivers[NUM_VALVES]
{{VALVE_PIN_BASE},
 {VALVE_PIN_BASE + 1},
 {VALVE_PIN_BASE + 2},
 {VALVE_PIN_BASE + 3},
 {VALVE_PIN_BASE + 4},
 {VALVE_PIN_BASE + 5},
 {VALVE_PIN_BASE + 6},
 {VALVE_PIN_BASE + 7},
 {VALVE_PIN_BASE + 8},
 {VALVE_PIN_BASE + 9},
 {VALVE_PIN_BASE + 10},
 {VALVE_PIN_BASE + 11},
 {VALVE_PIN_BASE + 12},
 {VALVE_PIN_BASE + 13},
 {VALVE_PIN_BASE + 14},
 {VALVE_PIN_BASE + 15}};

// Define "specs" per-register
RegSpecs app_reg_specs[APP_REG_COUNT]
{
    {(uint8_t*)&app_regs.ValvesState, sizeof(app_regs.ValvesState), U16},
    {(uint8_t*)&app_regs.ValvesSet, sizeof(app_regs.ValvesSet), U16},
    {(uint8_t*)&app_regs.ValvesClear, sizeof(app_regs.ValvesClear), U16},

    {(uint8_t*)&app_regs.ValveConfigs[0], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[1], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[2], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[3], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[4], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[5], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[6], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[7], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[8], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[9], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[10], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[11], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[12], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[13], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[14], sizeof(ValveConfig), U8},
    {(uint8_t*)&app_regs.ValveConfigs[15], sizeof(ValveConfig), U8},

    {(uint8_t*)&app_regs.AuxGPIODir, sizeof(app_regs.AuxGPIODir), U8},
    {(uint8_t*)&app_regs.AuxGPIOState, sizeof(app_regs.AuxGPIOState), U8},
    {(uint8_t*)&app_regs.AuxGPIOSet, sizeof(app_regs.AuxGPIOSet), U8},
    {(uint8_t*)&app_regs.AuxGPIOClear, sizeof(app_regs.AuxGPIOClear), U8},

    {(uint8_t*)&app_regs.AuxGPIOInputRiseEvent, sizeof(app_regs.AuxGPIOInputRiseEvent), U8},
    {(uint8_t*)&app_regs.AuxGPIOInputFallEvent, sizeof(app_regs.AuxGPIOInputFallEvent), U8},
    {(uint8_t*)&app_regs.AuxGPIORisingInputs, sizeof(app_regs.AuxGPIORisingInputs), U8},
    {(uint8_t*)&app_regs.AuxGPIOFallingInputs, sizeof(app_regs.AuxGPIOFallingInputs), U8},

    // More specs here for poke manager registers.
    {(uint8_t*)&app_regs.PokePin, sizeof(app_regs.PokePin), U8},
    {(uint8_t*)&app_regs.PokePinInverted, sizeof(app_regs.PokePinInverted), U8},
    {(uint8_t*)&app_regs.PokeState, sizeof(app_regs.PokeState), U8},
    {(uint8_t*)&app_regs.RawPokeState, sizeof(app_regs.RawPokeState), U8},
    {(uint8_t*)&app_regs.PokeDometer, sizeof(app_regs.PokeDometer), U32},
    {(uint8_t*)&app_regs.FSMEnabledState, sizeof(app_regs.FSMEnabledState), U8},
    {(uint8_t*)&app_regs.ForceFSM, sizeof(app_regs.ForceFSM), U8},
    {(uint8_t*)&app_regs.QueuedOdorMask, sizeof(app_regs.QueuedOdorMask), U16},
    {(uint8_t*)&app_regs.OdorSetupTimeUS, sizeof(app_regs.OdorSetupTimeUS), U32},
    {(uint8_t*)&app_regs.MinOdorDeliveryTimeUS, sizeof(app_regs.MinOdorDeliveryTimeUS), U32},
    {(uint8_t*)&app_regs.MaxOdorDeliveryTimeUS, sizeof(app_regs.MaxOdorDeliveryTimeUS), U32},
    {(uint8_t*)&app_regs.MinimumPokeTimeUS, sizeof(app_regs.MinimumPokeTimeUS), U32},
    {(uint8_t*)&app_regs.OdorDwellTimeUS, sizeof(app_regs.OdorDwellTimeUS), U32},

    // Camera 0 specs
    {(uint8_t*)&app_regs.Cam0PinState, sizeof(app_regs.Cam0PinState), U8},
    {(uint8_t*)&app_regs.Cam0FrameRate, sizeof(app_regs.Cam0FrameRate), U32},
    {(uint8_t*)&app_regs.Cam0DutyCycle, sizeof(app_regs.Cam0DutyCycle), Float},
    {(uint8_t*)&app_regs.EnableCam0Trigger, sizeof(app_regs.EnableCam0Trigger), U8},

    // Camera 1 specs
    {(uint8_t*)&app_regs.Cam1PinState, sizeof(app_regs.Cam1PinState), U8},
    {(uint8_t*)&app_regs.Cam1FrameRate, sizeof(app_regs.Cam1FrameRate), U32},
    {(uint8_t*)&app_regs.Cam1DutyCycle, sizeof(app_regs.Cam1DutyCycle), Float},
    {(uint8_t*)&app_regs.EnableCam1Trigger, sizeof(app_regs.EnableCam1Trigger), U8},

    {(uint8_t*)&app_regs.EnableValveLeds, sizeof(app_regs.EnableValveLeds), U8},

    // ADC specs
    {(uint8_t*)&app_regs.LatestFlowRate, sizeof(app_regs.LatestFlowRate), U8},
    {(uint8_t*)&app_regs.LatestRawAdcSample, sizeof(app_regs.LatestRawAdcSample), U8},
    {(uint8_t*)&app_regs.EnableAdcSampling, sizeof(app_regs.EnableAdcSampling), U8},
    {(uint8_t*)&app_regs.LeakAdcChannel, sizeof(app_regs.LeakAdcChannel), S8},
    {(uint8_t*)&app_regs.LeakThreshold, sizeof(app_regs.LeakThreshold), Float},
    {(uint8_t*)&app_regs.LeakState, sizeof(app_regs.LeakState), U8},
    {(uint8_t*)&app_regs.ManualFlowMeter, sizeof(app_regs.ManualFlowMeter), S8},
    {(uint8_t*)&app_regs.NominalFlowRate, sizeof(app_regs.NominalFlowRate), Float},
    {(uint8_t*)&app_regs.FlowRateTolerance, sizeof(app_regs.FlowRateTolerance), Float},
    {(uint8_t*)&app_regs.ManualFlowMeterState, sizeof(app_regs.ManualFlowMeterState), U8},
    {(uint8_t*)&app_regs.FlowMeterCalibrations, sizeof(app_regs.FlowMeterCalibrations), U8},

    {(uint8_t*)&app_regs.PidUpdateFrequency, sizeof(app_regs.PidUpdateFrequency), Float},
    {(uint8_t*)&app_regs.PidGains, sizeof(PidConfig), U8},

    {(uint8_t*)&app_regs.ProportionalValve0Adc, sizeof(app_regs.ProportionalValve0Adc), S8},
    {(uint8_t*)&app_regs.ProportionalValve0EnablePid, sizeof(app_regs.ProportionalValve0EnablePid), U8},
    {(uint8_t*)&app_regs.ProportionalValve0DutyCycle, sizeof(app_regs.ProportionalValve0DutyCycle), Float},
    {(uint8_t*)&app_regs.ProportionalValve0TargetFlowRate, sizeof(app_regs.ProportionalValve0TargetFlowRate), Float},
    {(uint8_t*)&app_regs.ProportionalValve1Adc, sizeof(app_regs.ProportionalValve1Adc), S8},
    {(uint8_t*)&app_regs.ProportionalValve1EnablePid, sizeof(app_regs.ProportionalValve1EnablePid), U8},
    {(uint8_t*)&app_regs.ProportionalValve1DutyCycle, sizeof(app_regs.ProportionalValve1DutyCycle), Float},
    {(uint8_t*)&app_regs.ProportionalValve1TargetFlowRate, sizeof(app_regs.ProportionalValve1TargetFlowRate), Float},
    {(uint8_t*)&app_regs.ProportionalValve2Adc, sizeof(app_regs.ProportionalValve2Adc), S8},
    {(uint8_t*)&app_regs.ProportionalValve2EnablePid, sizeof(app_regs.ProportionalValve2EnablePid), U8},
    {(uint8_t*)&app_regs.ProportionalValve2DutyCycle, sizeof(app_regs.ProportionalValve2DutyCycle), Float},
    {(uint8_t*)&app_regs.ProportionalValve2TargetFlowRate, sizeof(app_regs.ProportionalValve2TargetFlowRate), Float},
    {(uint8_t*)&app_regs.FreezePidUpdates, sizeof(app_regs.FreezePidUpdates), U8}
};

RegFnPair reg_handler_fns[APP_REG_COUNT]
{
    {read_valves_state, write_valves_state},
    {read_valves_set, write_valves_set},
    {read_valves_clear, write_valves_clear},

    {read_any_valve_config, write_any_valve_config}, // valve 0
    {read_any_valve_config, write_any_valve_config}, // valve 1
    {read_any_valve_config, write_any_valve_config}, // ...
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config},
    {read_any_valve_config, write_any_valve_config}, // valve 15

    {read_aux_gpio_dir, write_aux_gpio_dir},
    {read_aux_gpio_state, write_aux_gpio_state},
    {read_aux_gpio_set, write_aux_gpio_set},
    {read_aux_gpio_clear, write_aux_gpio_clear},

    {read_aux_gpio_rise_event, write_aux_gpio_rise_event},
    {read_aux_gpio_fall_event, write_aux_gpio_fall_event},
    {read_aux_gpio_rise_input, HarpCore::write_to_read_only_reg_error},
    {read_aux_gpio_fall_input, HarpCore::write_to_read_only_reg_error},

    // Poke manager handler functions
    {read_poke_pin, write_poke_pin},
    {read_poke_pin_inverted, write_poke_pin_inverted},
    {read_poke_state, HarpCore::write_to_read_only_reg_error},
    {read_raw_poke_state, HarpCore::write_to_read_only_reg_error},
    {read_pokedometer, HarpCore::write_to_read_only_reg_error},
    {read_fsm_enabled_state, write_fsm_enabled_state},
    {read_force_fsm, write_force_fsm},
    {read_current_odors, write_current_odors},
    {read_odor_setup_time_us, write_odor_setup_time_us},
    {read_min_odor_delivery_time_us, write_min_odor_delivery_time_us},
    {read_max_odor_delivery_time_us, write_max_odor_delivery_time_us},
    {read_minimum_poke_time_us, write_minimum_poke_time_us},
    {read_odor_dwell_time_us, write_odor_dwell_time_us},
    {read_cam0_pin_state, HarpCore::write_to_read_only_reg_error},
    {read_cam0_frame_rate, write_cam0_frame_rate},
    {read_cam0_duty_cycle, write_cam0_duty_cycle},
    {read_enable_cam0_trigger, write_enable_cam0_trigger},
    {read_cam1_pin_state, HarpCore::write_to_read_only_reg_error},
    {read_cam1_frame_rate, write_cam1_frame_rate},
    {read_cam1_duty_cycle, write_cam1_duty_cycle},
    {read_enable_cam1_trigger, write_enable_cam1_trigger},
    {read_valve_leds, write_valve_leds},

    // ADC handler functions
    {read_adc, HarpCore::write_to_read_only_reg_error},
    {read_raw_adc, HarpCore::write_to_read_only_reg_error},
    {read_adc_enable, write_adc_enable},
    {read_leak_adc_channel, write_leak_adc_channel},
    {read_leak_threshold, write_leak_threshold},
    {read_leak_state, HarpCore::write_to_read_only_reg_error},

    // Manual flow meter calibration handler functions
    {read_manual_flow_meter, write_manual_flow_meter},
    {read_nominal_flow_rate, write_nominal_flow_rate},
    {read_flow_rate_tolerance, write_flow_rate_tolerance},
    {read_manual_flow_meter_state, HarpCore::write_to_read_only_reg_error},
    {read_flow_meter_calibrations, write_flow_meter_calibrations},

    // Proportional valve handler functions
    {read_pid_update_frequency, write_pid_update_frequency},
    {read_pid_gains, write_pid_gains},

    {read_proportional_valve_0_adc, write_proportional_valve_0_adc},
    {read_proportional_valve_0_enable_pid, write_proportional_valve_0_enable_pid},
    {read_proportional_valve_0_duty_cycle, write_proportional_valve_0_duty_cycle},
    {read_proportional_valve_0_target_flow_rate, write_proportional_valve_0_target_flow_rate},
    {read_proportional_valve_1_adc, write_proportional_valve_1_adc},
    {read_proportional_valve_1_enable_pid, write_proportional_valve_1_enable_pid},
    {read_proportional_valve_1_duty_cycle, write_proportional_valve_1_duty_cycle},
    {read_proportional_valve_1_target_flow_rate, write_proportional_valve_1_target_flow_rate},
    {read_proportional_valve_2_adc, write_proportional_valve_2_adc},
    {read_proportional_valve_2_enable_pid, write_proportional_valve_2_enable_pid},
    {read_proportional_valve_2_duty_cycle, write_proportional_valve_2_duty_cycle},
    {read_proportional_valve_2_target_flow_rate, write_proportional_valve_2_target_flow_rate},
    {read_freeze_pid_updates, write_freeze_pid_updates}
};

void read_freeze_pid_updates(uint8_t reg_address)
{
    app_regs.FreezePidUpdates = freeze_pid_updates;
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_freeze_pid_updates(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    freeze_pid_updates = app_regs.FreezePidUpdates;
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_raw_adc(uint8_t reg_address)
{
    app_regs.LatestRawAdcSample = flow_detection.get_latest_raw_adc_sample();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void read_proportional_valve_0_target_flow_rate(uint8_t reg_address)
{
    app_regs.ProportionalValve0TargetFlowRate = proportional_valve_0_controller.get_target_flow_rate();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_0_target_flow_rate(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_0_controller.set_target_flow_rate(app_regs.ProportionalValve0TargetFlowRate);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_1_target_flow_rate(uint8_t reg_address)
{
    app_regs.ProportionalValve1TargetFlowRate = proportional_valve_1_controller.get_target_flow_rate();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_1_target_flow_rate(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_1_controller.set_target_flow_rate(app_regs.ProportionalValve1TargetFlowRate);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_2_target_flow_rate(uint8_t reg_address)
{
    app_regs.ProportionalValve2TargetFlowRate = proportional_valve_2_controller.get_target_flow_rate();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_2_target_flow_rate(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_2_controller.set_target_flow_rate(app_regs.ProportionalValve2TargetFlowRate);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_0_duty_cycle(uint8_t reg_address)
{
    app_regs.ProportionalValve0DutyCycle = proportional_valve_0_controller.get_duty_cycle();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_0_duty_cycle(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_0_controller.set_duty_cycle(app_regs.ProportionalValve0DutyCycle);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_1_duty_cycle(uint8_t reg_address)
{
    app_regs.ProportionalValve1DutyCycle = proportional_valve_1_controller.get_duty_cycle();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_1_duty_cycle(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_1_controller.set_duty_cycle(app_regs.ProportionalValve1DutyCycle);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_2_duty_cycle(uint8_t reg_address)
{
    app_regs.ProportionalValve2DutyCycle = proportional_valve_2_controller.get_duty_cycle();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_2_duty_cycle(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_2_controller.set_duty_cycle(app_regs.ProportionalValve2DutyCycle);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_0_enable_pid(uint8_t reg_address)
{
    app_regs.ProportionalValve0EnablePid = proportional_valve_0_controller.get_pid_enabled();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_0_enable_pid(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_0_controller.set_pid_enabled(app_regs.ProportionalValve0EnablePid);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_1_enable_pid(uint8_t reg_address)
{
    app_regs.ProportionalValve1EnablePid = proportional_valve_1_controller.get_pid_enabled();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_1_enable_pid(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_1_controller.set_pid_enabled(app_regs.ProportionalValve1EnablePid);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_2_enable_pid(uint8_t reg_address)
{
    app_regs.ProportionalValve2EnablePid = proportional_valve_2_controller.get_pid_enabled();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_2_enable_pid(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_2_controller.set_pid_enabled(app_regs.ProportionalValve2EnablePid);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_0_adc(uint8_t reg_address)
{
    app_regs.ProportionalValve0Adc = proportional_valve_0_controller.get_flow_adc_index();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_0_adc(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_0_controller.set_flow_adc_index(app_regs.ProportionalValve0Adc);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_1_adc(uint8_t reg_address)
{
    app_regs.ProportionalValve1Adc = proportional_valve_1_controller.get_flow_adc_index();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_1_adc(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_1_controller.set_flow_adc_index(app_regs.ProportionalValve1Adc);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_proportional_valve_2_adc(uint8_t reg_address)
{
    app_regs.ProportionalValve2Adc = proportional_valve_2_controller.get_flow_adc_index();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_proportional_valve_2_adc(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_2_controller.set_flow_adc_index(app_regs.ProportionalValve2Adc);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_pid_gains(uint8_t reg_address)
{
    PidConfig& pid_cfg = app_regs.PidGains;

    // Update Harp App registers with ValveDriver class contents.
    pid_cfg.kp = proportional_valve_0_controller.get_kp();
    pid_cfg.ki = proportional_valve_0_controller.get_ki();
    pid_cfg.kd = proportional_valve_0_controller.get_kd();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_pid_gains(msg_t& msg)
{

    HarpCore::copy_msg_payload_to_register(msg);
    const PidConfig& pid_cfg = app_regs.PidGains;

    // Apply the configuration.
    proportional_valve_0_controller.set_kp(pid_cfg.kp);
    proportional_valve_0_controller.set_ki(pid_cfg.ki);
    proportional_valve_0_controller.set_kd(pid_cfg.kd);

    proportional_valve_1_controller.set_kp(pid_cfg.kp);
    proportional_valve_1_controller.set_ki(pid_cfg.ki);
    proportional_valve_1_controller.set_kd(pid_cfg.kd);

    proportional_valve_2_controller.set_kp(pid_cfg.kp);
    proportional_valve_2_controller.set_ki(pid_cfg.ki);
    proportional_valve_2_controller.set_kd(pid_cfg.kd);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_pid_update_frequency(uint8_t reg_address)
{
    app_regs.PidUpdateFrequency = proportional_valve_0_controller.get_pid_update_frequency();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_pid_update_frequency(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    proportional_valve_0_controller.set_pid_update_frequency(app_regs.PidUpdateFrequency);  // All valves share the same PID update frequency.
    proportional_valve_1_controller.set_pid_update_frequency(app_regs.PidUpdateFrequency);
    proportional_valve_2_controller.set_pid_update_frequency(app_regs.PidUpdateFrequency);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}



void read_flow_meter_calibrations(uint8_t reg_address)
{
    app_regs.FlowMeterCalibrations = flow_detection.get_flow_meter_calibrations();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}


void write_flow_meter_calibrations(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    flow_detection.set_flow_meter_calibrations(app_regs.FlowMeterCalibrations);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_manual_flow_meter_state(uint8_t reg_address)
{
    app_regs.ManualFlowMeterState = flow_detection.get_manual_flow_meter_state();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void read_flow_rate_tolerance(uint8_t reg_address)
{
    app_regs.FlowRateTolerance = flow_detection.get_flow_rate_tolerance();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_flow_rate_tolerance(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    flow_detection.set_flow_rate_tolerance(app_regs.FlowRateTolerance);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_nominal_flow_rate(uint8_t reg_address)
{
    app_regs.NominalFlowRate = flow_detection.get_nominal_flow_rate();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_nominal_flow_rate(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    flow_detection.set_nominal_flow_rate(app_regs.NominalFlowRate);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_manual_flow_meter(uint8_t reg_address)
{
    app_regs.ManualFlowMeter = flow_detection.get_manual_flow_meter();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_manual_flow_meter(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    flow_detection.set_manual_flow_meter(app_regs.ManualFlowMeter);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_leak_state(uint8_t reg_address)
{
    app_regs.LeakState = flow_detection.get_leak_state();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void read_leak_threshold(uint8_t reg_address)
{
    app_regs.LeakThreshold = flow_detection.get_leak_threshold();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_leak_threshold(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    flow_detection.set_leak_threshold(app_regs.LeakThreshold);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_leak_adc_channel(uint8_t reg_address)
{
    app_regs.LeakAdcChannel = flow_detection.get_leak_adc();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_leak_adc_channel(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    flow_detection.set_leak_adc(app_regs.LeakAdcChannel);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_adc(uint8_t reg_address)
{
    app_regs.LatestFlowRate = flow_detection.get_latest_adc_sample();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address, HarpCore::harp_time_us_64());
}

void read_adc_enable(uint8_t reg_address)
{
    app_regs.EnableAdcSampling = flow_detection.get_adc_enabled_status();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_adc_enable(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    flow_detection.set_sampling_enabled(app_regs.EnableAdcSampling);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_valve_leds(uint8_t reg_address)
{
    app_regs.EnableValveLeds = gpio_get(LED_ENABLE_PIN);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_valve_leds(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    gpio_put(LED_ENABLE_PIN, app_regs.EnableValveLeds);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_enable_cam0_trigger(uint8_t reg_address)
{
    app_regs.EnableCam0Trigger = cam0_driver.get_enable_state();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_enable_cam0_trigger(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam0_driver.set_enable_state(app_regs.EnableCam0Trigger);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_cam0_duty_cycle(uint8_t reg_address)
{
    app_regs.Cam0DutyCycle = cam0_driver.get_pwm_duty();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_cam0_duty_cycle(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam0_driver.set_pwm_duty_cycle(app_regs.Cam0DutyCycle);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_cam0_frame_rate(uint8_t reg_address)
{
    app_regs.Cam0FrameRate = cam0_driver.get_pwm_freq();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_cam0_frame_rate(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam0_driver.set_pwm_freq(app_regs.Cam0FrameRate);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_cam0_pin_state(uint8_t reg_address)
{
    app_regs.Cam0PinState = cam0_driver.get_pwm_pin_state(); 
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void read_enable_cam1_trigger(uint8_t reg_address)
{
    app_regs.EnableCam1Trigger = cam1_driver.get_enable_state();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_enable_cam1_trigger(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam1_driver.set_enable_state(app_regs.EnableCam1Trigger);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_cam1_duty_cycle(uint8_t reg_address)
{
    app_regs.Cam1DutyCycle = cam1_driver.get_pwm_duty();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_cam1_duty_cycle(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam1_driver.set_pwm_duty_cycle(app_regs.Cam1DutyCycle);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_cam1_frame_rate(uint8_t reg_address)
{
    app_regs.Cam1FrameRate = cam1_driver.get_pwm_freq();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_cam1_frame_rate(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam1_driver.set_pwm_freq(app_regs.Cam1FrameRate);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void read_cam1_pin_state(uint8_t reg_address)
{
    app_regs.Cam1PinState = cam1_driver.get_pwm_pin_state(); 
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void read_poke_pin(uint8_t reg_address)
{
    app_regs.PokePin = poke_manager.get_poke_pin();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_poke_pin(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_poke_pin(app_regs.PokePin);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_poke_pin_inverted(uint8_t reg_address)
{
    app_regs.PokePinInverted = poke_manager.poke_pin_is_inverted();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_poke_pin_inverted(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_poke_pin_override_state(gpio_override(app_regs.PokePinInverted));
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_poke_state(uint8_t reg_address)
{
    // FIXME
    app_regs.PokeState = poke_manager.get_poke_state(); 
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void read_raw_poke_state(uint8_t reg_address)
{
    // FIXME
    app_regs.RawPokeState = poke_manager.get_raw_poke_state(); 
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void read_pokedometer(uint8_t reg_address)
{
    app_regs.PokeDometer = poke_manager.get_poke_count();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void read_fsm_enabled_state(uint8_t reg_address)
{
    // FIXME: doesn't exist.
//    app_regs.FSMEnabledState = poke_manager.get_enabled_state();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_fsm_enabled_state(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_enabled_state(app_regs.FSMEnabledState);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_force_fsm(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.force_poke();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_current_odors(uint8_t reg_address)
{
    // Get recent poke count value
    app_regs.QueuedOdorMask = poke_manager.get_current_odors();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_current_odors(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_current_odors(app_regs.QueuedOdorMask);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_odor_setup_time_us(uint8_t reg_address)
{
    app_regs.OdorSetupTimeUS = poke_manager.get_odor_setup_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_odor_setup_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_odor_setup_time_us(app_regs.OdorSetupTimeUS);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_min_odor_delivery_time_us(uint8_t reg_address)
{
    app_regs.MinOdorDeliveryTimeUS = poke_manager.get_min_odor_delivery_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_min_odor_delivery_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_min_odor_delivery_time_us(app_regs.MinOdorDeliveryTimeUS);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_odor_dwell_time_us(uint8_t reg_address)
{
    app_regs.OdorDwellTimeUS = poke_manager.get_odor_dwell_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_odor_dwell_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_odor_dwell_time_us(app_regs.OdorDwellTimeUS);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_max_odor_delivery_time_us(uint8_t reg_address)
{
    app_regs.MaxOdorDeliveryTimeUS = poke_manager.get_max_odor_delivery_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_max_odor_delivery_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_max_odor_delivery_time_us(app_regs.MaxOdorDeliveryTimeUS);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_minimum_poke_time_us(uint8_t reg_address)
{
    app_regs.MinimumPokeTimeUS = poke_manager.get_min_poke_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_minimum_poke_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_min_poke_time_us(app_regs.MinimumPokeTimeUS);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void read_valves_state(uint8_t reg_address)
{
    app_regs.ValvesState = get_valve_mask();  // Store the mask
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_valves_state(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    for (size_t valve_index = 0; valve_index < NUM_VALVES; ++valve_index)
    {
        if ((app_regs.ValvesState >> valve_index) & (typeof(app_regs.ValvesState))(1))
            valve_drivers[valve_index].energize();
        else
            valve_drivers[valve_index].deenergize();
    }
    // Reply with the actual value that we wrote.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_valves_set(uint8_t reg_address)
{
    // Return the most recently set value.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_valves_set(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    for (size_t valve_index = 0; valve_index < NUM_VALVES; ++valve_index)
    {
        if ((app_regs.ValvesSet >> valve_index) & (typeof(app_regs.ValvesSet))(1))
            valve_drivers[valve_index].energize();
    }
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_valves_clear(uint8_t reg_address)
{
    // Return the most recently cleared value.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_valves_clear(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    for (size_t valve_index = 0; valve_index < NUM_VALVES; ++valve_index)
    {
        if ((app_regs.ValvesClear >> valve_index) & (typeof(app_regs.ValvesClear))(1))
            valve_drivers[valve_index].deenergize();
    }
    // Reply with the actual value that we wrote.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_any_valve_config(uint8_t reg_address)
{
    uint8_t valve_index = reg_address - VALVE_START_APP_ADDRESS;
    ValveConfig& valve_cfg = app_regs.ValveConfigs[valve_index];
    const ValveDriver& valve_driver = valve_drivers[valve_index];
    // Update Harp App registers with ValveDriver class contents.
    valve_cfg.hit_output = valve_driver.get_hit_output();
    valve_cfg.hold_output = valve_driver.get_hold_output();
    valve_cfg.hit_duration_us = valve_driver.get_hit_duration_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_any_valve_config(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    uint8_t valve_index = msg.header.address - VALVE_START_APP_ADDRESS;
    const ValveConfig& valve_cfg = app_regs.ValveConfigs[valve_index];
    ValveDriver& valve_driver = valve_drivers[valve_index];
    // Apply the configuration.
    valve_driver.set_hit_duration_us(valve_cfg.hit_duration_us);
    valve_driver.set_normalized_hit_output(valve_cfg.hit_output);
    valve_driver.set_normalized_hold_output(valve_cfg.hold_output);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

//void read_aux_gpio_dir(uint8_t reg_address)
//{
//    // Nothing to do!
//    // This register will stay consistent with the underlying peripheral
//    //  register after we initialize it the first time.
//    if (!HarpCore::is_muted())
//        HarpCore::send_harp_reply(READ, reg_address);
//}

void write_aux_gpio_dir(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Apply register settings (set bits are outputs; cleared bits are inputs).
    gpio_set_dir_masked(uint32_t(GPIOS_MASK) << GPIO_PIN_BASE,
                        uint32_t(app_regs.AuxGPIODir) << GPIO_PIN_BASE);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_aux_gpio_state(uint8_t reg_address)
{
    // Update register contents.
    app_regs.AuxGPIOState = read_aux_gpios();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_aux_gpio_state(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Note: only write to outputs
    gpio_put_masked(uint32_t(app_regs.AuxGPIODir) << GPIO_PIN_BASE,
                    uint32_t(app_regs.AuxGPIOState) << GPIO_PIN_BASE);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_aux_gpio_set(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Note: only write to outputs (ie: mask on Set bits).
    gpio_put_masked(
        uint32_t(app_regs.AuxGPIODir & app_regs.AuxGPIOSet) << GPIO_PIN_BASE,
        uint32_t(app_regs.AuxGPIOSet) << GPIO_PIN_BASE);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_aux_gpio_clear(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Note: only write to outputs (ie: mask on Clear bits).
    gpio_put_masked(
        uint32_t(app_regs.AuxGPIODir & app_regs.AuxGPIOClear) << GPIO_PIN_BASE,
        0);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void leak_state_alert()
{
    const uint8_t LEAK_STATE_INDEX_ADDRESS = 86; // FIXME: this is hardcoded.
    app_regs.LeakState = flow_detection.get_leak_state(); // Update leak state
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, LEAK_STATE_INDEX_ADDRESS, HarpCore::harp_time_us_64());
}

void manual_flow_meter_alert()
{
    const uint8_t MANUAL_FLOW_METER_INDEX_ADDRESS = 90; // FIXME: this is hardcoded.
    app_regs.ManualFlowMeterState = flow_detection.get_manual_flow_meter_state(); // Update manual flow meter state
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, MANUAL_FLOW_METER_INDEX_ADDRESS, HarpCore::harp_time_us_64());
}

void request_next_odor()
{
    const uint8_t NEXT_ODOR_INDEX_ADDRESS = 66; // FIXME: this is hardcoded.
    app_regs.QueuedOdorMask = 0; // Mark it as "used."
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, NEXT_ODOR_INDEX_ADDRESS, HarpCore::harp_time_us_64());
}

void poke_state_changed()
{
    const uint8_t POKE_STATE_INDEX_ADDRESS = 61; // FIXME: this is hardcoded.
    app_regs.PokeState = 1; // Mark it as "used."
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, POKE_STATE_INDEX_ADDRESS, HarpCore::harp_time_us_64());
}

void raw_poke_rise()
{
    const uint8_t POKE_STATE_INDEX_ADDRESS = 62; // FIXME: this is hardcoded.
    app_regs.RawPokeState = 1; // Mark it as "used."
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, POKE_STATE_INDEX_ADDRESS, HarpCore::harp_time_us_64());
}

void raw_poke_fall()
{
    const uint8_t POKE_STATE_INDEX_ADDRESS = 62; // FIXME: this is hardcoded.
    app_regs.RawPokeState = 0; // Mark it as "used."
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, POKE_STATE_INDEX_ADDRESS, HarpCore::harp_time_us_64());
}

void camera_timestamp_callback(uint gpio, uint32_t events) {
    
    // Only react to rising edges
    // if (!(events & GPIO_IRQ_EDGE_RISE)) return;

    uint8_t index;
    if (gpio == CAM0_TRIGGER_PIN) {
        index = CAM0_PIN_STATE_INDEX_ADDRESS;
    } else if (gpio == CAM1_TRIGGER_PIN) {
        index = CAM1_PIN_STATE_INDEX_ADDRESS;
    } else {
        // index = CAM0_PIN_STATE_INDEX_ADDRESS;
        return; // Not one of our camera pins
    }

    const uint64_t ts = HarpCore::harp_time_us_64();
    push_event_from_isr(index, ts);

}

// Delphi specific functions
#define QUEUE_SIZE 128 // Must be a power of 2 for the masking to work correctly.
#define QUEUE_MASK (QUEUE_SIZE - 1)
HarpEvent eventQueue[QUEUE_SIZE];
volatile uint8_t head = 0;
volatile uint8_t tail = 0;

void push_event_from_isr(uint8_t index, uint64_t timestamp) {
    uint8_t next = (head + 1) & QUEUE_MASK;
    if (next != tail) { // Prevent overflow
        eventQueue[head].index = index;
        eventQueue[head].timestamp = timestamp;
        head = next;
    }
}

bool pop_event(HarpEvent &event) {
    if (tail == head) return false; // Queue is empty
    event.index = eventQueue[tail].index;
    event.timestamp = eventQueue[tail].timestamp;
    tail = (tail + 1) & QUEUE_MASK;
    return true;
}

// Valve state mask
uint16_t get_valve_mask() {
    uint16_t mask = 0;  // Start with all bits cleared

    for (size_t valve_index = 0; valve_index < NUM_VALVES && valve_index < 16; ++valve_index) // limit considers num valves and bit mask size
    {
        if (valve_drivers[valve_index].is_energized())
        {
            // mask |= (uint16_t)(1) << valve_index;  // Set bit for this valve
            mask |= (1u << valve_index);
        }
    }
    return mask;
}

uint16_t previous_mask = 0; // Initialize to zero or read initial state

void update_app_state() // Called when app.run() is called -- add poke detection here
{
    // Update valve controller state machines.
    for (auto& valve_driver: valve_drivers)
        valve_driver.update();

    // Update poke manager FSM
    poke_manager.update();

    // Update Camera 0 Driver FSM 
    cam0_driver.update();

    // Update Camera 1 Driver FSM 
    cam1_driver.update();

    // Update Flow Detection
    flow_detection.update();

    // Handle valve state changes
    uint16_t current_mask = get_valve_mask();
    if (current_mask != previous_mask) {
        app_regs.ValvesState = current_mask;
        if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, VALVES_STATE_INDEX_ADDRESS, HarpCore::harp_time_us_64());
        previous_mask = current_mask;
    }

    // Do not update PID controllers if final valve is open, as flow will be zero and this may cause integrator windup. Instead, wait for the next update cycle after the valve closes to update the controllers with fresh flow readings.
    if (!(current_mask & (1u << FINAL_VALVE_INDEX)) || !freeze_pid_updates) {
        // Update PID controllers with latest flow meter readings.
        auto& adc_samples = flow_detection.get_latest_adc_sample();
        if (app_regs.ProportionalValve0Adc >= 0 ) {
            int ch0 = proportional_valve_0_controller.get_flow_adc_index();
            assert(ch0 >= -1 && ch0 < MAX_ADC_CHS);
            // proportional_valve_0_controller.update(adc_samples.v[ch0]);
            proportional_valve_0_controller.update(adc_samples.v[0]);
        }
        if (app_regs.ProportionalValve1Adc >= 0 ) {
            int ch1 = proportional_valve_1_controller.get_flow_adc_index();
            // assert(ch1 >= -1 && ch1 < MAX_ADC_CHS);
            // proportional_valve_1_controller.update(adc_samples.v[ch1]);
            proportional_valve_1_controller.update(adc_samples.v[1]);
        }
        if (app_regs.ProportionalValve2Adc >= 0 ) {
            int ch2 = proportional_valve_2_controller.get_flow_adc_index();
            // assert(ch2 >= -1 && ch2 < MAX_ADC_CHS);
            // proportional_valve_2_controller.update(adc_samples.v[ch2]);
            proportional_valve_2_controller.update(adc_samples.v[2]);
        }
    } 

    // Handle harp events
    HarpEvent evt;
    while (pop_event(evt)) {
        if (!HarpCore::is_muted()) {
            HarpCore::send_harp_reply(EVENT, evt.index, evt.timestamp);
        }
    }

    // Process AuxGPIO input changes.
    // FIXME: do we need to update old_aux_gpio_inputs if we change (write-to)
    uint8_t aux_gpio_inputs = read_aux_gpios() & ~app_regs.AuxGPIODir;
    uint8_t changed_inputs = (old_aux_gpio_inputs ^ aux_gpio_inputs);
    app_regs.AuxGPIORisingInputs = app_regs.AuxGPIOInputRiseEvent & aux_gpio_inputs & changed_inputs;
    app_regs.AuxGPIOFallingInputs = app_regs.AuxGPIOInputFallEvent & ~aux_gpio_inputs & changed_inputs;
    old_aux_gpio_inputs = aux_gpio_inputs;
    // Emit EVENT messages for rising/falling edges on configured pins.
    if (HarpCore::is_muted())
        return;
    if (app_regs.AuxGPIOInputRiseEvent & app_regs.AuxGPIORisingInputs)
        HarpCore::send_harp_reply(EVENT, AUX_GPIO_RISING_INPUTS_ADDRESS);
    if (app_regs.AuxGPIOInputFallEvent & app_regs.AuxGPIOFallingInputs)
        HarpCore::send_harp_reply(EVENT, AUX_GPIO_FALLING_INPUTS_ADDRESS);
}

void reset_app()
{
    // Reset poke manager and all poke-manager-related registers
    poke_manager.reset();
    poke_manager.set_next_odor_callback_fn(request_next_odor);
    poke_manager.set_poke_state_callback_fn(poke_state_changed);
    poke_manager.set_raw_poke_rise_callback_fn(raw_poke_rise);
    poke_manager.set_raw_poke_fall_callback_fn(raw_poke_fall);
    app_regs.PokeDometer = poke_manager.get_poke_count();
    app_regs.FSMEnabledState = poke_manager.get_enabled_state();
    app_regs.ForceFSM = 0;
    app_regs.QueuedOdorMask = poke_manager.get_current_odors();
    app_regs.OdorSetupTimeUS = poke_manager.get_odor_setup_time_us();
    app_regs.MinOdorDeliveryTimeUS = poke_manager.get_min_odor_delivery_time_us();
    app_regs.MaxOdorDeliveryTimeUS = poke_manager.get_max_odor_delivery_time_us();
    app_regs.MinimumPokeTimeUS = poke_manager.get_min_poke_time_us();
    app_regs.OdorDwellTimeUS = poke_manager.get_odor_dwell_time_us();

    //Reset cam driver and all related registers
    cam0_driver.reset();
    app_regs.Cam0PinState = cam0_driver.get_pwm_pin_state();
    app_regs.Cam0FrameRate = cam0_driver.get_pwm_freq();
    app_regs.Cam0DutyCycle = cam0_driver.get_pwm_duty();
    app_regs.EnableCam0Trigger = cam0_driver.get_enable_state();

    cam1_driver.reset();
    app_regs.Cam1PinState = cam1_driver.get_pwm_pin_state();
    app_regs.Cam1FrameRate = cam1_driver.get_pwm_freq();
    app_regs.Cam1DutyCycle = cam1_driver.get_pwm_duty();
    app_regs.EnableCam1Trigger = cam1_driver.get_enable_state();

    // Valve LED state
    gpio_init(LED_ENABLE_PIN);
    gpio_set_dir(LED_ENABLE_PIN, GPIO_OUT);
    gpio_put(LED_ENABLE_PIN, 0);

    // Reset Harp register struct elements.
    app_regs.ValvesState = 0;
    app_regs.ValvesSet = 0;
    app_regs.ValvesClear = 0;
    // Turn off all outputs.
    for (auto& valve_driver: valve_drivers)
        valve_driver.reset();

    // Init the exposed auxiliary GPIO pins we are using as 4-inputs.
    // This *must* be called once to setup the AUX GPIOs.
    gpio_init_mask(GPIOS_MASK << GPIO_PIN_BASE);
    gpio_set_dir_masked(GPIOS_MASK << GPIO_PIN_BASE, 0);

    app_regs.AuxGPIODir = 0b10111110; // GPIO pins 22 and 28 are inputs (poke and ADC)
    app_regs.AuxGPIOState = (gpio_get_all() >> GPIO_PIN_BASE) & GPIOS_MASK; //all pins are set low
    app_regs.AuxGPIOSet = 0;
    app_regs.AuxGPIOClear = 0;

    gpio_set_dir_masked(uint32_t(GPIOS_MASK) << GPIO_PIN_BASE,
                    uint32_t(app_regs.AuxGPIODir) << GPIO_PIN_BASE);

    // Clear aux input EVENT message configuration.
    app_regs.AuxGPIORisingInputs = 0;
    app_regs.AuxGPIOFallingInputs = 0;

    old_aux_gpio_inputs = read_aux_gpios() & ~app_regs.AuxGPIODir;

    // Reset flow detection and all related registers
    flow_detection.reset();
    flow_detection.leak_state_alert_callback_fn(leak_state_alert);
    flow_detection.manual_flow_meter_alert_callback_fn(manual_flow_meter_alert);
    app_regs.LatestFlowRate = flow_detection.get_latest_adc_sample();
    app_regs.LatestRawAdcSample = flow_detection.get_latest_raw_adc_sample();
    app_regs.EnableAdcSampling = flow_detection.get_adc_enabled_status();
    app_regs.LeakAdcChannel = flow_detection.get_leak_adc();
    app_regs.LeakThreshold = flow_detection.get_leak_threshold();
    app_regs.LeakState = flow_detection.get_leak_state();
    
    app_regs.NominalFlowRate = flow_detection.get_nominal_flow_rate();
    app_regs.FlowRateTolerance = flow_detection.get_flow_rate_tolerance();
    app_regs.ManualFlowMeterState = flow_detection.get_manual_flow_meter_state();
    app_regs.FlowMeterCalibrations = flow_detection.get_flow_meter_calibrations();

    // Reset proportional valve controllers and all related registers
    proportional_valve_0_controller.reset();
    proportional_valve_1_controller.reset();
    proportional_valve_2_controller.reset();

    app_regs.PidUpdateFrequency = proportional_valve_0_controller.get_pid_update_frequency();
    
    app_regs.ProportionalValve0Adc = proportional_valve_0_controller.get_flow_adc_index(); 
    app_regs.ProportionalValve1Adc = proportional_valve_1_controller.get_flow_adc_index(); 
    app_regs.ProportionalValve2Adc = proportional_valve_2_controller.get_flow_adc_index(); 

    app_regs.ProportionalValve0EnablePid = proportional_valve_0_controller.get_pid_enabled();
    app_regs.ProportionalValve0TargetFlowRate = proportional_valve_0_controller.get_target_flow_rate();
    app_regs.ProportionalValve0DutyCycle = proportional_valve_0_controller.get_duty_cycle();
    
    app_regs.ProportionalValve1EnablePid = proportional_valve_1_controller.get_pid_enabled();
    app_regs.ProportionalValve1TargetFlowRate = proportional_valve_1_controller.get_target_flow_rate();
    app_regs.ProportionalValve1DutyCycle = proportional_valve_1_controller.get_duty_cycle();
    
    app_regs.ProportionalValve2EnablePid = proportional_valve_2_controller.get_pid_enabled();
    app_regs.ProportionalValve2TargetFlowRate = proportional_valve_2_controller.get_target_flow_rate();
    app_regs.ProportionalValve2DutyCycle = proportional_valve_2_controller.get_duty_cycle();
    app_regs.FreezePidUpdates = freeze_pid_updates;

    // GPIO interrupt for camera timestamping
    cam0_driver.set_pio_pwm_pin(CAM0_TRIGGER_PIN);
    cam1_driver.set_pio_pwm_pin(CAM1_TRIGGER_PIN);
    gpio_set_irq_enabled_with_callback(CAM0_TRIGGER_PIN, GPIO_IRQ_EDGE_RISE, true, &camera_timestamp_callback);
    gpio_set_irq_enabled(CAM1_TRIGGER_PIN, GPIO_IRQ_EDGE_RISE, true); // Enable IRQ for cam1 using the same handler

}

