#include <flow_detection.h>

FlowDetection::FlowDetection(uint8_t adc_mask, uint8_t num_adc_chs): 
adc_mask_{adc_mask}, num_adc_chs_{num_adc_chs}, dma_chan_{-1}, dma_complete_{false},
adc_sample_rate_{DEFAULT_SAMPLE_RATE}, leak_threshold_{DEFAULT_LEAK_THRESHOLD}, 
sampling_enabled_{false}, leak_adc_{-1}, leak_state_{0}, leak_state_alert_callback_fn_{nullptr},
manual_flow_meter_{-1}, nominal_flow_rate_{DEFAULT_FLOW_RATE}, flow_rate_tolerance_{FLOW_RATE_TOLERANCE},
 manual_flow_meter_state_{0}, manual_flow_meter_alert_callback_fn_{nullptr}
{
    reset();
}

FlowDetection* FlowDetection::s_instance_ = nullptr;

void FlowDetection::setup_dma() {
    dma_chan_ = dma_claim_unused_channel(true);
    dma_channel_config c = dma_channel_get_default_config(dma_chan_);
    channel_config_set_transfer_data_size(&c, DMA_SIZE_16);
    channel_config_set_read_increment(&c, false);
    channel_config_set_write_increment(&c, true);
    channel_config_set_dreq(&c, DREQ_ADC);

    dma_channel_configure(
        dma_chan_, &c,
        dma_buf_, &adc_hw->fifo,
        SAMPLES_PER_PERIOD,
        false
    );

    s_instance_ = this; // register this instance
    dma_channel_set_irq0_enabled(dma_chan_, true);
    irq_set_exclusive_handler(DMA_IRQ_0, dma_irq_trampoline);
    irq_set_enabled(DMA_IRQ_0, true);
}

void __isr FlowDetection::dma_irq_trampoline() {
    if (s_instance_) s_instance_->dma_irq_handler();
}


void FlowDetection::dma_irq_handler() {
    uint32_t mask = 1u << dma_chan_;
    if (!(dma_hw->ints0 & mask)) return;

    dma_hw->ints0 = mask;    // ack this channel
    adc_run(false);
    adc_fifo_drain();
    dma_complete_ = true;
}

FlowDetection::~FlowDetection() {
    adc_run(false);
    adc_fifo_drain();
    if (dma_chan_ >= 0) {
        dma_channel_abort(dma_chan_);
        dma_channel_set_irq0_enabled(dma_chan_, false);
        irq_remove_handler(DMA_IRQ_0, dma_irq_trampoline); // remove the SAME trampoline
        dma_channel_cleanup(dma_chan_);
        dma_channel_unclaim(dma_chan_);
        dma_chan_ = -1;
    }
    if (s_instance_ == this) s_instance_ = nullptr;
    leak_state_ = 0; // reset leak state on destruction
    leak_adc_ = -1; // reset leak ADC on destruction
    manual_flow_meter_ = -1; // reset manual flow meter on destruction
    manual_flow_meter_state_ = 0; // reset manual flow meter state on destruction
}

    
// Functions to alter the FSM
void FlowDetection::reset()
{
    adc_sample_rate_ = DEFAULT_SAMPLE_RATE;
    dma_complete_ = false;
    sampling_enabled_ = false;
    leak_adc_ = -1;
    manual_flow_meter_ = -1;
    leak_state_ = 0;
    manual_flow_meter_state_ = 0;
    setup_round_robin();
    setup_dma();
    leak_state_alert_callback_fn_ = nullptr;
    manual_flow_meter_alert_callback_fn_ = nullptr;
}


void FlowDetection::clear_latest_sample() {
    for (int i = 0; i < num_adc_chs_; ++i) {
        latest_adc_sample_.v[i] = 0;  // Use 0 to indicate no valid sample
    }
}

// Initializae ADCs
void FlowDetection::prepare_adc_pins() {
    // Ensure adc is initialized before touching pins
    // (Call adc_init() in your setup before this function)
    for (int ch = 0; ch < NUM_ADC_PINS; ++ch) {
        if (adc_mask_ & (1u << ch)) {
            adc_gpio_init(ADC_PIN_START + ch); // 26 + ch
        }
    }
}


// Set sampling rate by adjusting ADC clock divider (ADC clock = sys_clk / clkdiv)
void FlowDetection::configure_sampling_rate_hz(float sampling_rate_hz) {
    const int num_ch = num_adc_chs_;
    // Guard
    if (num_ch <= 0 || sampling_rate_hz <= 0) return;

    // clkdiv = 48e6 / (96 * N * sampling_rate)
    float clkdiv = 48000000.0f / (96.0f * num_ch * sampling_rate_hz);

    // Reasonable bounds (ADC clock divider is 24.8 fixed-point; avoid <1.0 which would attempt >48 MHz)
    if (clkdiv < 1.0f) clkdiv = 1.0f;
    if (clkdiv > 65535.0f) clkdiv = 65535.0f;

    adc_set_clkdiv(clkdiv);
}

// Start one sample: arm DMA for 8 samples and kick the ADC.
// Order will be: ADC0,1,2,3  (dummy)  then  ADC0,1,2,3  (kept).
void FlowDetection::sample_adc() { 
    adc_run(false);
    adc_fifo_drain();
    adc_select_input(0);  // start point of the round-robin sequence (ADC0)

    // Arm DMA
    dma_channel_set_read_addr(dma_chan_, &adc_hw->fifo, false);
    dma_channel_set_write_addr(dma_chan_, dma_buf_, false);
    dma_channel_set_trans_count(dma_chan_, SAMPLES_PER_PERIOD, false);
    // Start DMA + ADC free-run (ADC stops in IRQ after count is reached)
    dma_channel_start(dma_chan_);
    adc_run(true);
}

// Process ADC samples once DMA signals completion: convert to volts and store in circular buffer.
void FlowDetection::process_adc_samples() { 
    int kept_base = num_adc_chs_;   // 4 dummy samples first, then the kept ones
    
    ADC_Samples s{};
    for (int ch = 0; ch < num_adc_chs_; ++ch) {
        uint16_t bits = dma_buf_[kept_base + ch]; // raw ADC value for this channel
        s.v[ch] = bits_to_volts(bits); // convert to volts
    }
        
    // // OPTIONAL: Push into ETL vector; drop oldest half if full (non-blocking policy)
    // if (adc_samples_.size() >= adc_samples_.max_size()) {
    //     adc_samples_.erase(adc_samples_.begin(), adc_samples_.begin() + adc_samples_.size()/2);
    // }
    // adc_samples_.push_back(s); // store in circular buffer

    // Store most recent ADC sample
    latest_adc_sample_ = s;
}

// Initialize round robin
void FlowDetection::setup_round_robin()
{
    adc_init();
    prepare_adc_pins();

    // Ensure we know the starting point for each burst: begin on ADC0
    adc_select_input(0);

    // Round-robin through ADC0..3; enable FIFO + DMA DREQ
    adc_set_round_robin(adc_mask_ & 0x0F); 
    adc_fifo_setup(/*en=*/true, /*dreq_en=*/true, /*dreq_thresh=*/1,
                   /*err_in_fifo=*/false, /*byte_shift=*/false);
    adc_fifo_drain();

    // Slow the ADC clock for more S/H settle time.
    configure_sampling_rate_hz(adc_sample_rate_);
}

// Monitor for leaks
void FlowDetection::leak_monitor()
{
    // Check if leak detection is configured
    if (leak_adc_ < 0 || leak_adc_ >= num_adc_chs_) {
        leak_state_ = 0; // Not configured for leak detection
        return;
    }

    // Check if the latest sample for the leak ADC exceeds the threshold
    float leak_adc_volts = latest_adc_sample_.v[leak_adc_];
    if (leak_adc_volts < leak_threshold_) {
        if (leak_state_ == 0)
        {
            leak_state_ = 1; // Leak detected

            // Initiate alert
            leak_state_alert();

        }
        
    } else {
        if (leak_state_ == 1)
        {
            leak_state_ = 0; // No leak

            // Initiate alert
            leak_state_alert();
        }
    }
}

// Monitor for manual flow meter events
void FlowDetection::manual_flow_meter_monitor()
{
    // Check if manual flow meter monitoring is configured
    if (manual_flow_meter_ < 0 || manual_flow_meter_ >= num_adc_chs_) {
        manual_flow_meter_state_ = 0; // Not configured for manual flow meter monitoring
        return;
    }

    // Check if the latest sample for the manual flow meter ADC deviates from nominal by more than tolerance
    float flow_meter_volts = latest_adc_sample_.v[manual_flow_meter_];
    float lower_bound = nominal_flow_rate_ - flow_rate_tolerance_;
    float upper_bound = nominal_flow_rate_ + flow_rate_tolerance_;

    if (flow_meter_volts < lower_bound || flow_meter_volts > upper_bound) {
        if (manual_flow_meter_state_ == 0)
        {
                manual_flow_meter_state_ = 1; // Alert: flow rate out of expected range
    
                // Initiate alert
                manual_flow_meter_alert();
        }
    } else {
        if (manual_flow_meter_state_ == 1)
        {
            manual_flow_meter_state_ = 0; // Normal
    
            // Initiate alert
            manual_flow_meter_alert();
        }
    }
}

void FlowDetection::update()
{
    // Only sample if sampling is enabled
    if (!sampling_enabled_) return;

    // Report recent ADC values
    if (!dma_channel_is_busy(dma_chan_) && !dma_complete_) {
        sample_adc();
    }

    if (dma_complete_) {
        dma_complete_ = false; // reset for next round
        process_adc_samples();
    }

    // Monitor for leaks
    leak_monitor();

    // Monitor for manual flow meter events
    manual_flow_meter_monitor();
}

