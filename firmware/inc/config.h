#ifndef CONFIG_H
#define CONFIG_H

#define NUM_VALVES (16)
#define NUM_ODOR_VALVES (12)
#define FINAL_VALVE_INDEX (3)
#define ODOR_VALVE_INDEX_START (4)
#define PROPORTIONAL_VALVE_0_INDEX (0)
#define PROPORTIONAL_VALVE_1_INDEX (1)
#define PROPORTIONAL_VALVE_2_INDEX (2)
#define PROPORTIONAL_VALVE_0_ADC_INDEX (0)
#define PROPORTIONAL_VALVE_1_ADC_INDEX (1)
#define PROPORTIONAL_VALVE_2_ADC_INDEX (2)
#define POKE_PIN (22)
#define CAM0_TRIGGER_PIN (23)
#define CAM1_TRIGGER_PIN (24)
#define MAX_ADC_CHS (8)  // Number of ADCs that can be sampled
#define LED_ENABLE_PIN (4)

#define PIN_SCK   (26)  // SPI wiring to GPIOs
#define PIN_MOSI  (27)
#define PIN_MISO  (28)
#define PIN_CS    (29)

#define UART_TX_PIN (0)
#define HARP_SYNC_RX_PIN (5)
#define HARP_CORE_LED_PIN (2)

inline constexpr uint32_t VALVE_PIN_BASE = 6;
inline constexpr uint32_t GPIO_PIN_BASE = 22;

#define VALVES_MASK (0x0000FFFF)
#define GPIOS_MASK (0x0000000F) // Was 0x000000FF for 8 GPIOs

#define HARP_DEVICE_ID (1409)
#define HW_VERSION_MAJOR (1)
#define HW_VERSION_MINOR (0)
#define HW_ASSEMBLY_VERSION (0)

#define FW_VERSION_MAJOR (0)
#define FW_VERSION_MINOR (0)
#define FW_VERSION_PATCH (0)

#define UNUSED_SERIAL_NUMBER (0)  // Deprecated in favor of R_UUID

#endif // CONFIG_H
