#ifndef POKE_MANAGER_H // Include Gaurd
#define POKE_MANAGER_H

#include <hardware/timer.h>
#include <hardware/gpio.h>
#include <pico/stdlib.h> // for uart printing
#include <cstdio> // for printf
#include <valve_driver.h>
#include <etl/vector.h>
#include <config.h>


class PokeManager
{
public:

    enum state_t
    {
        RESET,
        ODOR_SETUP,
        ODOR_DISPENSING_TO_EXHAUST,
        ODOR_DELIVERY_TO_FINAL_VALVE,
        ODOR_PRECLEAN,
        VAC_START,
        ODOR_PURGE,
    };


    // Declare constructor
    PokeManager(
        ValveDriver& final_valve, //Pass by reference (work of this org. object)
        ValveDriver& vac_valve,
        ValveDriver (&odor_valves)[],
        size_t num_odor_valves
    );

    ~PokeManager(); // desctructor

    void update();

    void reset(); // reset the fsm

    inline void enable()
    {set_enabled_state(true);}

    inline void disable()
    {set_enabled_state(false);}

    inline void set_odor_valve_state(bool enabled)
    {
        if (odor_valve_index_ >= 0)
        {
            if (enabled)
                odor_valves_[odor_valve_index_].energize();
            else
                odor_valves_[odor_valve_index_].deenergize();
        }
    }

    inline void energize_odor_valve()
    {set_odor_valve_state(1);}

    inline void deenergize_odor_valve()
    {set_odor_valve_state(0);}

    inline void set_next_odor_callback_fn( void (* fn)(void))
    {request_next_odor_callback_fn_ = fn;}

    inline void request_next_odor()
    {
        if (request_next_odor_callback_fn_ != nullptr)
            request_next_odor_callback_fn_();
    }

/*
 * \brief enable (true) or disable (false) the odor delivery state machine.
 */
    void set_enabled_state(bool enabled);

    inline void set_current_odor(uint32_t odor_index)
    {odor_valve_index_ = odor_index;}

    inline void set_next_odor(uint32_t next_odor)
    {next_odor_index_ = next_odor;}

    inline void set_vacuum_close_time_us(uint32_t vacuum_close_time_us)
    {vacuum_close_time_us_ = vacuum_close_time_us;}

    inline void set_odor_delivery_time_us(uint32_t odor_delivery_time_us)
    {odor_delivery_time_us_ = odor_delivery_time_us;}

    void set_odor_transition_time_us(uint32_t odor_transition_time_us)
    {odor_transition_time_us_ = odor_transition_time_us;}

    inline void set_vacuum_setup_time_us(uint32_t vac_setup_time_us)
    {vac_setup_time_us_ = vac_setup_time_us;}

    inline void set_final_valve_energized_time_us(uint32_t final_valve_energized_time_us)
    {final_valve_energized_time_us_ = final_valve_energized_time_us;}

    inline void set_min_poke_time_us(uint32_t min_poke_time_us)
    {min_poke_time_us_ = min_poke_time_us;}

    inline void set_poke_pin(uint8_t pin)
    {
        clear_poke_pin();
        // Init new gpio pin.
        poke_pin_ = pin;
        gpio_init(poke_pin_);
        gpio_set_dir(poke_pin_, GPIO_IN);
        poke_pin_is_initialized_ = true;
        // Apply override state.
        set_poke_pin_override_state(override_state_);
    }

    inline void clear_poke_pin()
    {
        if (!poke_pin_is_initialized_)
            return;
        set_poke_pin_override_state(GPIO_OVERRIDE_NORMAL);
        gpio_deinit(poke_pin_);
        poke_pin_ = DEFAUT_POKE_PIN;
        poke_pin_is_initialized_ = false;
    }

    inline void force_poke()
    {poke();}

/**
 * \brief set the poke pin input override state to (1) invert or (0) uninvert
 *  the input.
*/
    inline void set_poke_pin_override_state(gpio_override override_state)
    {
        override_state_ = override_state; // Cache the override setting.
        if (poke_pin_is_initialized_)
            gpio_set_inover(poke_pin_, override_state);
    }

    void deenergize_all_valves();


/**
 * \brief true if the poke pin is inverted.
*/
    inline uint8_t poke_pin_is_inverted() const
    {
        gpio_override override_state =
            gpio_override(
                (io_bank0_hw->io[poke_pin_].ctrl & IO_BANK0_GPIO0_CTRL_INOVER_BITS)
                >> IO_BANK0_GPIO0_CTRL_INOVER_LSB);
        return override_state == GPIO_OVERRIDE_INVERT;
    }

    inline uint32_t get_enabled_state() const
    {return !disable_fsm_;}

    inline uint8_t get_poke_pin() const
    {return poke_pin_;}

    inline size_t get_poke_count() const
    {return poke_count_;}

    inline uint32_t get_current_odor() const
    {return odor_valve_index_;}

    inline uint32_t get_next_odor() const
    {return next_odor_index_;}

    inline uint32_t get_vacuum_close_time_us() const
    {return vacuum_close_time_us_;}

    inline uint32_t get_odor_delivery_time_us() const
    {return odor_delivery_time_us_;}

    inline uint32_t get_odor_transition_time_us() const
    {return odor_transition_time_us_;}

    inline uint32_t get_vacuum_setup_time_us() const
    {return vac_setup_time_us_;}

    inline uint32_t get_final_valve_energized_time_us() const
    {return final_valve_energized_time_us_;}

    inline uint32_t get_min_poke_time_us() const
    {return min_poke_time_us_;}

private:

/**
 * \brief update whether or not the input has seen a poke. Called in a loop.
 */
    void update_poke_status();

/**
 * \brief count a poke.
 */
    inline void poke()
    {poke_detected_ = true;}


/**
 * \brief time we've been in the current state.
 */
    inline uint32_t state_duration_us()
    {return time_us_32() - state_entry_time_us_;}

    // Declare data members
    state_t state_;
    uint32_t state_entry_time_us_;
    uint32_t poke_start_time_us_;

    uint8_t poke_pin_;
    gpio_override override_state_; /// Whether or not the poke pin is inverted.

    int odor_valve_index_;
    int next_odor_index_;

    size_t poke_count_;
    bool poke_detected_;
    bool disable_fsm_;
    bool beam_broken_; //keep track of beam state
    bool poke_initiated_once_; //Only trigger the FSM on 1 poke
    ValveDriver& vac_valve_;
    ValveDriver& final_valve_;

    ValveDriver (&odor_valves_)[];
    size_t num_odor_valves_;

    uint32_t vacuum_close_time_us_;
    uint32_t odor_delivery_time_us_;
    uint32_t odor_transition_time_us_;
    uint32_t vac_setup_time_us_;
    uint32_t final_valve_energized_time_us_;
    uint32_t min_poke_time_us_;

    void (*request_next_odor_callback_fn_)(void);

    bool poke_pin_is_initialized_;

    // Declare Constants
    static inline constexpr uint32_t DEFAULT_VACUUM_CLOSE_TIME_US = 20e3;
    static inline constexpr uint32_t DEFAULT_ODOR_DELIVERY_TIME_US = 10e3;
    static inline constexpr uint32_t DEFAULT_ODOR_TRANSITION_TIME_US = 30e3;
    static inline constexpr uint32_t DEFAULT_VACUUM_SETUP_TIME_US = 20e3;
    static inline constexpr uint32_t DEFAULT_FINAL_VALVE_ENERGIZED_TIME_US = 110e3;
    static inline constexpr uint32_t MIN_POKE_TIME_US = 10e3;
    static inline constexpr uint8_t DEFAUT_POKE_PIN = GPIO_PIN_BASE;
};

#endif // POKE_MANAGER_H
