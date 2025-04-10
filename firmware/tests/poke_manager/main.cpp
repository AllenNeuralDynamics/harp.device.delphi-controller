#include <cstring>
#include <config.h>
#include <poke_manager.h>
#include <pico/stdlib.h> // for uart printing
#include <cstdio> // for printf

PokeManager poke_manager;

// Core0 main.
int main()
{
    //Debug UART setup
    stdio_uart_init_full(uart0, 921600, UART_TX_PIN, -1); // use uart1 tx only.
    printf("Hello, from an RP2040!\r\n");

    while(true)
        poke_manager.update();
}
