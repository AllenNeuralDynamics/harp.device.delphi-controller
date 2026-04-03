#include <cstring>
#include <harp_synchronizer.h>
#include <harp_c_app.h>
#include <core_registers.h>
#include <reg_types.h>
#include <config.h>
#include <etl/vector.h>
#include <poke_manager.h>
#include <valve_driver.h>
#include <delphi_controller_app.h>
#ifdef DEBUG
    #include <pico/stdlib.h> // for uart printing
    #include <cstdio> // for printf
#endif
// Create Harp App.
HarpCApp& app = HarpCApp::init(HARP_DEVICE_ID,
                               HW_VERSION_MAJOR, HW_VERSION_MINOR,
                               HW_ASSEMBLY_VERSION,
                               HARP_VERSION_MAJOR, HARP_VERSION_MINOR,
                               FW_VERSION_MAJOR, FW_VERSION_MINOR,
                               UNUSED_SERIAL_NUMBER,
                               "delphi-controller",
                               (uint8_t*)GIT_HASH,
                               &app_regs, app_reg_specs,
                               reg_handler_fns, APP_REG_COUNT, update_app_state,
                               reset_app);


// Final valve driver
ValveDriver& final_valve = valve_drivers[FINAL_VALVE_INDEX]; 

// Consider the rest of the valves as odor delivery valves.
ValveDriver* odor_valves_start = valve_drivers + ODOR_VALVE_INDEX_START; // Odor valves start after the final valve
ValveDriver (&odor_valves)[] = *reinterpret_cast<ValveDriver(*)[]>(odor_valves_start);

// Pass valves into the poke manager constructor
PokeManager poke_manager(final_valve, odor_valves, NUM_ODOR_VALVES);

// Select Cam pin for the CAM0  DRIVER constuctor
CameraDriver cam0_driver(CAM0_TRIGGER_PIN, pio0, 0);

// Select Cam pin for the CAM1  DRIVER constuctor
CameraDriver cam1_driver(CAM1_TRIGGER_PIN, pio1, 0);

// Construct Flow Detection Object
FlowDetection flow_detection(MAX_ADC_CHS);

// Construct Proportional Valve Controller
ValveDriver& proportional_valve_0 = valve_drivers[PROPORTIONAL_VALVE_0_INDEX]; 
ProportionalValveControl proportional_valve_0_controller (proportional_valve_0, PROPORTIONAL_VALVE_0_ADC_INDEX);

ValveDriver& proportional_valve_1 = valve_drivers[PROPORTIONAL_VALVE_1_INDEX]; 
ProportionalValveControl proportional_valve_1_controller (proportional_valve_1, PROPORTIONAL_VALVE_1_ADC_INDEX);

ValveDriver& proportional_valve_2 = valve_drivers[PROPORTIONAL_VALVE_2_INDEX]; 
ProportionalValveControl proportional_valve_2_controller (proportional_valve_2, PROPORTIONAL_VALVE_2_ADC_INDEX);

// Core0 main.
int main()
{
    // Init Synchronizer.
    HarpSynchronizer::init(uart1, HARP_SYNC_RX_PIN);
    app.set_synchronizer(&HarpSynchronizer::instance());
#ifdef DEBUG
    stdio_uart_init_full(uart0, 921600, UART_TX_PIN, -1); // use uart1 tx only.
    printf("Hello, from an RP2040!\r\n");
#endif
    reset_app();
    while(true)
        app.run();
}
