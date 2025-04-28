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
    etl::vector<ValveDriver, NUM_ODOR_VALVES>& odor_valves
    );

    ~PokeManager(); // desctructor

    void update();

/**
 * \brief true if a poke was detected. Inline replaces function with code
 */
    inline void poke()
    {poke_detected_ = true;}

    void deenergize_all_valves();

private:

/**
 * \brief time we've been in the current state.
 */
    inline uint32_t state_duration_us()
    {return time_us_32() - state_entry_time_us_;}

    // Declare data members
    state_t state_;
    uint32_t state_entry_time_us_;
    int valve_index_;
    size_t poke_count_;
    bool poke_detected_;
    ValveDriver& vac_valve_;
    ValveDriver& final_valve_;
    etl::vector<ValveDriver, NUM_ODOR_VALVES>& odor_valves_;

    // Declare Constants
    static inline constexpr uint32_t VACUUM_CLOSE_TIME_US = 2e4;// 20ms
    static inline constexpr uint32_t MIN_POKE_TIME_US = 1e6;
    static inline constexpr uint32_t ODOR_DELIVERY_TIME_US = 1e6; // 1 second
    static inline constexpr uint32_t ODOR_TRANSITION_TIME_US = 0; // No vaccum setup needed
    static inline constexpr uint32_t VAC_SETUP_TIME_US = 0;
    static inline constexpr uint32_t FINAL_VALVE_ENERGIZED_TIME_US = 0;

};

#endif // POKE_MANAGER_H
