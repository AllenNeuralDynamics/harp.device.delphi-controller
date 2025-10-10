#include <poke_manager.h>

PokeManager::PokeManager(ValveDriver& final_valve, ValveDriver& vac_valve,
                         ValveDriver (&odor_valves)[], size_t num_odor_valves)
: final_valve_{final_valve}, vac_valve_{vac_valve}, odor_valves_{odor_valves},
num_odor_valves_{num_odor_valves},
state_{RESET}, poke_count_{0}, poke_pin_{DEFAUT_POKE_PIN},
odor_valve_index_{-1}, disable_fsm_{false},
poke_detected_{false}, poke_state_{0}, raw_poke_state_{0},
beam_broken_{false}, poke_initiated_once_{false},
request_next_odor_callback_fn_{nullptr}, request_poke_state_callback_fn_{nullptr},
request_raw_poke_rise_callback_fn_{nullptr}, request_raw_poke_fall_callback_fn_{nullptr},
poke_pin_is_initialized_{false}, valve_state_{false}
{
    reset(); // set timing constants to defaults.
}

PokeManager::~PokeManager() //destuctor
{
    //Deengergize all valves
    deenergize_all_valves();
    poke_count_ = 0;
    poke_state_ = 0;
    raw_poke_state_ = 0;
    poke_detected_ = false;
    disable_fsm_ = false;
    state_ = RESET;
    beam_broken_ = false;
    poke_initiated_once_ = false;
    valve_state_ = false;
}

void PokeManager::deenergize_all_valves()
{
    final_valve_.deenergize();
    vac_valve_.deenergize();
    for (int i = 0; i < num_odor_valves_; ++i)
        odor_valves_[i].deenergize();
}

//Poke detection function
void PokeManager::update_poke_status()
{
    // Check to see if poke has been detected
    // Beam is no longer broken
    if (!gpio_get(poke_pin_))
    {
        beam_broken_ == false;
        poke_start_time_us_ = time_us_32();
        poke_initiated_once_ = false;

        if (raw_poke_state_ == 1)
        {
            //falling edge event
            raw_poke_fall();
        }
        raw_poke_state_ = 0;  
    }

    // Beam broken -- update raw poke state
    if (gpio_get(poke_pin_))
    {
        if (raw_poke_state_ == 0)
        {
            //rising edge event
            raw_poke_rise();
        }
        raw_poke_state_ = 1;  
    }

    // Poke detected -- start poke timer
    if (gpio_get(poke_pin_) && !beam_broken_)
    {
        poke_start_time_us_ = time_us_32();
        beam_broken_ = true;
    }

    // Check duration since beam break/poke
    if (gpio_get(poke_pin_) && beam_broken_)
    {
        //gpio_put(LED_PIN, 1); // Turn on LED whenever the beam is broken
        if ((time_us_32() - poke_start_time_us_) >= min_poke_time_us_ && poke_initiated_once_ == false)
        {
            //Poke was detected!
            poke();
            poke_state_changed();
            poke_state_ = 1;
#if(DEBUG)
        printf("Poke detected!\r\n");
#endif
            //Account for the successful poke so that another doesn't occur on the same poke
            poke_initiated_once_ = true;
        }
    }
}

// Functions to alter the FSM
void PokeManager::reset()
{
    state_ = RESET;
    deenergize_all_valves();
    disable();
    odor_valve_index_ = -1;
    poke_count_ = 0;
    poke_state_ = 0;
    poke_detected_ = false;
    beam_broken_ = false;
    poke_initiated_once_ = false;
    valve_state_ = false;
    clear_poke_pin();
    request_next_odor_callback_fn_ = nullptr;
    request_poke_state_callback_fn_ = nullptr;
    request_raw_poke_rise_callback_fn_ = nullptr;
    request_raw_poke_fall_callback_fn_ = nullptr;
    set_vacuum_close_time_us(DEFAULT_VACUUM_CLOSE_TIME_US);
    set_min_odor_delivery_time_us(DEFAULT_MIN_ODOR_DELIVERY_TIME_US);
    set_max_odor_delivery_time_us(DEFAULT_MAX_ODOR_DELIVERY_TIME_US);
    set_odor_transition_time_us(DEFAULT_ODOR_TRANSITION_TIME_US);
    set_vacuum_setup_time_us(DEFAULT_VACUUM_SETUP_TIME_US);
    set_final_valve_energized_time_us(DEFAULT_FINAL_VALVE_ENERGIZED_TIME_US);
}

void PokeManager::set_enabled_state(bool enabled)
{
    if (enabled)
        disable_fsm_ = false;
    else
    {
        disable_fsm_ = true;
        deenergize_all_valves(); // deenergize all valves
        // Clear internal state machine variables.
        state_ = RESET;
        poke_detected_ = false;
        poke_state_ = 0;
        beam_broken_ = false;
        poke_initiated_once_ = false;
        valve_state_ = false;
    }
}

void PokeManager::update()
{
    //enabled by default, but if disabled, bail early
    if (disable_fsm_)
        return;

    // check for poke
    update_poke_status();

    state_t next_state{state_}; // initialize next-state to current state.

    // Handling next-state logic.
    switch (state_)
    {
        case RESET:
            //initialize RESET state by turning off all valves
            deenergize_all_valves();
            next_state = ODOR_SETUP;
            break;
        case ODOR_SETUP:
            if (odor_valve_index_ < 0){
                // The odor should be primed before a poke
                poke_detected_ = false;
                poke_state_ = 0; 
            }
            
            else if (odor_valve_index_ != -1 && !valve_state_){
                energize_odor_valve(); // Need to initiated the event that the odor was consumed before this
            }
            else if (state_duration_us() >= vacuum_close_time_us_  && odor_valve_index_ != -1)
            {
                next_state = ODOR_DISPENSING_TO_EXHAUST;
            }
            break;
        case ODOR_DISPENSING_TO_EXHAUST:
            if (poke_detected_) // poke detected and an odor is primed
            {
                next_state = ODOR_DELIVERY_TO_FINAL_VALVE;
            }
            break;
        case ODOR_DELIVERY_TO_FINAL_VALVE:
            if ((state_duration_us() >= min_odor_delivery_time_us_ && !poke_initiated_once_) || state_duration_us() >= max_odor_delivery_time_us_) //adjust to determine if the beam is still broken after the poke (up to max)
                next_state = ODOR_PRECLEAN;
            break;
        case ODOR_PRECLEAN:
            if (state_duration_us() >= odor_transition_time_us_)
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
#if(DEBUG)
        printf("State transition %d -> %d\r\n", state_, next_state);
        printf("State transition time %i\r\n", state_duration_us());
#endif
        state_entry_time_us_ = time_us_32();

        // Next state logic should only be assessed if there is a state transition
        if (next_state == ODOR_SETUP)
        {
            // explicitly grab odor in the queue
            deenergize_all_valves();
        }

        // Don't need to do anything because odor is being sent to exhaust and
        // we are waiting for a poke
        if (next_state == ODOR_DISPENSING_TO_EXHAUST){}

        if (next_state == ODOR_DELIVERY_TO_FINAL_VALVE)
        {
            final_valve_.energize();
            ++poke_count_;
#if(DEBUG)
            printf("Number of pokes = %i\r\n", poke_count_);
#endif
            poke_detected_ = false;
            poke_state_ = 0;
        }

        if (next_state == ODOR_PRECLEAN)
        {
            final_valve_.deenergize();
        }

        if (next_state == VAC_START)
        {
            deenergize_odor_valve();
            vac_valve_.energize();
        }

        if (next_state == ODOR_PURGE)
        {
            // Energize the final valve
            final_valve_.energize();
            odor_valve_index_ = -1; // Consume queued odor.
            request_next_odor(); //request
            
#if(DEBUG)
            printf("Odor Valve: %i\r\n", odor_valve_index_); //valve odor index
#endif
        }
    }
    // Update state:
    state_ = next_state;

    
}
