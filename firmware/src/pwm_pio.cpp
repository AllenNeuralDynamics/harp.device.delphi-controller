#include <pwm_pio.h>

CameraDriver::CameraDriver(uint8_t pwm_pio_pin)
: pwm_pio_pin_{DEFAULT_PIO_PWM_PIN}, pwm_freq_{DEFAULT_FREQ}, 
pwm_duty_{DEFAULT_DUTY_CYCLE}, pin_is_initialized_{false},
sm_{DEFAULT_PIO_SM}, pio_{pio0}, pwm_pin_state_{0}, 
enable_state_{1}, disabled_{false}
{
    reset(); // set timing constants to defaults.
}

CameraDriver::~CameraDriver() //destuctor
{
  pwm_freq_ = 0;  
  pwm_pin_state_ = 0;
}

// Functions to alter the FSM
void CameraDriver::reset()
{
    sm_ = DEFAULT_PIO_SM;
    uint8_t offset = pio_add_program(pio_, &pwm_program);
    pwm_init(pio_, sm_, offset, pwm_pio_pin_);
    pwm_freq_ = DEFAULT_FREQ;
    pwm_duty_ = DEFAULT_DUTY_CYCLE; //50% duty cycle
}

void CameraDriver::pwm_init(PIO pio, uint8_t sm, uint8_t offset, uint8_t pin)
 {
    pio_gpio_init(pio, pin);
    pio_sm_set_consecutive_pindirs(pio, sm, pin, 1, true);
    pio_sm_config c = pwm_program_get_default_config(offset);
    sm_config_set_clkdiv(&c, 1.0f);  // full speed
    sm_config_set_set_pins(&c, pin, 1);
    sm_config_set_fifo_join(&c, PIO_FIFO_JOIN_NONE);
    pio_sm_init(pio, sm, offset, &c);
    pio_sm_set_enabled(pio, sm, true);
}

void CameraDriver::set_pwm(PIO pio, uint sm, float duty_cycle, uint32_t freq) 
{
    // const uint32_t total_us = 16667;  // 60 Hz period in µs
    if (freq != 0)
    {
        uint32_t sys_clk = clock_get_hz(clk_sys);
        uint32_t period = sys_clk / freq;
        uint32_t high_us = (uint32_t)(period * duty_cycle);
        uint32_t low_us = period - high_us;
        pio_sm_put_blocking(pio, sm, high_us);
        pio_sm_put_blocking(pio, sm, low_us);
    } 
    else 
    {
        uint32_t period = 0;
        uint32_t high_us = (uint32_t)(period * duty_cycle);
        uint32_t low_us = period - high_us;
        pio_sm_put_blocking(pio, sm, high_us);
        pio_sm_put_blocking(pio, sm, low_us);  
    }
    
}

void CameraDriver::check_camera_trigger_state(uint8_t enable)
{
    if (enable == 1)
    {
        disabled_= false;
        pio_sm_set_enabled(pio_, sm_, true); 
    }
    else
    {
        pwm_freq_ = 0;
        set_pwm(pio_, sm_, pwm_duty_, pwm_freq_); 
        pio_sm_set_enabled(pio_, sm_, false); 
        disabled_= true;
    }
}


//Rise and Fall events of camera triggering
void CameraDriver::pwm_signal_status()
{
    // Check to see if poke has been detected
    // Beam is no longer broken
    if (!gpio_get(pwm_pio_pin_))
    {
        if (pwm_pin_state_ == 1)
        {
            //falling edge event
            // raw_poke_fall();
        }
        pwm_pin_state_ = 0;  
    }

    // Beam broken -- update raw poke state
    if (gpio_get(pwm_pio_pin_))
    {
        if (pwm_pin_state_ == 0)
        {
            //rising edge event
            // raw_poke_rise();
        }
        pwm_pin_state_ = 1;  
    }
}


void CameraDriver::update()
{
    //check trigger state
    check_camera_trigger_state(enable_state_);

    //enabled by default, but if disabled, bail early
    if (disabled_)
        return;

    // check pin state -- trigger events
    pwm_signal_status();

    // set PWM 
    set_pwm(pio_, sm_, pwm_duty_, pwm_freq_); 
    // sleep_ms(1); // see about elimininating
}

