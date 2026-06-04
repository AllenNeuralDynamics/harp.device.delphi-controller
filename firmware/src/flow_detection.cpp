#include "flow_detection.h"

FlowDetection* FlowDetection::s_instance_ = nullptr;

FlowDetection::FlowDetection(uint8_t max_adc_chs)
    : max_adc_chs_(max_adc_chs),
      current_channel_(0),
      sampling_enabled_(false),
      dma_complete_(false),
      ready_for_next_sample_(true),
      dma_tx_chan_(-1),
      dma_rx_chan_(-1),
      leak_adc_(-1),
      leak_threshold_(DEFAULT_LEAK_THRESHOLD),
      leak_state_(0),
      manual_flow_meter_(-1),
      nominal_flow_rate_(DEFAULT_FLOW_RATE),
      flow_rate_tolerance_(FLOW_RATE_TOLERANCE),
      manual_flow_meter_state_(0),
      leak_state_alert_callback_fn_(nullptr),
      manual_flow_meter_alert_callback_fn_(nullptr)
{
    reset();
}

FlowDetection::~FlowDetection()
{
    sampling_enabled_ = false;
    teardown_dma();
    s_instance_ = nullptr;
}

void FlowDetection::reset()
{
    for (uint8_t i = 0; i < NUM_FLOW_METERS; i++) {
        flow_regs_.meter[i].q[0] = DEFAULT_A0;
        flow_regs_.meter[i].q[1] = DEFAULT_A1;
        flow_regs_.meter[i].q[2] = DEFAULT_A2;
        flow_regs_.meter[i].q[3] = DEFAULT_A3;
        flow_regs_.meter[i].q[4] = DEFAULT_A4;
        flow_regs_.meter[i].q[5] = DEFAULT_A5;
    }

    current_channel_ = 0;
    dma_complete_ = false;
    ready_for_next_sample_ = true;

    // teardown_dma();
    prepare_adc_pins();
    drain_spi_fifo();
    setup_dma();
    clear_latest_sample();
}

void FlowDetection::prepare_adc_pins()
{
    spi_init(SPI_PORT, 1'000'000);
    gpio_set_function(PIN_MOSI, GPIO_FUNC_SPI);
    gpio_set_function(PIN_MISO, GPIO_FUNC_SPI);
    gpio_set_function(PIN_SCK, GPIO_FUNC_SPI);

    gpio_init(PIN_CS);
    gpio_set_dir(PIN_CS, GPIO_OUT);
    gpio_put(PIN_CS, 1);
}

void FlowDetection::setup_dma()
{
    dma_tx_chan_ = dma_claim_unused_channel(true);
    dma_rx_chan_ = dma_claim_unused_channel(true);

    auto txc = dma_channel_get_default_config(dma_tx_chan_);
    channel_config_set_transfer_data_size(&txc, DMA_SIZE_8);
    channel_config_set_dreq(&txc, spi_get_dreq(SPI_PORT, true));
    channel_config_set_read_increment(&txc, true);
    channel_config_set_write_increment(&txc, false);

    dma_channel_configure(
        dma_tx_chan_, &txc,
        &spi_get_hw(SPI_PORT)->dr,
        tx_buf_, 3, false
    );

    auto rxc = dma_channel_get_default_config(dma_rx_chan_);
    channel_config_set_transfer_data_size(&rxc, DMA_SIZE_8);
    channel_config_set_dreq(&rxc, spi_get_dreq(SPI_PORT, false));
    channel_config_set_write_increment(&rxc, true);
    channel_config_set_read_increment(&rxc, false);

    dma_channel_configure(
        dma_rx_chan_, &rxc,
        rx_buf_, &spi_get_hw(SPI_PORT)->dr,
        3, false
    );

    s_instance_ = this;
    dma_channel_set_irq1_enabled(dma_rx_chan_, true);
    irq_set_exclusive_handler(DMA_IRQ_1, dma_irq_trampoline);
    irq_set_enabled(DMA_IRQ_1, true);
}

void FlowDetection::dma_irq_trampoline()
{
    if (s_instance_)
        s_instance_->dma_irq_handler();
}

void FlowDetection::dma_irq_handler()
{
    dma_hw->ints1 = (1u << dma_rx_chan_);
    gpio_put(PIN_CS, 1);
    dma_complete_ = true;
}

void FlowDetection::sample_adc()
{
    uint8_t ch = current_channel_;

    tx_buf_[0] = 0x06 | ((ch >> 2) & 0x01);
    tx_buf_[1] = (ch & 0x03) << 6;
    tx_buf_[2] = 0x00;

    dma_complete_ = false;
    gpio_put(PIN_CS, 0);

    dma_channel_set_read_addr(dma_tx_chan_, tx_buf_, false);
    dma_channel_set_write_addr(dma_rx_chan_, rx_buf_, false);
    dma_start_channel_mask((1u << dma_tx_chan_) | (1u << dma_rx_chan_));
}

void FlowDetection::process_latest_sample()
{
    uint8_t ch = current_channel_;

    uint16_t raw =
        ((rx_buf_[1] & 0x0F) << 8) |
         rx_buf_[2];

    latest_raw_adc_sample_.z[ch] = raw;

    float volts = raw * (VREF_VOLTS / ADC_BITS);
    latest_adc_sample_.v[ch] = eval_polynominal(flow_regs_.meter[ch].q, volts);

    current_channel_ = (current_channel_ + 1) % max_adc_chs_;
}

void FlowDetection::update()
{
    if (!sampling_enabled_)
        return;

    if (ready_for_next_sample_) {
        ready_for_next_sample_ = false;
        sample_adc();
    }

    if (dma_complete_) {
        dma_complete_ = false;
        process_latest_sample();
        ready_for_next_sample_ = true;
    }

    leak_monitor();
    manual_flow_meter_monitor();
}

void FlowDetection::clear_latest_sample()
{
    for (uint8_t i = 0; i < max_adc_chs_; i++) {
        latest_adc_sample_.v[i] = 0.0f;
        latest_raw_adc_sample_.z[i] = 0.0f;
    }
}

void FlowDetection::leak_monitor()
{
    if (leak_adc_ < 0 || leak_adc_ >= max_adc_chs_)
        return;

    float v = latest_adc_sample_.v[leak_adc_];

    if (v < leak_threshold_) {
        if (!leak_state_) {
            leak_state_ = 1;
            if (leak_state_alert_callback_fn_)
                leak_state_alert_callback_fn_();
        }
    } else if (leak_state_) {
        leak_state_ = 0;
        if (leak_state_alert_callback_fn_)
            leak_state_alert_callback_fn_();
    }
}

void FlowDetection::manual_flow_meter_monitor()
{
    if (manual_flow_meter_ < 0 || manual_flow_meter_ >= max_adc_chs_)
        return;

    float v = latest_adc_sample_.v[manual_flow_meter_];
    float lo = nominal_flow_rate_ - flow_rate_tolerance_;
    float hi = nominal_flow_rate_ + flow_rate_tolerance_;

    if (v < lo || v > hi) {
        if (!manual_flow_meter_state_) {
            manual_flow_meter_state_ = 1;
            if (manual_flow_meter_alert_callback_fn_)
                manual_flow_meter_alert_callback_fn_();
        }
    } else if (manual_flow_meter_state_) {
        manual_flow_meter_state_ = 0;
        if (manual_flow_meter_alert_callback_fn_)
            manual_flow_meter_alert_callback_fn_();
    }
}