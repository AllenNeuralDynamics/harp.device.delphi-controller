#ifndef POKE_MANAGER_H
#define POKE_MANAGER_H

#include <hardware/timer.h>
#include <pico/stdlib.h> // for uart printing
#include <cstdio> // for printf


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

    PokeManager();

    ~PokeManager();


    void update();


/**
 * \brief true if a poke was detected.
 */
    bool poke_detected();



private:

/**
 * \brief time we've been in the current state.
 */
    inline uint32_t state_duration_us()
    {return time_us_32() - state_entry_time_us_;}

    state_t state_;
    uint32_t state_entry_time_us_;
    size_t poke_count_;

    // Constants
    static inline constexpr uint32_t VACUUM_CLOSE_TIME_US = 1e5;// 100'000;
    static inline constexpr uint32_t MIN_POKE_TIME_US = 10e3;
    static inline constexpr uint32_t ODOR_DELIVERY_TIME_US = 20e3; // ??
    static inline constexpr uint32_t ODOR_TRANSITION_TIME_US = 10e3;
    static inline constexpr uint32_t VAC_SETUP_TIME_US = 10e3;
    static inline constexpr uint32_t FINAL_VALVE_ENERGIZED_TIME_US = 20e3;

};

#endif // POKE_MANAGER_H
