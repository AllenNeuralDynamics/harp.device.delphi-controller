# == DO NOT EDIT THE FOLLOWING LINES for the Raspberry Pi Pico VS Code Extension to work ==
if(WIN32)
    set(USERHOME $ENV{USERPROFILE})
else()
    set(USERHOME $ENV{HOME})
endif()
set(sdkVersion 2.1.1)
set(toolchainVersion 14_2_Rel1)
set(picotoolVersion 2.1.1)
set(picoVscode ${USERHOME}/.pico-sdk/cmake/pico-vscode.cmake)
if (EXISTS ${picoVscode})
    include(${picoVscode})
endif()
# ====================================================================================
set(PICO_BOARD pico CACHE STRING "Board type")

cmake_minimum_required(VERSION 3.13)
find_package(Git REQUIRED)
execute_process(COMMAND "${GIT_EXECUTABLE}" rev-parse --short HEAD OUTPUT_VARIABLE COMMIT_ID OUTPUT_STRIP_TRAILING_WHITESPACE)
message(STATUS "Computed Git Hash: ${COMMIT_ID}")
add_definitions(-DGIT_HASH="${COMMIT_ID}") # Usable in source code.

#add_definitions(-DDEBUG) # Uncomment for debugging
add_definitions(-DUSBD_MANUFACTURER="Allen Institute")
add_definitions(-DUSBD_PRODUCT="test-poke_manager")

# PICO_SDK_PATH must be defined.
include(${PICO_SDK_PATH}/pico_sdk_init.cmake)

# Use modern conventions like std::invoke
set(CMAKE_CXX_STANDARD 17)

project(poke-manager)

pico_sdk_init()

add_executable(${PROJECT_NAME}
    main.cpp
)

add_library(poke_manager
    ../../src/poke_manager.cpp
)

include_directories(../../inc)

target_link_libraries(poke_manager 
                        pico_stdlib)
target_link_libraries(${PROJECT_NAME}
                      poke_manager pico_stdlib)

pico_add_extra_outputs(${PROJECT_NAME})

message(WARNING "Debug printf() messages from harp core to UART with baud \
        rate 921600.")
pico_enable_stdio_uart(${PROJECT_NAME} 1) # UART stdio for printf.
pico_enable_stdio_uart(poke_manager 1) # UART stdio for printf.
# Additional libraries need to have stdio init also.


