
# == DO NOT EDIT THE FOLLOWING LINES for the Raspberry Pi Pico VS Code Extension to work ==
if(WIN32)
    set(USERHOME $ENV{USERPROFILE})
else()
    set(USERHOME $ENV{HOME})
endif()
set(sdkVersion 2.2.0)
set(toolchainVersion 14_2_Rel1)
set(picotoolVersion 2.2.0)
set(picoVscode ${USERHOME}/.pico-sdk/cmake/pico-vscode.cmake)
if (EXISTS ${picoVscode})
    include(${picoVscode})
endif()
# ====================================================================================

set(PICO_BOARD pico CACHE STRING "Board type")

cmake_minimum_required(VERSION 3.13)

# --------------------------------------------------------------------
# Force GNU mode (fixes IntelliSense uint16_t issue)
# --------------------------------------------------------------------
set(CMAKE_C_STANDARD 17)
set(CMAKE_C_STANDARD_REQUIRED ON)
set(CMAKE_C_EXTENSIONS ON)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS ON)
# --------------------------------------------------------------------

find_package(Git REQUIRED)
execute_process(
    COMMAND "${GIT_EXECUTABLE}" rev-parse --short HEAD
    OUTPUT_VARIABLE COMMIT_ID
    OUTPUT_STRIP_TRAILING_WHITESPACE
)
message(STATUS "Computed Git Hash: ${COMMIT_ID}")
add_definitions(-DGIT_HASH="${COMMIT_ID}") # Usable in source code.

#add_definitions(-DDEBUG) # Uncomment for debugging
add_definitions(-DUSBD_MANUFACTURER="Allen Institute")
add_definitions(-DUSBD_PRODUCT="delphi-controller")

# PICO_SDK_PATH must be defined.
include(${PICO_SDK_PATH}/pico_sdk_init.cmake)

project(delphi-controller)

pico_sdk_init()

add_subdirectory(lib/harp.core.rp2040/firmware)
add_subdirectory(lib/rp2040.pwm)
add_subdirectory(lib/etl build/etl)

add_executable(${PROJECT_NAME}
    src/main.cpp
    src/delphi_controller_app.cpp
)

add_library(valve_driver
    src/valve_driver.cpp
)

add_library(flow_detection
    src/flow_detection.cpp
)

add_library(poke_manager
    src/poke_manager.cpp
)

add_library(pwm_pio
    src/pwm_pio.cpp
)

add_library(proportional_valve_control
    src/proportional_valve_control.cpp
)

include_directories(inc)

target_link_libraries(flow_detection pico_stdlib hardware_spi hardware_dma hardware_irq)
target_link_libraries(valve_driver rp2040_pwm pico_stdlib)
target_link_libraries(poke_manager pico_stdlib valve_driver etl::etl)
target_link_libraries(pwm_pio hardware_pio pico_stdlib)
target_link_libraries(proportional_valve_control valve_driver pico_stdlib)

target_link_libraries(${PROJECT_NAME}
    harp_core harp_c_app rp2040_pwm poke_manager
    hardware_pio hardware_gpio pwm_pio valve_driver
    harp_sync flow_detection proportional_valve_control
    pico_stdlib etl::etl
)

if(DEBUG)
    message(WARNING "Debug printf() messages from harp core to UART with baud rate 921600.")
    pico_enable_stdio_uart(${PROJECT_NAME} 1)
    pico_enable_stdio_uart(poke_manager 1)
    pico_enable_stdio_uart(pwm_pio 1)
    pico_enable_stdio_uart(proportional_valve_control 1)
endif()

pico_add_extra_outputs(${PROJECT_NAME})
