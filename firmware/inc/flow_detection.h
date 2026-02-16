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
 * \brief Detect and initiate events for leaks
 */
    void leak_monitor();

/**
 * \brief Detect and initiate events for manual flow meter
 */
    void manual_flow_meter_monitor();

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
 * \brief Manual flow meter monitoring
 */    
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
    float    v[NUM_ADC_PINS];   // flow rate for ADC0..3 (GPIO26..29)
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

    // Event Handlers
    inline void leak_state_alert_callback_fn( void (* fn)(void))
    {leak_state_alert_callback_fn_ = fn;}

    inline void leak_state_alert()
    {
        if (leak_state_alert_callback_fn_ != nullptr)
            leak_state_alert_callback_fn_();
    }

    inline void manual_flow_meter_alert_callback_fn( void (* fn)(void))
    {manual_flow_meter_alert_callback_fn_ = fn;}

    inline void manual_flow_meter_alert()
    {
        if (manual_flow_meter_alert_callback_fn_ != nullptr)
            manual_flow_meter_alert_callback_fn_();
    }

private:

    // Declare data members
    uint8_t adc_mask_;
    int8_t leak_adc_;
    uint8_t num_adc_chs_;
    float adc_sample_rate_;
    float leak_threshold_;
    uint8_t leak_state_;
    bool sampling_enabled_;
    int dma_chan_;
    bool dma_complete_;
    ADC_Samples latest_adc_sample_;

    int8_t manual_flow_meter_;
    float nominal_flow_rate_;
    float flow_rate_tolerance_;
    uint8_t manual_flow_meter_state_;
    float conversion_slope_;
    float conversion_offset_;

    void (*leak_state_alert_callback_fn_)(void);
    void (*manual_flow_meter_alert_callback_fn_)(void);
    
    // DMA handler setup
    static FlowDetection* s_instance_;       // holds the active instance
    static void __isr dma_irq_trampoline();  // C-compatible ISR
    void dma_irq_handler();                  // r

    // Declare Constants
    static inline constexpr float VOLTS_FLOW_RATE_SLOPE = 0.02f; // Conversion factor from volts to flow rate (tune as needed based on flow meter characteristics)
    static inline constexpr float VOLTS_FLOW_RATE_OFFSET = 0.5f; // Conversion factor from flow rate to volts (tune as needed based on flow meter characteristics)
    static inline constexpr float DEFAULT_SAMPLE_RATE = 100.0f; // Sample at 100 Hz
    static inline constexpr float DEFAULT_LEAK_THRESHOLD = 50.0f; // threshold for leaks in mL/min
    static inline constexpr float DEFAULT_FLOW_RATE = 75.0f; // nominal flow rate in mL/min (tune as needed)
    static inline constexpr float FLOW_RATE_TOLERANCE = 5.0f; // ! +-5mL/min tolerance for flow rate detection (tune as needed)
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
    inline float convert_to_flowrate(uint16_t bits) {
        float bits_to_volts = (bits & 0x0FFF) * (VREF_VOLTS / 4096.0f);
        float volts_to_flow_rate = (bits_to_volts - conversion_offset_) / conversion_slope_;
        return volts_to_flow_rate;
    }
};





