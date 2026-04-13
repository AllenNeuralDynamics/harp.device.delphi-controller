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
    void setup_dma();
    void teardown_dma();
    void prime_adc();

    void clear_latest_sample();
    void leak_monitor();
    void manual_flow_meter_monitor();

    // flow meter configuration
    static inline constexpr uint8_t NUM_FLOW_METERS = MAX_ADC_CHS;
    static inline constexpr uint8_t NUMBER_OF_COEFFICIENTS = 4;

    struct FlowMeterConfig {
        float q[NUMBER_OF_COEFFICIENTS];
    };

    struct FlowMeterRegisterBlock
    {
        FlowMeterConfig meter[NUM_FLOW_METERS];
    };

    static inline void drain_spi_fifo()
    {
        spi_hw_t* hw = spi_get_hw(SPI_PORT);
        while (hw->sr & SPI_SSPSR_RNE_BITS) {
            (void)hw->dr;
        }
        hw->icr = SPI_SSPICR_RORIC_BITS;
    }

    inline void set_flow_meter_calibrations(const FlowMeterRegisterBlock& regs)
    {
        bool was_enabled = sampling_enabled_;
        sampling_enabled_ = false;
        flow_regs_ = regs;
        sampling_enabled_ = was_enabled;
    }

    inline const FlowMeterRegisterBlock& get_flow_meter_calibrations() const
    {
        return flow_regs_;
    }

    inline void set_leak_adc(int8_t ch)
    {
        leak_adc_ = ch;
        leak_state_ = 0;
    }

    inline int8_t get_leak_adc() const
    {
        return leak_adc_;
    }

    inline void set_leak_threshold(float t)
    {
        leak_threshold_ = t;
    }

    inline float get_leak_threshold() const
    {
        return leak_threshold_;
    }

    inline void set_manual_flow_meter(int8_t ch)
    {
        manual_flow_meter_ = ch;
        manual_flow_meter_state_ = 0;
    }

    inline int8_t get_manual_flow_meter() const
    {
        return manual_flow_meter_;
    }

    inline void set_nominal_flow_rate(float r)
    {
        nominal_flow_rate_ = r;
    }

    inline float get_nominal_flow_rate() const
    {
        return nominal_flow_rate_;
    }

    inline void set_flow_rate_tolerance(float t)
    {
        flow_rate_tolerance_ = t;
    }

    inline float get_flow_rate_tolerance() const
    {
        return flow_rate_tolerance_;
    }

    #pragma pack(push, 1)
    struct ADC_Samples {
        float v[NUM_FLOW_METERS];
    };
    #pragma pack(pop)

    #pragma pack(push, 1)
    struct ADC_Samples_Raw {
        uint16_t z[NUM_FLOW_METERS];
    };
    #pragma pack(pop)

    inline const ADC_Samples get_latest_adc_sample() const
    {
        return latest_adc_sample_;
    }

    inline const ADC_Samples_Raw get_latest_raw_adc_sample() const
    {
        return latest_raw_adc_sample_;
    }

    inline bool get_adc_enabled_status() const
    {
        return sampling_enabled_;
    }

    inline uint8_t get_leak_state() const
    {
        return leak_state_;
    }

    inline uint8_t get_manual_flow_meter_state() const
    {
        return manual_flow_meter_state_;
    }

    inline void leak_state_alert_callback_fn(void (*fn)(void))
    {
        leak_state_alert_callback_fn_ = fn;
    }

    inline void manual_flow_meter_alert_callback_fn(void (*fn)(void))
    {
        manual_flow_meter_alert_callback_fn_ = fn;
    }

    inline void set_sampling_enabled(bool enabled)
    {
        sampling_enabled_ = enabled;
    }

private:
    void sample_adc();
    void process_latest_sample();

    static void __isr dma_irq_trampoline();
    void dma_irq_handler();

private:
    uint8_t max_adc_chs_;
    uint8_t current_channel_;

    bool sampling_enabled_;
    volatile bool dma_complete_;
    bool ready_for_next_sample_;

    int dma_tx_chan_;
    int dma_rx_chan_;

    uint8_t tx_buf_[3];
    uint8_t rx_buf_[3];

    ADC_Samples     latest_adc_sample_;
    ADC_Samples_Raw latest_raw_adc_sample_;

    FlowMeterRegisterBlock flow_regs_;

    int8_t leak_adc_;
    float leak_threshold_;
    uint8_t leak_state_;

    int8_t manual_flow_meter_;
    float nominal_flow_rate_;
    float flow_rate_tolerance_;
    uint8_t manual_flow_meter_state_;

    void (*leak_state_alert_callback_fn_)(void);
    void (*manual_flow_meter_alert_callback_fn_)(void);

    static FlowDetection* s_instance_;

    static inline spi_inst_t* SPI_PORT = spi1;
    static inline constexpr float ADC_BITS = 4095.0f;
    static inline constexpr float VREF_VOLTS = 3.3f;
    static inline constexpr float DEFAULT_LEAK_THRESHOLD = 50.0f;
    static inline constexpr float DEFAULT_FLOW_RATE = 75.0f;
    static inline constexpr float FLOW_RATE_TOLERANCE = 5.0f;

    static inline constexpr float DEFAULT_A0 = -25.0f;
    static inline constexpr float DEFAULT_A1 = 50.0f;
    static inline constexpr float DEFAULT_A2 = 0.0f;
    static inline constexpr float DEFAULT_A3 = 0.0f;

    static inline float eval_cubic(const float q[4], float x)
    {
        return ((q[3] * x + q[2]) * x + q[1]) * x + q[0];
    }
};