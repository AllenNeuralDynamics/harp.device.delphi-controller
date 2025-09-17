#include <delphi_controller_app.h>

app_regs_t app_regs;
uint8_t old_aux_gpio_inputs;


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
    {(uint8_t*)&app_regs.PokeDometer, sizeof(app_regs.PokeDometer), U32},
    {(uint8_t*)&app_regs.FSMEnabledState, sizeof(app_regs.FSMEnabledState), U8},
    {(uint8_t*)&app_regs.ForceFSM, sizeof(app_regs.ForceFSM), U8},
    {(uint8_t*)&app_regs.QueuedOdorIndex, sizeof(app_regs.QueuedOdorIndex), S8},
    {(uint8_t*)&app_regs.VacuumCloseTimeUS, sizeof(app_regs.VacuumCloseTimeUS), U32},
    {(uint8_t*)&app_regs.MinOdorDeliveryTimeUS, sizeof(app_regs.MinOdorDeliveryTimeUS), U32},
    {(uint8_t*)&app_regs.MaxOdorDeliveryTimeUS, sizeof(app_regs.MaxOdorDeliveryTimeUS), U32},
    {(uint8_t*)&app_regs.VacuumSetupTimeUS, sizeof(app_regs.VacuumSetupTimeUS), U32},
    {(uint8_t*)&app_regs.FinalValveEnergizedTimeUS, sizeof(app_regs.FinalValveEnergizedTimeUS), U32},
    {(uint8_t*)&app_regs.MinimumPokeTimeUS, sizeof(app_regs.MinimumPokeTimeUS), U32},
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
    {read_pokedometer, HarpCore::write_to_read_only_reg_error},
    {read_fsm_enabled_state, write_fsm_enabled_state},
    {read_force_fsm, write_force_fsm},
    {read_current_odor, write_current_odor},
    {read_vacuum_close_time_us, write_vacuum_close_time_us},
    {read_min_odor_delivery_time_us, write_min_odor_delivery_time_us},
    {read_max_odor_delivery_time_us, write_max_odor_delivery_time_us},
    {read_odor_transition_time_us, write_odor_transition_time_us},
    {read_vacuum_setup_time_us, write_vacuum_setup_time_us},
    {read_final_valve_energized_time_us, write_final_valve_energized_time_us},
    {read_minimum_poke_time_us, write_minimum_poke_time_us}
};


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
    app_regs.PokeState = poke_manager.get_poke_state(); // Doesn't exist.
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

void read_current_odor(uint8_t reg_address)
{
    // Get recent poke count value
    app_regs.QueuedOdorIndex = poke_manager.get_current_odor();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_current_odor(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_current_odor(app_regs.QueuedOdorIndex);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_vacuum_close_time_us(uint8_t reg_address)
{
    app_regs.VacuumCloseTimeUS = poke_manager.get_vacuum_close_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_vacuum_close_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_vacuum_close_time_us(app_regs.VacuumCloseTimeUS);
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

void read_odor_transition_time_us(uint8_t reg_address)
{
    app_regs.OdorTransitionTimeUS = poke_manager.get_odor_transition_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_odor_transition_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_odor_transition_time_us(app_regs.OdorTransitionTimeUS);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_vacuum_setup_time_us(uint8_t reg_address)
{
    app_regs.VacuumSetupTimeUS = poke_manager.get_vacuum_setup_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_vacuum_setup_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_vacuum_setup_time_us(app_regs.VacuumSetupTimeUS);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_final_valve_energized_time_us(uint8_t reg_address)
{
    app_regs.FinalValveEnergizedTimeUS =
        poke_manager.get_final_valve_energized_time_us();
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_final_valve_energized_time_us(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    poke_manager.set_final_valve_energized_time_us(app_regs.FinalValveEnergizedTimeUS);
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
    for (size_t valve_index = 0; valve_index < NUM_VALVES; ++valve_index)
    {
        app_regs.ValvesState = 0;
        if (valve_drivers[valve_index].is_energized())
            app_regs.ValvesState |= (typeof(app_regs.ValvesState))(1) << valve_index;
    }
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
    const uint8_t NEXT_ODOR_INDEX_ADDRESS = 65; // FIXME: this is hardcoded.
    app_regs.QueuedOdorIndex = -1; // Mark it as "used."
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, NEXT_ODOR_INDEX_ADDRESS);
}

void poke_state_changed()
{
    const uint8_t POKE_STATE_INDEX_ADDRESS = 61; // FIXME: this is hardcoded.
    app_regs.PokeState = 1; // Mark it as "used."
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(EVENT, POKE_STATE_INDEX_ADDRESS);
}

void update_app_state() // Called when app.run() is called -- add poke detection here
{
    // Update valve controller state machines.
    for (auto& valve_driver: valve_drivers)
        valve_driver.update();

    // Update poke manager FSM
    poke_manager.update();
    // TODO: Issue poke-related state changes as events.

    // Process AuxGPIO input changes.
    // FIXME: do we need to update old_aux_gpio_inputs if we change (write-to)
    //  app_regs.AuxGPIODir ?
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
    app_regs.PokeDometer = poke_manager.get_poke_count();
    app_regs.FSMEnabledState = poke_manager.get_enabled_state();
    app_regs.ForceFSM = 0;
    app_regs.QueuedOdorIndex = poke_manager.get_current_odor();
    app_regs.VacuumCloseTimeUS = poke_manager.get_vacuum_close_time_us();
    app_regs.MinOdorDeliveryTimeUS = poke_manager.get_min_odor_delivery_time_us();
    app_regs.MaxOdorDeliveryTimeUS = poke_manager.get_max_odor_delivery_time_us();
    app_regs.OdorTransitionTimeUS = poke_manager.get_odor_transition_time_us();
    app_regs.VacuumSetupTimeUS = poke_manager.get_vacuum_setup_time_us();
    app_regs.FinalValveEnergizedTimeUS = poke_manager.get_final_valve_energized_time_us();
    app_regs.MinimumPokeTimeUS = poke_manager.get_min_poke_time_us();

    // Reset Harp register struct elements.
    app_regs.ValvesState = 0;
    app_regs.ValvesSet = 0;
    app_regs.ValvesClear = 0;
    // Turn off all outputs.
    for (auto& valve_driver: valve_drivers)
        valve_driver.reset();

    // Init the exposed auxiliary GPIO pins we are using as all-inputs.
    // This *must* be called once to setup the AUX GPIOs.
    gpio_init_mask(GPIOS_MASK << GPIO_PIN_BASE);
    gpio_set_dir_masked(GPIOS_MASK << GPIO_PIN_BASE, 0);

    app_regs.AuxGPIODir = 0; // All inputs (consistent with what we just set).
    app_regs.AuxGPIOState = (gpio_get_all() >> GPIO_PIN_BASE) & GPIOS_MASK;
    app_regs.AuxGPIOSet = 0;
    app_regs.AuxGPIOClear = 0;

    // Clear aux input EVENT message configuration.
    app_regs.AuxGPIORisingInputs = 0;
    app_regs.AuxGPIOFallingInputs = 0;

    old_aux_gpio_inputs = read_aux_gpios() & ~app_regs.AuxGPIODir;


}

