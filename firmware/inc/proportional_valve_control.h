// Closed loop control of proportional valves to regulate flow rate. 
#include <valve_driver.h>
#include <pico/stdlib.h> 

/**
 * \brief Proportional valve class
 */
class ProportionalValveControl
{
public:
/**
 * \brief constructor.
 * \param proportional_valve_index the index of the proportional valve to control.
 * \param flow_adc_index the index of the ADC channel to read the flow rate from.
 */
    ProportionalValveControl(ValveDriver& proportional_valve, uint8_t flow_adc_index);

/**
 * \brief destructor.
 */
    ~ProportionalValveControl();

/**
 * \brief reset to proportional valve control.
 * \details reinitialize valve control settings to defaults.
 */
    void reset();

/**
 * \brief Call periodically in a loop to update duty cycle (voltage output) to proportional valve based on flow rate readings from ADC.
 */
    void update(float current_flow_rate);


/**
 * \brief Call periodically in a loop to update duty cycle (voltage output) to proportional valve based on flow rate readings from ADC.
 */
    void pid_controller();

// Getters and setters for PID parameters and target flow rate
    float get_target_flow_rate() const { return target_flow_rate_; }
    void set_target_flow_rate(float target_flow_rate) { target_flow_rate_ = target_flow_rate; }

    float get_kp() const { return kp_; }
    void set_kp(float kp) { kp_ = kp; }

    float get_ki() const { return ki_; }
    void set_ki(float ki) { ki_ = ki; }

    float get_kd() const { return kd_; }
    void set_kd(float kd) { kd_ = kd; }

    float get_pid_update_frequency() const { return update_rate_hz_; }
    void set_pid_update_frequency(float update_rate_hz) { update_rate_hz_ = update_rate_hz; }

    uint8_t get_flow_adc_index() const { return flow_adc_index_; }
    void set_flow_adc_index(uint8_t flow_adc_index) { flow_adc_index_ = flow_adc_index; }

    uint8_t get_pid_enabled() const { return control_enabled_; }
    void set_pid_enabled(bool enabled) { control_enabled_ = enabled; }

    float get_duty_cycle() const { return duty_cycle_; }
    void set_duty_cycle(float duty_cycle) { duty_cycle_ = duty_cycle; }

private:
    uint8_t flow_adc_index_;
    float target_flow_rate_;
    float current_flow_rate_;
    float kp_;
    float ki_;
    float kd_;
    float previous_error_;
    float integral_error_;
    float derivative_error_;
    float duty_cycle_;
    bool control_enabled_;
    float update_rate_hz_;
    uint32_t start_time_us_;
    ValveDriver& proportional_valve_;
    float prev_meas_;   // previous filtered measurement (flow)
    float d_filt_;   // filtered derivative term


    static const uint32_t DEFAULT_FREQUENCY_HZ = 25000; // Default PWM frequency for proportional valve control
    static const uint32_t DEFAULT_UPDATE_RATE_HZ = 100; // Default update rate for PID controller
    static const uint32_t DEFAULT_HIT_DURATION_US = 0; // Default hit duration for PID controller
};
