#include <cstring>
#include <config.h>
#include <etl/vector.h>
#include <poke_manager.h>
#include <valve_driver.h>
#include <pico/stdlib.h> // for uart printing
#include <cstdio> // for printf


// Inititalize final and vac valve pins
ValveDriver final_valve(1);
ValveDriver vac_valve(2);
etl::vector<ValveDriver, NUM_ODOR_VALVES> odor_valves;

// Pass valves into the poke manager constructor
PokeManager poke_manager(final_valve, vac_valve, odor_valves);

// Core0 main.
int main()
{ 
    // Odor valves vector
    
    for (int i = 4; i < 4 + NUM_ODOR_VALVES; i++){
        odor_valves.emplace_back(i);
    }

    stdio_usb_init();
    stdio_set_translate_crlf(&stdio_usb, false); // Don't replace outgoing chars.
    while (!stdio_usb_connected()){} // Block until connection to serial port.
    printf("Hello, from an RP2040!\r\n");
    poke_manager.poke();
    while(true)
        poke_manager.update();
}
