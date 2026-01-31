#include <poke_manager.h>

PokeManager::PokeManager(ValveDriver& final_valve, ValveDriver (&odor_valves)[], size_t num_odor_valves)
: final_valve_{final_valve}, odor_valves_{odor_valves},
num_odor_valves_{num_odor_valves},
state_{RESET}, poke_count_{0}, poke_pin_{DEFAUT_POKE_PIN},
odor_valve_mask_{0}, disable_fsm_{false},
poke_detected_{false}, poke_state_{0}, raw_poke_state_{0},
beam_broken_{false}, poke_initiated_once_{false},
request_next_odor_callback_fn_{nullptr}, request_poke_state_callback_fn_{nullptr},
request_raw_poke_rise_callback_fn_{nullptr}, request_raw_poke_fall_callback_fn_{nullptr},
poke_pin_is_initialized_{false}, valve_state_{false}, block_poke_detection_{false}
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
    block_poke_detection_ = false;
}

void PokeManager::deenergize_all_valves()
{
    final_valve_.deenergize();
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
        poke_start_time_us_ = time_us_32();
        if (raw_poke_state_ == 1)
        {
            //falling edge event
            beam_broken_ == false;
            poke_initiated_once_ = false;
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
            beam_broken_ = true;
            if (!block_poke_detection_)
            {
                poke_start_time_us_ = time_us_32();
            }
            else 
            {
                beam_broken_ = false;  //prevent a poke from being initiated during the state machine
            }
            raw_poke_rise();
        }
        raw_poke_state_ = 1;  
    }

    // Check duration since beam break/poke
    if (beam_broken_) // Prevent pokes from being registered until correct state
    {
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
    odor_valve_mask_ = 0;
    poke_count_ = 0;
    poke_state_ = 0;
    poke_detected_ = false;
    beam_broken_ = false;
    block_poke_detection_ = false;
    poke_initiated_once_ = false;
    valve_state_ = false;
    clear_poke_pin();
    request_next_odor_callback_fn_ = nullptr;
    request_poke_state_callback_fn_ = nullptr;
    request_raw_poke_rise_callback_fn_ = nullptr;
    request_raw_poke_fall_callback_fn_ = nullptr;
    set_odor_setup_time_us(DEFAULT_ODOR_SETUP_TIME_US);
    set_min_odor_delivery_time_us(DEFAULT_MIN_ODOR_DELIVERY_TIME_US);
    set_max_odor_delivery_time_us(DEFAULT_MAX_ODOR_DELIVERY_TIME_US);
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
        block_poke_detection_ = false;
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
            if (odor_valve_mask_ == 0){
                // The odor should be primed before a poke
                poke_detected_ = false;
                poke_state_ = 0; 
            }   
            else if (state_duration_us() >= odor_setup_time_us_  && odor_valve_mask_ != 0)
            {
                energize_odor_valve();
                next_state = ODOR_READY_FOR_POKE;
            }
            break;
        case ODOR_READY_FOR_POKE:
            if (poke_detected_) // poke detected and an odor is primed
            {
                next_state = ODOR_DELIVERY_TO_FINAL_VALVE;
            }
            break;
        case ODOR_DELIVERY_TO_FINAL_VALVE:
            if ((state_duration_us() >= min_odor_delivery_time_us_ && !poke_initiated_once_) || state_duration_us() >= max_odor_delivery_time_us_)
            {
                //adjust to determine if the beam is still broken after the poke (up to max)
                // Clear current odor and request next odor
                deenergize_all_valves();
                odor_valve_mask_ = 0; // Clear the mask
                request_next_odor(); //request
                next_state = ODOR_SETUP;
            }
            break;
        default:
            break;
    }

    // Update how long we've been in the new state.
    if (state_ != next_state)
    {
        state_entry_time_us_ = time_us_32();

        // Next state logic should only be assessed if there is a state transition
        // Wait for poke
        if (next_state == ODOR_READY_FOR_POKE)
        {
            block_poke_detection_ = false;
        }

        if (next_state == ODOR_DELIVERY_TO_FINAL_VALVE)
        {
            final_valve_.energize();
            ++poke_count_;
            poke_detected_ = false;
            block_poke_detection_ = true;
            poke_state_ = 0;
        }
    }
    // Update state:
    state_ = next_state;
}
