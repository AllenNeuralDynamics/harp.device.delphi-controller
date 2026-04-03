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


class FlowDetection
{
public:
    FlowDetection(uint8_t max_adc_chs);
    ~FlowDetection();

    void reset();
    void update();

    void prepare_adc_pins();
    void sample_adc();
    void clear_latest_sample();
    void configure_adc_mask(uint8_t adc_mask);

    void setup_dma();
    void leak_monitor();
    void manual_flow_meter_monitor();

    // register writes
    inline void set_adc_mask(uint8_t adc_mask)
    {
        configure_adc_mask(adc_mask);
    }

    // flow meter configuration struct and related constants
    static inline constexpr uint8_t NUM_FLOW_METERS = MAX_ADC_CHS;
    static inline constexpr uint8_t NUMBER_OF_COEFFICIENTS = 4;

    #pragma pack(push, 1)
    struct FlowMeterConfig {
        float q[NUMBER_OF_COEFFICIENTS];   // a0 + a1*V + a2*V^2 + a3*V^3
    };
    #pragma pack(pop)

    #pragma pack(push, 1)
    struct FlowMeterRegisterBlock
    {
        FlowMeterConfig meter[NUM_FLOW_METERS];
    };
    #pragma pack(pop)

    inline void set_flow_meter_calibrations(const FlowMeterRegisterBlock& regs)
    {
        // Optional: disable sampling for atomic update
        bool was_enabled = sampling_enabled_;
        sampling_enabled_ = false;

        flow_regs_ = regs;   // single struct copy (fast, safe)
        
        sampling_enabled_ = was_enabled;
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


    // Register reads
    // ADC struct
    #pragma pack(push, 1)
    struct ADC_Samples {
        float v[MAX_ADC_CHS];
    };
    #pragma pack(pop)
    
    inline int8_t get_adc_mask() const
    {
        return adc_mask_;
    }

    inline const ADC_Samples& get_latest_adc_sample() const
    {
        return latest_adc_sample_;
    }

    inline const ADC_Samples& get_latest_raw_adc_sample() const
    {
        return latest_raw_adc_sample_;
    }

    inline const FlowMeterRegisterBlock& get_flow_meter_calibrations() const
    {
        return flow_regs_;
    }

    // remaining reads
    inline bool get_adc_enabled_status() const
    {
        return sampling_enabled_;
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
    uint8_t active_adc_count_;
    uint8_t active_adc_channels_[MAX_ADC_CHS];
    uint8_t max_adc_chs_;

    bool sampling_enabled_;

    int8_t leak_adc_;
    float leak_threshold_;
    uint8_t leak_state_;

    FlowMeterRegisterBlock flow_regs_;

    int8_t manual_flow_meter_;
    float nominal_flow_rate_;
    float flow_rate_tolerance_;
    uint8_t manual_flow_meter_state_;

    uint8_t tx_buf_[3];
    uint8_t rx_buf_[3];

    int dma_tx_chan_;
    int dma_rx_chan_;
    volatile bool dma_complete_;
    volatile uint8_t current_channel_;

    ADC_Samples latest_adc_sample_;
    ADC_Samples latest_raw_adc_sample_;

    void (*leak_state_alert_callback_fn_)(void);
    void (*manual_flow_meter_alert_callback_fn_)(void);

    static FlowDetection* s_instance_;
    static void __isr dma_irq_trampoline();
    void dma_irq_handler();

    static inline spi_inst_t* SPI_PORT = spi1;
    static inline constexpr uint8_t DEFAULT_ADC_MASK = 0x1F;
    static inline constexpr float ADC_BITS = 4095.0f;
    static inline constexpr float VREF_VOLTS = 3.3f;
    static inline constexpr float DEFAULT_LEAK_THRESHOLD = 50.0f;
    static inline constexpr float DEFAULT_FLOW_RATE = 75.0f;
    static inline constexpr float FLOW_RATE_TOLERANCE = 5.0f; // ! +-5mL/min tolerance for flow rate detection (tune as needed)

    // default coefficients for converting ADC volts to flow rate (these are just placeholders and should be calibrated for the specific flow meter)
    static inline constexpr float DEFAULT_A0 = -0.025f;
    static inline constexpr float DEFAULT_A1 = 0.05f;
    static inline constexpr float DEFAULT_A2 = 0.0f;
    static inline constexpr float DEFAULT_A3 = 0.0f;

    // flow meter conversion using cubic polynomial with coefficients from flow_regs_
    static inline float eval_cubic(const float *c, float v)
    {
        // Horner's method
        return ((c[3]*v + c[2])*v + c[1])*v + c[0];
    }

};