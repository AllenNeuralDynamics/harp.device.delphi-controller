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

// Inititalize final and vac valve pins -- Does this go here?
ValveDriver& final_valve = valve_drivers[0]; // add to config
ValveDriver& vac_valve = valve_drivers[1];
ValveDriver (&odor_valves)[] = (&valve_drivers)[2]; // refer to rest of valve drivers as valves for odoor delivery
                                                // i.e: odor_valves[app_regs.NextOdor].energize(); // check out harp core

// Pass valves into the poke manager constructor
PokeManager poke_manager(final_valve, vac_valve, odor_valves, NUM_ODOR_VALVES);

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
