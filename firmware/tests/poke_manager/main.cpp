#include <cstring>
#include <cstdint>
#include <config.h>
#include <etl/vector.h>
#include <poke_manager.h>
#include <valve_driver.h>
#include <pico/stdlib.h> // for uart printing
#include <cstdio> // for printf


ValveDriver valve_drivers[NUM_VALVES]
{{VALVE_PIN_BASE},
 {VALVE_PIN_BASE + 1},
 {VALVE_PIN_BASE + 2},
 {VALVE_PIN_BASE + 3},
 {VALVE_PIN_BASE + 4},
 {VALVE_PIN_BASE + 5},
 {VALVE_PIN_BASE + 6},
 {VALVE_PIN_BASE + 7},
 {VALVE_PIN_BASE + 8},
 {VALVE_PIN_BASE + 9},
 {VALVE_PIN_BASE + 10},
 {VALVE_PIN_BASE + 11},
 {VALVE_PIN_BASE + 12},
 {VALVE_PIN_BASE + 13},
 {VALVE_PIN_BASE + 14},
 {VALVE_PIN_BASE + 15}};

ValveDriver& final_valve = valve_drivers[0]; // add to config
ValveDriver& vac_valve = valve_drivers[1];
// Consider the rest of the valves as odor delivery valves.
ValveDriver* odor_valves_start = valve_drivers + 2;
ValveDriver (&odor_valves)[] = *reinterpret_cast<ValveDriver(*)[]>(odor_valves_start);

// Pass valves into the poke manager constructor
PokeManager poke_manager(final_valve, vac_valve, odor_valves, NUM_ODOR_VALVES);


// LED and Poke Port
const uint LED_PIN = 2;//25;
const uint POKE_PIN = 22; //GPIO pin for pokes


void request_next_odor()
{
    printf("Next odor, please!\r\n");
}

// Core0 main.
int main()
{
    gpio_init(LED_PIN);
    gpio_set_dir(LED_PIN, GPIO_OUT);

    stdio_usb_init();
    stdio_set_translate_crlf(&stdio_usb, false); // Don't replace outgoing chars.
    while (!stdio_usb_connected()){} // Block until connection to serial port.
    printf("Hello, from an RP2040!\r\n");
    poke_manager.set_poke_pin(POKE_PIN);
    poke_manager.set_poke_pin_override_state(GPIO_OVERRIDE_INVERT);
    poke_manager.set_next_odor_callback_fn(request_next_odor);
    poke_manager.set_enabled_state(true);
    while(true){
        poke_manager.update(); // update through FSM
    }
}
