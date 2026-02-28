#include <proportional_valve_control.h>

ProportionalValveControl::ProportionalValveControl(ValveDriver& proportional_valve, uint8_t flow_adc_index):
flow_adc_index_{flow_adc_index}, target_flow_rate_{0.0f}, proportional_valve_{proportional_valve},
kp_{0.0f}, ki_{0.0f}, kd_{0.0f}, previous_error_{0.0f}, integral_error_{0.0f}, derivative_error_{0.0f},
duty_cycle_{0.0f}, control_enabled_{false}, update_rate_hz_{DEFAULT_UPDATE_RATE_HZ}, start_time_us_{0},
prev_meas_{0.0f}, d_filt_{0.0f}
{
    reset();
}

ProportionalValveControl::~ProportionalValveControl()
{
    control_enabled_ = false;
    duty_cycle_ = 0.0f;
    proportional_valve_.set_normalized_hold_output(duty_cycle_);
    proportional_valve_.deenergize();   
}

void ProportionalValveControl::reset()
{
    target_flow_rate_ = 0.0f;
    kp_ = 0.0f;
    ki_ = 0.0f;
    kd_ = 0.0f;
    previous_error_ = 0.0f;
    integral_error_ = 0.0f;
    derivative_error_ = 0.0f;
    duty_cycle_ = 0.0f;
    control_enabled_ = false;
    start_time_us_ = 0;
    update_rate_hz_ = DEFAULT_UPDATE_RATE_HZ;
    proportional_valve_.set_pwm_frequency_hz(DEFAULT_FREQUENCY_HZ);
    proportional_valve_.set_hit_duration_us(DEFAULT_HIT_DURATION_US);
    proportional_valve_.set_normalized_hold_output(duty_cycle_);
    proportional_valve_.deenergize();
}

void ProportionalValveControl::pid_controller()
{
    const float dt = (1.0f / update_rate_hz_);
    if (dt <= 0.0f) return; // guard

    // --- 1) Error (using your filtered current_flow_rate_) ---
    const float y = current_flow_rate_;
    const float error = target_flow_rate_ - y;

    // --- 2) Derivative on measurement with low-pass filter ---
    // Raw derivative of measurement
    float dy = 0.0f;
    if (dt > 1e-6f) {
        dy = (y - prev_meas_) / dt;
    }

    // Low-pass the derivative to tame noise (fc_d ~ 10–15 Hz works well at 100 Hz update)
    const float fc_d = 25.0f; // Hz (tunable)
    float alpha_d = (2.0f * 3.14159265f * fc_d * dt) / (1.0f + 2.0f * 3.14159265f * fc_d * dt);
    if (alpha_d < 0.0f) alpha_d = 0.0f;
    if (alpha_d > 1.0f) alpha_d = 1.0f;
    d_filt_ = d_filt_ + alpha_d * (dy - d_filt_);

    // D contribution is negative on measurement
    const float u_d = -kd_ * d_filt_;

    // --- 3) Unclamped control (controller space) ---
    float u_unclamped = kp_ * error + ki_ * integral_error_ + u_d;

    // --- 4) Anti-windup (conditional integration in controller space [0,1]) ---
    float u_sat = u_unclamped;
    if (u_sat < 0.0f) u_sat = 0.0f;
    if (u_sat > 1.0f) u_sat = 1.0f;

    const bool sat_high = (u_unclamped > 1.0f);
    const bool sat_low  = (u_unclamped < 0.0f);
    const bool allow_i =
        (!sat_high && !sat_low) ||
        (sat_high && (error < 0.0f)) ||
        (sat_low  && (error > 0.0f));

    if (allow_i) {
        integral_error_ += error * dt;

        // Integral clamp: limit |Ki * I| <= 0.5 of output span
        if (ki_ > 0.0f) {
            const float Iabs_max = 0.5f / ki_;
            if (integral_error_ >  Iabs_max) integral_error_ =  Iabs_max;
            if (integral_error_ < -Iabs_max) integral_error_ = -Iabs_max;
        }
    }

    // --- 5) Recompute after integral update and clamp to [0,1] ---
    float u_cmd = kp_ * error + ki_ * integral_error_ + u_d;
    if (u_cmd < 0.0f) u_cmd = 0.0f;
    if (u_cmd > 1.0f) u_cmd = 1.0f;

    // --- 6) Slew-rate limiter (controller space) ---
    static float u_prev = 0.0f;            // or make this a member: u_prev_
    const float du_max_per_s = 0.6f;       // tune 0.3–1.0 full-scale per second
    const float du_step = du_max_per_s * dt;
    float du = u_cmd - u_prev;
    if (du >  du_step) u_cmd = u_prev + du_step;
    if (du < -du_step) u_cmd = u_prev - du_step;
    if (u_cmd < 0.0f) u_cmd = 0.0f;
    if (u_cmd > 1.0f) u_cmd = 1.0f;
    u_prev = u_cmd;

    // --- 7) Output (controller space). Map deadband outside if you use D_MIN/D_MAX. ---
    duty_cycle_ = u_cmd;
    proportional_valve_.set_normalized_hold_output(duty_cycle_);

    // --- 8) Save state for next tick ---
    prev_meas_ = y;
    previous_error_ = error; // keep if you still want it for logging
}


// void ProportionalValveControl::pid_controller()
// {
//     const float dt = (1.0f / update_rate_hz_);

//     // --- existing PID body (as we last wrote) ---
//     const float error = target_flow_rate_ - current_flow_rate_;
//     const float d_raw = (dt > 1e-6f) ? (error - previous_error_) / dt : 0.0f;

//     float u_unclamped = kp_ * error + ki_ * integral_error_ + kd_ * d_raw;

//     float u_sat = u_unclamped;
//     if (u_sat < 0.0f) u_sat = 0.0f;
//     if (u_sat > 1.0f) u_sat = 1.0f;

//     const bool sat_high = (u_unclamped > 1.0f);
//     const bool sat_low  = (u_unclamped < 0.0f);
//     const bool allow_i =
//         (!sat_high && !sat_low) ||
//         (sat_high && (error < 0.0f)) ||
//         (sat_low  && (error > 0.0f));

//     if (allow_i) {
//         integral_error_ += error * dt;
//         // Clamp |Ki * I| <= 0.5 of output span
//         if (ki_ > 0.0f) {
//             const float Iabs_max = 0.5f / ki_;
//             if (integral_error_ >  Iabs_max) integral_error_ =  Iabs_max;
//             if (integral_error_ < -Iabs_max) integral_error_ = -Iabs_max;
//         }
//     }

//     // Recompute and clamp in [0,1]
//     float u_cmd = kp_ * error + ki_ * integral_error_ + kd_ * d_raw;
//     if (u_cmd < 0.0f) u_cmd = 0.0f;
//     if (u_cmd > 1.0f) u_cmd = 1.0f;

//     // --- NEW: Slew-rate limiter in controller space ---
//     static float u_prev = 0.0f;            // if you prefer, make this a member (u_prev_)
//     const float du_max_per_s = 0.6f;       // tune: 0.3–1.0 (full-scale per second)
//     const float du_step = du_max_per_s * dt;
//     float du = u_cmd - u_prev;
//     if (du >  du_step) u_cmd = u_prev + du_step;
//     if (du < -du_step) u_cmd = u_prev - du_step;
//     if (u_cmd < 0.0f) u_cmd = 0.0f;
//     if (u_cmd > 1.0f) u_cmd = 1.0f;
//     u_prev = u_cmd;

//     duty_cycle_ = u_cmd;

//     // IMPORTANT: Deadband linearization should be applied OUTSIDE the PID.
//     // For now we leave your direct hold write as-is, but ideally map:
//     // duty_hold = D_MIN + duty_cycle_ * (D_MAX - D_MIN)
//     proportional_valve_.set_normalized_hold_output(duty_cycle_);

//     previous_error_ = error;
// }

void ProportionalValveControl::update(float current_flow_rate)
{
    // Check the valve state and energize/deenergize the valve or exit the loop needed before running PID controller
    if (control_enabled_ && !proportional_valve_.is_energized())
    {
        proportional_valve_.energize();
    }
    else if (!control_enabled_ && proportional_valve_.is_energized())
    {
        proportional_valve_.deenergize();
        return;
    }
    else if (!control_enabled_) return;

    // Check to see if the alloted time has passed since last update before running PID controller again
    // This ensures that we are running the PID controller at the specified update rate.
    if ((time_us_32() - start_time_us_) >= (1000000.0f / update_rate_hz_))  // seconds to microseconds conversion
    {
        current_flow_rate_ = current_flow_rate;
        pid_controller();
        start_time_us_ = time_us_32();
    }
}