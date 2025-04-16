#include <poke_manager.h>

PokeManager::PokeManager()
: state_{RESET}, poke_count_{0}, poke_detected_{false}
{
    // Nothing else to do!
}


PokeManager::~PokeManager()
{
    // TODO: implement this!
}

void PokeManager::update()
{
    // Update inputs.
    // TODO: poke detection here.
    // printf("Updating\r\n");

    state_t next_state{state_}; // initialize next-state to current state.

    // Handling next-state logic.
    switch (state_)
    {
        case RESET:
            next_state = ODOR_SETUP;
            break;
        case ODOR_SETUP:
            if (state_duration_us() >= VACUUM_CLOSE_TIME_US)
                next_state = ODOR_DISPENSING_TO_EXHAUST;
            break;
        case ODOR_DISPENSING_TO_EXHAUST:
            if (poke_detected_)
                next_state = ODOR_DELIVERY_TO_FINAL_VALVE;
            break;
        case ODOR_DELIVERY_TO_FINAL_VALVE:
            if (state_duration_us() >= ODOR_DELIVERY_TIME_US)
                next_state = ODOR_PRECLEAN;
            break;
        case ODOR_PRECLEAN:
            if (state_duration_us() >= ODOR_TRANSITION_TIME_US)
                next_state = VAC_START;
            break;
        case VAC_START:
            if (state_duration_us() >= VAC_SETUP_TIME_US)
                next_state = ODOR_PURGE;
            break;
        case ODOR_PURGE:
            if (state_duration_us() >= FINAL_VALVE_ENERGIZED_TIME_US)
                next_state = ODOR_PURGE;
            break;
        default:
            break;
    }

    // Update how long we've been in the new state.
    if (state_ != next_state)
    {
        state_entry_time_us_ = time_us_32();
        printf("State transition %d -> %d\r\n", state_, next_state);
    }

    if (next_state == RESET)
    {
        // deenergize_all_valves();
        state_entry_time_us_ = time_us_32(); // Force this to happen at reset.
    }
    if (next_state == ODOR_DELIVERY_TO_FINAL_VALVE)
    {
        ++poke_count_;
        poke_detected_ = false;
    }


    // Handling state-and/or-input-depenendent output logic.
    if (next_state == ODOR_PRECLEAN)
    {
        // deenergize_valve(??);
        // more stuff here.
    }


    // Update state:
    state_ = next_state;
}
