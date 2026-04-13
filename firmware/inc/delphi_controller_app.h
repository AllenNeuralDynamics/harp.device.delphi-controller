#ifndef DELPHI_CONTROLLER_APP_H
#define DELPHI_CONTROLLER_APP_H
#include <pico/stdlib.h>
#include <cstring>
#include <config.h>
#include <harp_message.h>
#include <harp_core.h>
#include <harp_c_app.h>
#include <valve_driver.h>
#include <poke_manager.h>
#include <pwm_pio.h>
#include <flow_detection.h>
#include <proportional_valve_control.h>
#include <hardware/gpio.h>
#ifdef DEBUG
    #include <stdio.h>
    #include <cstdio> // for printf
#endif

// Setup for Harp App
inline constexpr size_t APP_REG_COUNT = 74;
// Numeric addresses for Harp Registers (clunky) -- DO ALL NEW REGISTERS NEED TO BE REFERENCED TO THESE??
inline constexpr size_t VALVE_START_APP_ADDRESS = APP_REG_START_ADDRESS + 3;
inline constexpr size_t LAST_VALVE_APP_ADDRESS = VALVE_START_APP_ADDRESS + NUM_VALVES - 1;
inline constexpr size_t AUX_GPIO_INPUT_RISE_EVENT_ADDRESS = LAST_VALVE_APP_ADDRESS + 5;
inline constexpr size_t AUX_GPIO_RISING_INPUTS_ADDRESS = AUX_GPIO_INPUT_RISE_EVENT_ADDRESS + 2;
inline constexpr size_t AUX_GPIO_FALLING_INPUTS_ADDRESS = AUX_GPIO_INPUT_RISE_EVENT_ADDRESS + 3;

extern RegSpecs app_reg_specs[APP_REG_COUNT];
extern RegFnPair reg_handler_fns[APP_REG_COUNT];
extern HarpCApp& app;

extern ValveDriver valve_drivers[NUM_VALVES];
extern PokeManager poke_manager;
extern CameraDriver cam0_driver;
extern CameraDriver cam1_driver;
extern FlowDetection flow_detection;
extern ProportionalValveControl proportional_valve_0_controller;
extern ProportionalValveControl proportional_valve_1_controller;
extern ProportionalValveControl proportional_valve_2_controller;
extern uint8_t old_aux_gpio_inputs;

// struct for HARP event queueing
static inline constexpr uint8_t CAM0_PIN_STATE_INDEX_ADDRESS = 72;
static inline constexpr uint8_t CAM1_PIN_STATE_INDEX_ADDRESS = 76;
struct HarpEvent {
    uint8_t index;
    uint64_t timestamp;
};

//Valves state mask variables
const uint8_t VALVES_STATE_INDEX_ADDRESS = 32;

// Valve configuration struct for configuring the Hit-and-hold driver
#pragma pack(push, 1)
struct ValveConfig
{
    float hit_output;
    float hold_output;
    uint32_t hit_duration_us;
};
#pragma pack(pop)

// PID Gains
#pragma pack(push, 1)
struct PidConfig
{
    float kp;
    float ki;
    float kd;
};
#pragma pack(pop)

// Registers
#pragma pack(push, 1)
struct app_regs_t
{
    uint16_t ValvesState; // Raw (energized/deenergized) state of all valves.
                          // Bitmask: one bit per valve.
                          // Write: 0 = deenergize. 1 = energize
                          // Read: 0 = deenergized. 1 = energized
    uint16_t ValvesSet; // Energize the valves specified in the bitmask.
                        // Bitmask: one bit per valve. (1 = energize)
                        // Read values are identical to ValveStates.
    uint16_t ValvesClear; // Deenergize the valve specified in the bitmask.
                          // Bitmask: one bit per valve. (1 = de-energize)
                          // Read values are the bitwise inverse of ValvesState
    /// @ref ValveConfigs are represented as 16 individual registers instead
    /// of a register with an array of 16 ValveConfigs.
    ValveConfig ValveConfigs[NUM_VALVES]; // Represents app regs: 35, 36, ... 50
                                          // 16 Heterogeneous registers each
                                          // representing a ValveConfig packed
                                          // struct.
    uint8_t AuxGPIODir;
    uint8_t AuxGPIOState;
    uint8_t AuxGPIOSet;
    uint8_t AuxGPIOClear;

    uint8_t AuxGPIOInputRiseEvent;
    uint8_t AuxGPIOInputFallEvent;
    uint8_t AuxGPIORisingInputs; // Raw state of which inputs rose (could be multiple)
    uint8_t AuxGPIOFallingInputs; // Raw state of which inputs fell (could be multiple)
    
    // Poke Manager app "registers" here.
    uint8_t PokePin;
    uint8_t PokePinInverted;
    uint8_t PokeState;
    uint8_t RawPokeState;
    uint32_t PokeDometer;
    uint8_t FSMEnabledState;
    uint8_t ForceFSM;
    int16_t QueuedOdorMask;
    uint32_t OdorSetupTimeUS;
    uint32_t MinOdorDeliveryTimeUS;
    uint32_t MaxOdorDeliveryTimeUS;
    uint32_t MinimumPokeTimeUS;
    uint32_t OdorDwellTimeUS;

    // Camera 0 registers
    uint8_t Cam0PinState;
    uint32_t Cam0FrameRate;
    float Cam0DutyCycle;
    uint8_t EnableCam0Trigger;

    // Camera 1 registers
    uint8_t Cam1PinState;
    uint32_t Cam1FrameRate;
    float Cam1DutyCycle;
    uint8_t EnableCam1Trigger;

    uint8_t EnableValveLeds;

    // ADC registers
    FlowDetection::ADC_Samples LatestFlowRate; //Read Only (5 float ADC flow rates)
    FlowDetection::ADC_Samples_Raw LatestRawAdcSample; //Read Only (5 float ADC raw samples)
    uint8_t EnableAdcSampling;
    int8_t LeakAdcChannel;
    float LeakThreshold;
    uint8_t LeakState;

    // Manual flow meter calibration registers
    int8_t ManualFlowMeter; // ADC channel used for manual flow meter calibration
    float NominalFlowRate; // Nominal flow rate for manual flow meter calibration 
    float FlowRateTolerance; // Tolerance for flow rate detection (e.g., +-0.1 L/min)
    uint8_t ManualFlowMeterState; // State of manual flow meter (0 = normal, 1 = alert)

    FlowDetection::FlowMeterRegisterBlock FlowMeterCalibrations; // Register block for flow meter calibration coefficients

    // Proportional valve PID control
    // General PID registers
    float PidUpdateFrequency;
    PidConfig PidGains;

    //Proportional valve 0 registers
    int8_t ProportionalValve0Adc;
    uint8_t ProportionalValve0EnablePid;
    float ProportionalValve0DutyCycle;
    float ProportionalValve0TargetFlowRate;

    //Proportional valve 1 registers
    int8_t ProportionalValve1Adc;
    uint8_t ProportionalValve1EnablePid;
    float ProportionalValve1DutyCycle;
    float ProportionalValve1TargetFlowRate;

    //Proportional valve 2 registers
    int8_t ProportionalValve2Adc;
    uint8_t ProportionalValve2EnablePid;
    float ProportionalValve2DutyCycle;
    float ProportionalValve2TargetFlowRate;
};
#pragma pack(pop)

extern app_regs_t app_regs;

/**
 * \brief callback function to alert when the leak status changes
 */
void leak_state_alert(void);

/**
 * \brief callback function to alert when the manual flow meter status changes
 */
void manual_flow_meter_alert(void);

/**
 * \brief callback function to tell the PC we need another odor from
 *  within the PokeManager state machine logic.
 */
void request_next_odor(void);

/**
 * \brief callback function to tell the PC when the poke state changed
 */
void poke_state_changed(void);

/**
 * \brief callback function to tell the PC when the beam broke (raw poke)
 */
void raw_poke_rise(void);

/**
 * \brief callback function to tell the PC when the beam broke (raw poke)
 */
void raw_poke_fall(void);

/**
 * \brief callback for camera timestamp
 */
void camera_timestamp_callback(uint gpio, uint32_t events);

/**
 * \brief function for queueing HARP events
 */
void push_event_from_isr(uint8_t index, uint64_t timestamp);
bool pop_event(HarpEvent &event);

/**
 * \brief function getting valve state mask
 */
uint16_t get_valve_mask();


/**
 * \brief update the app state. Called in a loop.
 */
void update_app_state();

/**
 * \brief reset the app.
 */
void reset_app();

inline uint8_t read_aux_gpios()
{return uint8_t((gpio_get_all() >> GPIO_PIN_BASE) & GPIOS_MASK);}

void read_valves_state(uint8_t reg_address);
void read_valves_set(uint8_t reg_address);
void read_valves_clear(uint8_t reg_address);
void read_any_valve_config(uint8_t reg_address);
void read_aux_gpio_state(uint8_t reg_address);

void read_poke_pin(uint8_t reg_address);
void read_poke_pin_inverted(uint8_t reg_address);
void read_poke_state(uint8_t reg_address);
void read_raw_poke_state(uint8_t reg_address);
void read_pokedometer(uint8_t reg_address);
void read_fsm_enabled_state(uint8_t reg_address);
//void read_force_fsm(uint8_t reg_address); // aliased to read_reg_generic
void read_current_odors(uint8_t reg_address);
void read_odor_setup_time_us(uint8_t reg_address);
void read_min_odor_delivery_time_us(uint8_t reg_address);
void read_max_odor_delivery_time_us(uint8_t reg_address);
void read_minimum_poke_time_us(uint8_t reg_address);
void read_odor_dwell_time_us(uint8_t reg_address);

void read_cam0_pin_state(uint8_t reg_address);
void read_cam0_frame_rate(uint8_t reg_address);
void read_cam0_duty_cycle(uint8_t reg_address);
void read_enable_cam0_trigger(uint8_t reg_address);
void read_cam1_pin_state(uint8_t reg_address);
void read_cam1_frame_rate(uint8_t reg_address);
void read_cam1_duty_cycle(uint8_t reg_address);
void read_enable_cam1_trigger(uint8_t reg_address);
void read_valve_leds(uint8_t reg_address);

void read_adc(uint8_t reg_address);
void read_raw_adc(uint8_t reg_address);
void read_adc_enable(uint8_t reg_address);
void read_leak_adc_channel(uint8_t reg_address);
void read_leak_threshold(uint8_t reg_address);
void read_leak_state(uint8_t reg_address);
void read_manual_flow_meter(uint8_t reg_address);
void read_nominal_flow_rate(uint8_t reg_address);
void read_flow_rate_tolerance(uint8_t reg_address);
void read_manual_flow_meter_state(uint8_t reg_address);
void read_flow_meter_calibrations(uint8_t reg_address);

void read_pid_update_frequency(uint8_t reg_address);
void read_pid_gains(uint8_t reg_address);
void read_proportional_valve_0_adc(uint8_t reg_address);
void read_proportional_valve_0_enable_pid(uint8_t reg_address);
void read_proportional_valve_0_duty_cycle(uint8_t reg_address);
void read_proportional_valve_0_target_flow_rate(uint8_t reg_address);
void read_proportional_valve_1_adc(uint8_t reg_address);
void read_proportional_valve_1_enable_pid(uint8_t reg_address);
void read_proportional_valve_1_duty_cycle(uint8_t reg_address);
void read_proportional_valve_1_target_flow_rate(uint8_t reg_address);
void read_proportional_valve_2_adc(uint8_t reg_address);
void read_proportional_valve_2_enable_pid(uint8_t reg_address);
void read_proportional_valve_2_duty_cycle(uint8_t reg_address);
void read_proportional_valve_2_target_flow_rate(uint8_t reg_address);


void write_valves_state(msg_t& msg);
void write_valves_set(msg_t& msg);
void write_valves_clear(msg_t& msg);
void write_any_valve_config(msg_t& msg);
void write_aux_gpio_dir(msg_t& msg);
void write_aux_gpio_state(msg_t& msg);
void write_aux_gpio_set(msg_t& msg);
void write_aux_gpio_clear(msg_t& msg);

void write_poke_pin(msg_t& msg);
void write_poke_pin_inverted(msg_t& msg);
// Cannot write to poke_stage
// Cannot write to pokedometer
void write_fsm_enabled_state(msg_t& msg);
void write_force_fsm(msg_t& msg);
void write_current_odors(msg_t& msg);
void write_odor_setup_time_us(msg_t& msg);
void write_min_odor_delivery_time_us(msg_t& msg);
void write_max_odor_delivery_time_us(msg_t& msg);
void write_minimum_poke_time_us(msg_t& msg);
void write_odor_dwell_time_us(msg_t& msg);
void write_cam0_frame_rate(msg_t& msg);
void write_cam0_duty_cycle(msg_t& msg);
void write_enable_cam0_trigger(msg_t& msg);
void write_cam1_frame_rate(msg_t& msg);
void write_cam1_duty_cycle(msg_t& msg);
void write_enable_cam1_trigger(msg_t& msg);
void write_valve_leds(msg_t& msg);

void write_adc_enable(msg_t& msg);
void write_leak_adc_channel(msg_t& msg);
void write_leak_threshold(msg_t& msg);

void write_manual_flow_meter(msg_t& msg);
void write_nominal_flow_rate(msg_t& msg);
void write_flow_rate_tolerance(msg_t& msg);
void write_flow_meter_calibrations(msg_t& msg);

void write_pid_update_frequency(msg_t& msg);
void write_pid_gains(msg_t& msg);
void write_proportional_valve_0_adc(msg_t& msg);
void write_proportional_valve_0_enable_pid(msg_t& msg);
void write_proportional_valve_0_duty_cycle(msg_t& msg);
void write_proportional_valve_0_target_flow_rate(msg_t& msg);
void write_proportional_valve_1_adc(msg_t& msg);
void write_proportional_valve_1_enable_pid(msg_t& msg);
void write_proportional_valve_1_duty_cycle(msg_t& msg);
void write_proportional_valve_1_target_flow_rate(msg_t& msg);
void write_proportional_valve_2_adc(msg_t& msg);
void write_proportional_valve_2_enable_pid(msg_t& msg);
void write_proportional_valve_2_duty_cycle(msg_t& msg);
void write_proportional_valve_2_target_flow_rate(msg_t& msg);


#endif // DELPHI_CONTROLLER_APP_H
