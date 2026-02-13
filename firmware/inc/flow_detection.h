// Flow detection class 
#include <cstdio>
#include <cmath>
#include "pico/stdlib.h"
#include "hardware/adc.h"
#include "hardware/dma.h"
#include "hardware/irq.h"
#include "hardware/clocks.h"
#include "etl/vector.h"
#include <config.h>

class FlowDetection
{
public:

/**
 * \brief constructor.
 * \param adc_mask ADC pins connected to flow meters.
 * \param num_adc_chs Number of ADC channels.
 */
    FlowDetection(uint8_t adc_mask, uint8_t num_adc_chs);  // Specify a mask for determining which ADC pins are connected to flow meters.

/**
 * \brief destructor.
 */
    ~FlowDetection();

/**
 * \brief reset to flow detection.
 * \details PWM freqency will be set to zero until fps is specified
 */
    void reset();

/**
 * \brief Call periodically in a loop to sample ADCs
 */
    void update();

/**
 * \brief Initialize ADCs
 */
    void prepare_adc_pins();

/**
 * \brief Collect ADC samples
 */
    void sample_adc();

/**
 * \brief Process ADC samples
 */
    void process_adc_samples();

 /**
 * \brief Clear recent ADC sample.
 */   
    void clear_latest_sample();

/**
 * \brief DMA setup
 */
    void setup_dma();

/**
 * \brief DMA round robin between ADC channels
 */
    void setup_round_robin();

/**
 * \brief Configure the sampling rate in Hz.
 */
    void configure_sampling_rate_hz(float sampling_rate_hz);
    
// Register writes
/**
 * \brief Configure the sampling rate in Hz.
 */
    inline void set_sampling_rate_hz(float adc_sampling_rate)
    {
        adc_sample_rate_ = adc_sampling_rate;
        configure_sampling_rate_hz(adc_sample_rate_);  //reconfigure the ADC clock divider to achieve new sampling rate
    }

/**
 * \brief Set leak threshold as a fraction of full scale (e.g., 0.75 for 75% of max flow)
 */
    inline void set_leak_threshold(float leak_threshold)
    {
        leak_threshold_ = leak_threshold;
    }

/**
 * \brief Set the ADC that will detect leaks (if applicable)
 */
    inline void set_leak_adc(int8_t leak_adc)
    {
        leak_adc_ = leak_adc;
    }

/**
 * \brief Set sampling enabled or disabled. When disabled, latest sample will be cleared and ADC will not be sampled.
 */
    inline void set_sampling_enabled(bool enabled)
    {
        sampling_enabled_ = enabled;
        if (!enabled) {
            clear_latest_sample();
            adc_run(false); // stop ADC if sampling is disabled
            adc_fifo_drain(); // drain adc FIFO to clear out any pending samples

            
            // clear DMA pending interrupt
            if (dma_chan_ >= 0) {
                dma_channel_abort(dma_chan_);
                const uint32_t mask = (1u << dma_chan_);
                if (dma_hw->ints0 & mask) {
                    dma_hw->ints0 = mask;   // ack stale IRQ
                }
            }
            dma_complete_ = false; // reset DMA complete flag
        }
        else {
            dma_complete_ = false;     // Reset before starting
            adc_fifo_drain();          // FIFO must be empty before arming DMA

            // Start DMA if not already running; this will kick off ADC sampling as well since DMA is paced by ADC FIFO DREQ.
            if (!dma_channel_is_busy(dma_chan_)) {
                sample_adc();
            }
        }
    }

// Register reads
/**
 * \brief Latest ADC sample (timestamp + volts for each channel)
 */

     //  Struct to store ADC samples
    #pragma pack(push, 1)
    struct ADC_Samples {
    float    v[NUM_ADC_PINS];   // volts for ADC0..3 (GPIO26..29)
    };
    #pragma pack(pop)

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


private:

    // Declare data members
    uint8_t adc_mask_;
    int8_t leak_adc_;
    uint8_t num_adc_chs_;
    float adc_sample_rate_;
    float leak_threshold_;
    bool sampling_enabled_;
    int dma_chan_;
    bool dma_complete_;
    ADC_Samples latest_adc_sample_;
    
    // DMA handler setup
    static FlowDetection* s_instance_;       // holds the active instance
    static void __isr dma_irq_trampoline();  // C-compatible ISR
    void dma_irq_handler();                  // r

    // Declare Constants
    static inline constexpr float DEFAULT_SAMPLE_RATE = 100.0f; // Sample at 100 Hz
    static inline constexpr float DEFAULT_LEAK_THRESHOLD = 0.0f; // 75% of max flow
    // static inline constexpr float ADC_CLKDIV_SLOWDOWN = 2000.0f; // tune as needed for more/less settling time
    static inline constexpr float VREF_VOLTS = 3.3f; // ADC Vref ~3.3V.
    static inline constexpr int  SAMPLES_PER_CHANNEL = 2;; // 1 dummy + 1 kept conversion per channel
    static inline constexpr int  SAMPLES_PER_PERIOD = NUM_ADC_PINS * SAMPLES_PER_CHANNEL;; // 1 dummy + 1 kept conversion per channel

    // DMA buffer
    uint16_t dma_buf_[SAMPLES_PER_PERIOD];

    // OPTIONAL: Circular buffer of ADC samples; can be used for more complex processing or debugging. Not needed if only latest sample is relevant.
    // static inline constexpr uint32_t SAMPLE_CAPACITY  = 1024; // ETL Buffer capacity.
    // etl::vector<ADC_Samples, SAMPLE_CAPACITY> adc_samples_; // Circular buffer of samples.
    // uint32_t sample_capacity_;

    // Convert Bits to Volts
    inline float bits_to_volts(uint16_t bits) {
        return (bits & 0x0FFF) * (VREF_VOLTS / 4096.0f);
    }
};





