#include <cstring>
#include <config.h>
#include <poke_manager.h>
#include <pico/stdlib.h> // for uart printing
#include <cstdio> // for printf

PokeManager poke_manager;

// Core0 main.
int main()
{ 
    stdio_usb_init();
    stdio_set_translate_crlf(&stdio_usb, false); // Don't replace outgoing chars.
    while (!stdio_usb_connected()){} // Block until connection to serial port.
    printf("Hello, from an RP2040!\r\n");
    poke_manager.poke();
    while(true)
        poke_manager.update();

}
