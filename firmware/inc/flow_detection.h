// Flow detection class spi
#pragma once

#include <cstdio>
#include <cmath>
#include <cstdint>

#include "pico/stdlib.h"
#include "hardware/spi.h"
#include "hardware/dma.h"
#include "hardware/irq.h"

#include "config.h"

#define NUM_ADC_CHS (__builtin_popcount(ADC_MASK))

class FlowDetection
{
public:
    #pragma pack(push, 1)
    struct ADC_Samples {
        float v[NUM_ADC_CHS];
    };
    #pragma pack(pop)

    FlowDetection(uint8_t adc_mask);
    ~FlowDetection();

    void reset();
    void update();

    void prepare_adc_pins();
    void sample_adc();
    void clear_latest_sample();

    void setup_dma();
    void leak_monitor();
    void manual_flow_meter_monitor();

    // Delete this
    inline void set_sampling_rate_hz(float adc_sampling_rate)
    {
        adc_sample_rate_ = adc_sampling_rate;
    }

    inline void set_leak_threshold(float leak_threshold)
    {
        leak_threshold_ = leak_threshold;
    }

    inline void set_leak_adc(int8_t leak_adc)
    {
        leak_adc_ = leak_adc;
    }

    inline void set_manual_flow_meter(int8_t manual_flow_meter)
    {
        manual_flow_meter_ = manual_flow_meter;
    }

    inline void set_nominal_flow_rate(float nominal_flow_rate)
    {
        nominal_flow_rate_ = nominal_flow_rate;
    }

    inline void set_flow_rate_tolerance(float flow_rate_tolerance)
    {
        flow_rate_tolerance_ = flow_rate_tolerance;
    }

    inline void set_calibrate_slope(float calibrate_slope)
    {
        conversion_slope_ = calibrate_slope;
    }

    inline void set_calibrate_offset(float calibrate_offset)
    {
        conversion_offset_ = calibrate_offset;
    }

    // Register reads
    inline ADC_Samples get_latest_adc_sample() const
    {
        return latest_adc_sample_;
    }

    inline bool get_adc_enabled_status() const
    {
        return sampling_enabled_;
    }

    inline float get_adc_sampling_rate() const
    {
        return adc_sample_rate_;
    }

    inline int8_t get_leak_adc() const
    {
        return leak_adc_;
    }

    inline float get_leak_threshold() const
    {
        return leak_threshold_;
    }

    inline uint8_t get_leak_state() const
    {
        return leak_state_;
    }

    inline int8_t get_manual_flow_meter() const
    {
        return manual_flow_meter_;
    }

    inline float get_nominal_flow_rate() const
    {
        return nominal_flow_rate_;
    }

    inline float get_flow_rate_tolerance() const
    {
        return flow_rate_tolerance_;
    }

    inline uint8_t get_manual_flow_meter_state() const
    {
        return manual_flow_meter_state_;
    }

    inline float get_calibrate_slope() const
    {
        return conversion_slope_;
    }

    inline float get_calibrate_offset() const
    {
        return conversion_offset_;
    }

    inline ADC_Samples get_latest_flow_measurements() const
    {
        return latest_adc_sample_;
    }

    // Alerts
    inline void leak_state_alert_callback_fn(void (*fn)(void))
    {
        leak_state_alert_callback_fn_ = fn;
    }

    inline void leak_state_alert()
    {
        if (leak_state_alert_callback_fn_)
            leak_state_alert_callback_fn_();
    }

    inline void manual_flow_meter_alert_callback_fn(void (*fn)(void))
    {
        manual_flow_meter_alert_callback_fn_ = fn;
    }

    inline void manual_flow_meter_alert()
    {
        if (manual_flow_meter_alert_callback_fn_)
            manual_flow_meter_alert_callback_fn_();
    }

    inline void set_sampling_enabled(bool enabled)
    {
        sampling_enabled_ = enabled;

        if (!enabled) {
            // Stop any in-flight DMA transfers
            if (dma_tx_chan_ >= 0) {
                dma_channel_abort(dma_tx_chan_);
            }
            if (dma_rx_chan_ >= 0) {
                dma_channel_abort(dma_rx_chan_);
            }

            // Acknowledge any pending RX DMA IRQ
            if (dma_rx_chan_ >= 0) {
                const uint32_t mask = (1u << dma_rx_chan_);
                if (dma_hw->ints0 & mask) {
                    dma_hw->ints0 = mask;
                }
            }

            // Deassert CS to leave ADC in idle state
            gpio_put(PIN_CS, 1);

            // Clear data and state
            dma_complete_   = false;
            current_channel_ = 0;
            clear_latest_sample();
        }
        else {
            // Reset state before restarting sampling
            dma_complete_ = false;
            current_channel_ = 0;

            // Kick off the first SPI conversion if idle
            if (!dma_channel_is_busy(dma_rx_chan_)) {
                sample_adc();
            }
        }
    }

private:
    uint8_t adc_mask_;
    uint8_t num_adc_chs_;

    float adc_sample_rate_;
    bool sampling_enabled_;

    int8_t leak_adc_;
    float leak_threshold_;
    uint8_t leak_state_;

    int8_t manual_flow_meter_;
    float nominal_flow_rate_;
    float flow_rate_tolerance_;
    uint8_t manual_flow_meter_state_;

    float conversion_slope_;
    float conversion_offset_;

    uint8_t tx_buf_[3];
    uint8_t rx_buf_[3];

    int dma_tx_chan_;
    int dma_rx_chan_;
    volatile bool dma_complete_;
    volatile uint8_t current_channel_;

    ADC_Samples latest_adc_sample_;
    ADC_Samples latest_flow_measurements_;

    void (*leak_state_alert_callback_fn_)(void);
    void (*manual_flow_meter_alert_callback_fn_)(void);

    static FlowDetection* s_instance_;
    static void __isr dma_irq_trampoline();
    void dma_irq_handler();

    static inline spi_inst_t* SPI_PORT = spi1;
    static inline constexpr float ADC_BITS = 1023.0f;
    static inline constexpr float VREF_VOLTS = 3.3f;
    static inline constexpr float DEFAULT_SAMPLE_RATE = 100.0f;
    static inline constexpr float DEFAULT_LEAK_THRESHOLD = 50.0f;
    static inline constexpr float DEFAULT_FLOW_RATE = 75.0f;
    static inline constexpr float FLOW_RATE_TOLERANCE = 5.0f; // ! +-5mL/min tolerance for flow rate detection (tune as needed)

    static inline constexpr float VOLTS_FLOW_RATE_SLOPE = 0.02f;
    static inline constexpr float VOLTS_FLOW_RATE_OFFSET = 0.5f;

    inline float convert_to_flowrate(uint16_t bits) const
    {
        float volts = bits * (VREF_VOLTS / ADC_BITS);
        return (volts - conversion_offset_) / conversion_slope_;
    }
};