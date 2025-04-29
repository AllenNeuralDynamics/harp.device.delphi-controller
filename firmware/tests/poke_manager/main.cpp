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

// LED an Poke Port
const uint LED_PIN = 25;
const uint POKE_PIN = 0; //GPIO pin for pokes
bool beam_broken = false; //keep track of beam state
bool poke_initiated_once = false; //Only trigger the FSM on 1 poke
uint32_t poke_start_time_us; //poke start time
static inline constexpr uint32_t MIN_POKE_TIME_US = 1e6; //poke duration - 1s 

// Core0 main.
int main()
{ 
    // setup AUX GPIO pins and convert into pins
    gpio_init_mask(GPIOS_MASK << GPIO_PIN_BASE); //pico sdk function
    gpio_set_dir_masked(GPIOS_MASK << GPIO_PIN_BASE, 0); //0: input, 1: output

    // Initialize one input pin for the poke port
    // gpio_init(POKE_PIN );
    // gpio_set_dir(POKE_PIN , 0);

    gpio_init(LED_PIN);
    gpio_set_dir(LED_PIN, GPIO_OUT);

    // Odor valves vector -- set valves
    for (int i = 4; i < 4 + NUM_ODOR_VALVES; i++){
        odor_valves.emplace_back(i);
    }

    stdio_usb_init();
    stdio_set_translate_crlf(&stdio_usb, false); // Don't replace outgoing chars.
    while (!stdio_usb_connected()){} // Block until connection to serial port.
    printf("Hello, from an RP2040!\r\n");
    while(true){
        poke_manager.update(); // update through FSM
        
        // Beam is no longer broken
        if (gpio_get(POKE_PIN) == 1){
            gpio_put(LED_PIN, 0);
            beam_broken == false;
            poke_start_time_us = time_us_32();
            poke_initiated_once = false;
        }

        // Poke detected -- start poke timer
        if (gpio_get(POKE_PIN) == 0 && beam_broken == false){
            poke_start_time_us = time_us_32();
            beam_broken = true;
        }
            
        // Check duration since beam break/poke
        if (gpio_get(POKE_PIN) == 0 && beam_broken == true){
            gpio_put(LED_PIN, 1); // Turn on LED whenever the beam is broken
            if ((time_us_32() - poke_start_time_us) >= MIN_POKE_TIME_US && poke_initiated_once == false){
                
                //Poke was detected!
                poke_manager.poke();

                //Account for the successful poke so that another doesn't occur on the same poke 
                poke_initiated_once = true;
            }   
        }
    }
}
