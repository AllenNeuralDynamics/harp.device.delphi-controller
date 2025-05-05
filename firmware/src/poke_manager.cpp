#include <poke_manager.h>

PokeManager::PokeManager(ValveDriver& final_valve, ValveDriver& vac_valve, etl::vector<ValveDriver, NUM_ODOR_VALVES>& odor_valves)
: state_{RESET}, poke_count_{0}, poke_pin_{DEFAUT_POKE_PIN}, odor_valve_index_{0}, next_odor_index_{0}, disable_fsm_{false}, poke_detected_{false}, final_valve_{final_valve}, vac_valve_{vac_valve}, odor_valves_{odor_valves}, beam_broken_{false}, poke_initiated_once_{false} 
{
    // Nothing else to do!
}

PokeManager::~PokeManager() //destuctor
{
    //Deengergize all valves
    deenergize_all_valves();
    poke_count_ = 0;
    poke_detected_ = false;
    disable_fsm_ = false;
    state_ = RESET;
    beam_broken_ = false;
    poke_initiated_once_ = false;
}

void PokeManager::deenergize_all_valves()
{
    final_valve_.deenergize();
    vac_valve_.deenergize();
    for (int i = 0; i < NUM_ODOR_VALVES; i++){
        odor_valves_[i].deenergize();
    }
}

// Functions to configure Delphi Task/update it
void PokeManager::set_vacuum_close_time_us(uint32_t vacuum_close_time_us)
{
    vacuum_close_time_us_ = vacuum_close_time_us;
}

void PokeManager::set_odor_delivery_time_us(uint32_t odor_delivery_time_us)
{
    odor_delivery_time_us_ = odor_delivery_time_us;
}

void PokeManager::set_odor_transition_time_us(uint32_t odor_transition_time_us)
{
    odor_transition_time_ = odor_transition_time_us;
}

void PokeManager::set_vac_setup_time_uss(uint32_t vac_setup_time_us)
{
    vac_setup_time_us_ = vac_setup_time_us;
}

void PokeManager::set_final_valve_energized_time_us(uint32_t final_valve_energized_time_us)
{
    final_valve_energized_time_us_ = final_valve_energized_time_us;
}

void PokeManager::set_min_poke_time_us(uint32_t min_poke_time_us)
{
    min_poke_time_us_ = min_poke_time_us;
}

void PokeManager::set_poke_pin(uint8_t pin)
{
    poke_pin_ = pin;
}

//Poke detection function
void PokeManager::check_poke_status()
{
    // Check to see if poke has been detected
    // Beam is no longer broken
    if (gpio_get(poke_pin_) == 1) // specify poke pin
    {
        beam_broken_ == false;
        poke_start_time_us_ = time_us_32();
        poke_initiated_once_ = false;
    }

    // Poke detected -- start poke timer
    if (gpio_get(poke_pin_) == 0 && beam_broken_ == false)
    {
        poke_start_time_us_ = time_us_32();
        beam_broken_ = true;
    }
        
    // Check duration since beam break/poke
    if (gpio_get(poke_pin_) == 0 && beam_broken_ == true)
    {
        gpio_put(LED_PIN, 1); // Turn on LED whenever the beam is broken
        if ((time_us_32() - poke_start_time_us_) >= min_poke_time_us_ && poke_initiated_once_ == false){
            
            //Poke was detected!
            poke();

            //Account for the successful poke so that another doesn't occur on the same poke 
            poke_initiated_once_ = true;
        }   
    }
}

// Functions to alter the FSM
void PokeManager::reset()
{
    deenergize_all_valves();
    odor_valve_index_ = next_odor_index_;
    poke_count_ = 0;
    poke_detected_ = false;
    disable_fsm_ = false;
    state_ = RESET;
    beam_broken_ = false;
    poke_initiated_once_ = false;
    set_vacuum_close_time_us(DEFAULT_VACUUM_CLOSE_TIME_US);
    set_odor_delivery_time_us(DEFAULT_ODOR_DELIVERY_TIME_US);
    set_odor_transition_time_us(DEFAULT_ODOR_TRANSITION_TIME_US);
    set_vac_setup_time_uss(DEFAULT_VAC_SETUP_TIME_US);
    set_final_valve_energized_time_us(DEFAULT_FINAL_VALVE_ENERGIZED_TIME_US);
}

void PokeManager::pause() // Needed for odor changes/refills
{
    disable_fsm_ = true;
    deenergize_all_valves(); // deenergize all valves
}

void PokeManager::restart() // Needed for odor changes/refills
{
    disable_fsm_ = false;
    poke_detected_ = false;
    state_ = RESET;
    beam_broken_ = false;
    poke_initiated_once_ = false;
}

void PokeManager::update_next_odor(int next_odor) // Update the index of the next odor
{
    next_odor_index_ = next_odor;
}

void PokeManager::update()
{
    //enabled by default, but if disabled, pause the FSM
    if (disable_fsm_ == false)
    {
        //initialize RESET state
        if (state_ == RESET) //need to query state_ for initital 
        {
            // state_entry_time_us_ = time_us_32();
            deenergize_all_valves();
        }

        // check for poke
        check_poke_status();

        state_t next_state{state_}; // initialize next-state to current state.

        // Handling next-state logic.
        switch (state_)
        {
            case RESET:
                next_state = ODOR_SETUP;
                break;
            case ODOR_SETUP:
                if (state_duration_us() >= vacuum_close_time_us_)
                    next_state = ODOR_DISPENSING_TO_EXHAUST;
                break;
            case ODOR_DISPENSING_TO_EXHAUST:
                if (poke_detected_)
                    next_state = ODOR_DELIVERY_TO_FINAL_VALVE;
                break;
            case ODOR_DELIVERY_TO_FINAL_VALVE:
                if (state_duration_us() >= odor_delivery_time_us_)
                    next_state = ODOR_PRECLEAN;
                break;
            case ODOR_PRECLEAN:
                if (state_duration_us() >= odor_transition_time_)
                    next_state = VAC_START;
                break;
            case VAC_START:
                if (state_duration_us() >= vac_setup_time_us_)
                    next_state = ODOR_PURGE;
                break;
            case ODOR_PURGE:
                if (state_duration_us() >= final_valve_energized_time_us_)
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
                odor_valves_[odor_valve_index_].energize();
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
                odor_valves_[odor_valve_index_].deenergize();
                vac_valve_.energize();
            }

            if (next_state == ODOR_PURGE)
            {
                // Energize the final valve
                final_valve_.energize();
                odor_valve_index_ = next_odor_index_; //DO SOMETHING HERE TO READ FROM THE REGISTER TO GET NEXT ODOR
                // ++odor_valve_index_;
                // if (odor_valve_index_ == NUM_ODOR_VALVES) // test logic for iterating through valves
                //     odor_valve_index_ = 0;

                printf("Odor Valve: %i\r\n", odor_valve_index_); //valve odor index
            }
        }
        // Update state:
        state_ = next_state;

    }
}
