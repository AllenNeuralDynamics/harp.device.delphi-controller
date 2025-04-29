#include <poke_manager.h>

PokeManager::PokeManager(ValveDriver& final_valve, ValveDriver& vac_valve, etl::vector<ValveDriver, NUM_ODOR_VALVES>& odor_valves)
: state_{RESET}, poke_count_{0}, valve_index_{0}, poke_detected_{false}, final_valve_{final_valve}, vac_valve_{vac_valve}, odor_valves_{odor_valves}
{
    // Nothing else to do!
}

PokeManager::~PokeManager()
{
    // TODO: implement this!
}

void PokeManager::deenergize_all_valves()
{
    final_valve_.deenergize();
    vac_valve_.deenergize();
    for (int i = 0; i < NUM_ODOR_VALVES; i++){
        odor_valves_[i].deenergize();
    }
}

void PokeManager::update()
{
    //initialize RESET state
    if (state_ == RESET) //need to query state_ for initital 
    {
        // state_entry_time_us_ = time_us_32();
        deenergize_all_valves();
    }

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
                next_state = ODOR_SETUP;
            break;
        default:
            break;
    }

    // Update how long we've been in the new state.
    if (state_ != next_state)
    {
        printf("State transition %d -> %d\r\n", state_, next_state);
        printf("State transition time %i\r\n", state_duration_us());
        state_entry_time_us_ = time_us_32();
    
        // Next state logic should only be assessed if there is a state transition
        if (next_state == ODOR_SETUP)
        {
            // Energize one of the odor valves
            deenergize_all_valves();
            odor_valves_[valve_index_].energize();
        }

        if (next_state == ODOR_DISPENSING_TO_EXHAUST)
        {
            // Don't need to do anything because odor is being sent to exhaust and we are waiting for a poke
        }

        if (next_state == ODOR_DELIVERY_TO_FINAL_VALVE)
        {
            final_valve_.energize();
            ++poke_count_;
            printf("Number of pokes = %i\r\n", poke_count_);
            poke_detected_ = false;
        }

        if (next_state == ODOR_PRECLEAN)
        {
            final_valve_.deenergize();
        }

        if (next_state == VAC_START)
        {
            // Deenergize odor valve
            odor_valves_[valve_index_].deenergize();
            vac_valve_.energize();
        }

        if (next_state == ODOR_PURGE)
        {
            // Energize the final valve
            final_valve_.energize();
            ++valve_index_;
            if (valve_index_ == NUM_ODOR_VALVES) // test logic for iterating through valves
                valve_index_ = 0;

            printf("Odor Valve: %i\r\n", valve_index_); //valve odor index
        }
    }
    // Update state:
    state_ = next_state;
}
