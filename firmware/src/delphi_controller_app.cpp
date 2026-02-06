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
    {(uint8_t*)&app_regs.CamPin, sizeof(app_regs.CamPin), U8},
    {(uint8_t*)&app_regs.CamPinState, sizeof(app_regs.CamPinState), U8},
    {(uint8_t*)&app_regs.FrameRate, sizeof(app_regs.FrameRate), U32},
    {(uint8_t*)&app_regs.DutyCycle, sizeof(app_regs.DutyCycle), Float},
    {(uint8_t*)&app_regs.EnableCamTrigger, sizeof(app_regs.EnableCamTrigger), U8},
    {(uint8_t*)&app_regs.EnableValveLeds, sizeof(app_regs.EnableValveLeds), U8}
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
    {read_cam_pin, write_cam_pin}, //Start here
    {read_cam_pin_state, HarpCore::write_to_read_only_reg_error},
    {read_frame_rate, write_frame_rate},
    {read_duty_cycle, write_duty_cycle},
    {read_enable_cam_trigger, write_enable_cam_trigger},
    {read_valve_leds, write_valve_leds}
};

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

void read_enable_cam_trigger(uint8_t reg_address)
{
    app_regs.EnableCamTrigger = cam_driver.get_enable_state();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_enable_cam_trigger(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam_driver.set_enable_state(app_regs.EnableCamTrigger);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_duty_cycle(uint8_t reg_address)
{
    app_regs.DutyCycle = cam_driver.get_pwm_duty();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_duty_cycle(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam_driver.set_pwm_duty_cycle(app_regs.DutyCycle);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_frame_rate(uint8_t reg_address)
{
    app_regs.FrameRate = cam_driver.get_pwm_freq();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_frame_rate(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam_driver.set_pwm_freq(app_regs.FrameRate);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_cam_pin(uint8_t reg_address)
{
    app_regs.CamPin = cam_driver.get_pio_pwm_pin();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_cam_pin(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    cam_driver.set_pio_pwm_pin(app_regs.CamPin); //disable previous camera pin 
    gpio_set_irq_enabled_with_callback(app_regs.CamPin, GPIO_IRQ_EDGE_RISE, true, &camera_timestamp_callback);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_cam_pin_state(uint8_t reg_address)
{
    app_regs.CamPinState = cam_driver.get_pwm_pin_state(); 
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
    poke_manager.force_poke(); // FIXME: is this the correct way to force the fsm?
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
    // // Toggle LED for testing
    // gpio_put(25, !gpio_get(25));
    push_event_from_isr(CAM_PIN_STATE_INDEX_ADDRESS, HarpCore::harp_time_us_64());
}

// Delphi specific functions
#define QUEUE_SIZE 128
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

    // Update Camera Driver FSM 
    cam_driver.update();

    // Handle harp events
    HarpEvent evt;
    while (pop_event(evt)) {
        if (!HarpCore::is_muted()) {
            HarpCore::send_harp_reply(EVENT, evt.index, evt.timestamp);
        }
    }

    // Handle valve state changes
    uint16_t current_mask = get_valve_mask();
    if (current_mask != previous_mask) {
        app_regs.ValvesState = current_mask;
        if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, VALVES_STATE_INDEX_ADDRESS, HarpCore::harp_time_us_64());
        previous_mask = current_mask;
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

    //Reset cam driver and all related registers
    cam_driver.reset();
    // cam_driver.set_pwm_rise_callback_fn(rising_edge_detected); //USED FOR POOLING EVENTS
    // cam_driver.set_pwm_fall_callback_fn(falling_edge_detected); // USED FOR POOLING EVENTS
    app_regs.CamPinState = cam_driver.get_pwm_pin_state();
    app_regs.FrameRate = cam_driver.get_pwm_freq();
    app_regs.DutyCycle = cam_driver.get_pwm_duty();
    app_regs.EnableCamTrigger = cam_driver.get_enable_state();

    // FOR TESTING -- LED blinking
    // gpio_init(LED_ENABLE_PIN);
    // gpio_set_dir(LED_ENABLE_PIN, GPIO_OUT);

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

    app_regs.AuxGPIODir = 0b00001110; // GPIO pins 23-25 as outputs
    app_regs.AuxGPIOState = (gpio_get_all() >> GPIO_PIN_BASE) & GPIOS_MASK; //all pins are set low
    app_regs.AuxGPIOSet = 0;
    app_regs.AuxGPIOClear = 0;

    gpio_set_dir_masked(uint32_t(GPIOS_MASK) << GPIO_PIN_BASE,
                    uint32_t(app_regs.AuxGPIODir) << GPIO_PIN_BASE);

    // Clear aux input EVENT message configuration.
    app_regs.AuxGPIORisingInputs = 0;
    app_regs.AuxGPIOFallingInputs = 0;

    old_aux_gpio_inputs = read_aux_gpios() & ~app_regs.AuxGPIODir;

    // gpio_set_irq_enabled_with_callback(26, GPIO_IRQ_EDGE_RISE, true, &camera_timestamp_callback);

}

