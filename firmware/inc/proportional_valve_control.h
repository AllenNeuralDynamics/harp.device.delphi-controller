// Closed loop control of proportional valves to regulate flow rate.
#include <valve_driver.h>
#include <pico/stdlib.h>

class ProportionalValveControl
{
public:
    ProportionalValveControl(ValveDriver& proportional_valve);
    ~ProportionalValveControl();

    void reset();
    void update(float current_flow_rate);
    void pid_controller();

    // PID parameters
    inline float get_target_flow_rate() const
     { return target_flow_rate_; }
    inline void set_target_flow_rate(float target_flow_rate) 
    { target_flow_rate_ = target_flow_rate; }

    inline float get_kp() const { return kp_; }
    inline void set_kp(float kp) { kp_ = kp; }

    inline float get_ki() const { return ki_; }
    inline void set_ki(float ki) { ki_ = ki; }

    inline float get_kd() const { return kd_; }
    inline void set_kd(float kd) { kd_ = kd; }

    inline float get_pid_update_frequency() const
     { return update_rate_hz_; }
    inline void set_pid_update_frequency(float update_rate_hz) 
    { update_rate_hz_ = update_rate_hz; }

    // ADC configuration (safe)
    inline bool   has_flow_adc() const { return flow_adc_index_ >= 0; }
    inline int8_t get_flow_adc_index() const
    { return flow_adc_index_; }
    inline void   set_flow_adc_index(int8_t flow_adc_index) 
    { flow_adc_index_ = flow_adc_index; }


    // PID enable state
    inline bool get_pid_enabled() const
     { return control_enabled_; }
    
     inline void set_pid_enabled(bool enabled) 
    {
        control_enabled_ = enabled; 
        if (!enabled) {
            set_duty_cycle(0.0f);   // HARD SAFETY OFF
            proportional_valve_.set_normalized_hold_output(0.0f);
            proportional_valve_.deenergize();}
    }


    inline float get_duty_cycle() const { return duty_cycle_; }
    inline void  set_duty_cycle(float duty_cycle) { duty_cycle_ = duty_cycle; }

private:
    int8_t  flow_adc_index_;   // -1 = not configured, 0..N-1 valid
    float   target_flow_rate_;
    float   current_flow_rate_;
    float   kp_;
    float   ki_;
    float   kd_;
    float   previous_error_;
    float   integral_error_;
    float   derivative_error_;
    float   duty_cycle_;
    bool    control_enabled_;
    float   update_rate_hz_;
    uint32_t start_time_us_;
    ValveDriver& proportional_valve_;
    float   prev_meas_;
    float   d_filt_;
    float u_prev_;

    static const uint32_t DEFAULT_FREQUENCY_HZ  = 25000;
    static const uint32_t DEFAULT_UPDATE_RATE_HZ = 100;
    static const uint32_t DEFAULT_HIT_DURATION_US = 0;
};