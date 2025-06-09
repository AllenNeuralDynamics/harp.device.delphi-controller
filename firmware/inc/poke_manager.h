#ifndef POKE_MANAGER_H // Include Gaurd
#define POKE_MANAGER_H

#include <hardware/timer.h>
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

    void pause(); // pause fsm

    void restart(); // restart fsm

    inline void update_next_odor(uint32_t next_odor)
    {next_odor_index_ = next_odor;}

    inline void set_vacuum_close_time_us(uint32_t vacuum_close_time_us)
    {vacuum_close_time_us_ = vacuum_close_time_us;}

    inline void set_odor_delivery_time_us(uint32_t odor_delivery_time_us)
    {odor_delivery_time_us_ = odor_delivery_time_us;}

    void set_odor_transition_time_us(uint32_t odor_transition_time_us)
    {odor_transition_time_us_ = odor_transition_time_us;}

    inline void set_vac_setup_time_us(uint32_t vac_setup_time_us)
    {vac_setup_time_us_ = vac_setup_time_us;}

    inline void set_final_valve_energized_time_us(uint32_t final_valve_energized_time_us)
    {final_valve_energized_time_us_ = final_valve_energized_time_us;}

    inline void set_min_poke_time_us(uint32_t min_poke_time_us)
    {min_poke_time_us_ = min_poke_time_us;}

    inline void set_poke_pin(uint8_t pin)
    {poke_pin_ = pin;}

/**
 * \brief true if a poke was detected. Inline replaces function with code
 */
    inline void poke()
    {poke_detected_ = true;}

    void deenergize_all_valves();

    void check_poke_status();

    inline size_t get_poke_count() const
    {return poke_count_;}

    inline int get_current_odor() const
    {return odor_valve_index_;}

    inline int get_next_odor() const
    {return next_odor_index_;}

    inline uint32_t get_vacuum_close_time() const
    {return vacuum_close_time_us_;}

    inline uint32_t get_odor_delivery_time() const
    {return odor_delivery_time_us_;}

    inline uint32_t get_odor_transition_time() const
    {return odor_transition_time_us_;}

    inline uint32_t get_vac_setup_time() const
    {return vac_setup_time_us_;}

    inline uint32_t get_final_valve_energized_time() const
    {return final_valve_energized_time_us_;}

    inline uint32_t get_min_poke_time() const
    {return min_poke_time_us_;}

private:

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

    // Declare Constants
    static inline constexpr uint32_t DEFAULT_VACUUM_CLOSE_TIME_US = 20e3;
    static inline constexpr uint32_t DEFAULT_ODOR_DELIVERY_TIME_US = 10e3; 
    static inline constexpr uint32_t DEFAULT_ODOR_TRANSITION_TIME_US = 30e3; 
    static inline constexpr uint32_t DEFAULT_VAC_SETUP_TIME_US = 20e3;
    static inline constexpr uint32_t DEFAULT_FINAL_VALVE_ENERGIZED_TIME_US = 110e3;
    static inline constexpr uint32_t MIN_POKE_TIME_US = 10e3;
    static inline constexpr uint8_t DEFAUT_POKE_PIN = GPIO_PIN_BASE;
};

#endif // POKE_MANAGER_H
