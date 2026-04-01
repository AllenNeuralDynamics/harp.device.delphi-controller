#include "flow_detection.h"

FlowDetection* FlowDetection::s_instance_ = nullptr;

FlowDetection::FlowDetection(uint8_t adc_mask)
    : adc_mask_(adc_mask),
      num_adc_chs_{NUM_ADC_CHS},
      adc_sample_rate_(DEFAULT_SAMPLE_RATE),
      sampling_enabled_(true),
      leak_adc_(-1),
      leak_threshold_(DEFAULT_LEAK_THRESHOLD),
      leak_state_(0),
      manual_flow_meter_(-1),
      nominal_flow_rate_(DEFAULT_FLOW_RATE),
      flow_rate_tolerance_(FLOW_RATE_TOLERANCE),
      manual_flow_meter_state_(0),
      conversion_slope_(0.02f),
      conversion_offset_(0.5f),
      dma_complete_(false),
      current_channel_(0),
      leak_state_alert_callback_fn_(nullptr),
      manual_flow_meter_alert_callback_fn_(nullptr)
{
    reset();
}

FlowDetection::~FlowDetection()
{
    sampling_enabled_ = false;
    dma_channel_abort(dma_tx_chan_);
    dma_channel_abort(dma_rx_chan_);
    s_instance_ = nullptr;
}

// ================= FSM Control =================
void FlowDetection::reset()
{
    adc_sample_rate_ = DEFAULT_SAMPLE_RATE;
    dma_complete_ = false;
    sampling_enabled_ = false;
    leak_adc_ = -1;
    manual_flow_meter_ = -1;
    leak_state_ = 0;
    manual_flow_meter_state_ = 0;

    prepare_adc_pins();
    setup_dma();
    clear_latest_sample();

    leak_state_alert_callback_fn_ = nullptr;
    manual_flow_meter_alert_callback_fn_ = nullptr;

    conversion_slope_ = VOLTS_FLOW_RATE_SLOPE;
    conversion_offset_ = VOLTS_FLOW_RATE_OFFSET;
}

// ================= SPI / GPIO =================
void FlowDetection::prepare_adc_pins()
{
    spi_init(SPI_PORT, 1'000'000);

    gpio_set_function(PIN_MOSI, GPIO_FUNC_SPI);
    gpio_set_function(PIN_MISO, GPIO_FUNC_SPI);
    gpio_set_function(PIN_SCK,  GPIO_FUNC_SPI);

    gpio_init(PIN_CS);
    gpio_set_dir(PIN_CS, GPIO_OUT);
    gpio_put(PIN_CS, 1);
}

// ================= DMA =================
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

    s_instance_ = this;

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

    dma_channel_set_irq0_enabled(dma_rx_chan_, true);
    irq_set_exclusive_handler(DMA_IRQ_0, dma_irq_trampoline);
    irq_set_enabled(DMA_IRQ_0, true);
}

void FlowDetection::dma_irq_trampoline()
{
    if (s_instance_)
        s_instance_->dma_irq_handler();
}

void FlowDetection::dma_irq_handler()
{
    dma_hw->ints0 = 1u << dma_rx_chan_;
    gpio_put(PIN_CS, 1);

    uint16_t raw =
        ((rx_buf_[1] & 0x03) << 8) |
         rx_buf_[2];

    latest_adc_sample_.v[current_channel_] =
        convert_to_flowrate(raw);

    current_channel_ =
        (current_channel_ + 1) % num_adc_chs_;

    dma_complete_ = true;
}

// ================= Sampling =================
void FlowDetection::sample_adc()
{
    tx_buf_[0] = 0x01;
    tx_buf_[1] = 0x80 | (current_channel_ << 4);
    tx_buf_[2] = 0x00;

    gpio_put(PIN_CS, 0);

    dma_channel_set_read_addr(dma_tx_chan_, tx_buf_, false);
    dma_channel_set_write_addr(dma_rx_chan_, rx_buf_, false);

    dma_start_channel_mask(
        (1u << dma_tx_chan_) |
        (1u << dma_rx_chan_)
    );
}

void FlowDetection::clear_latest_sample()
{
    for (uint8_t i = 0; i < num_adc_chs_; i++)
        latest_adc_sample_.v[i] = 0.0f;
}

// ================= Monitoring =================
void FlowDetection::leak_monitor()
{
    if (leak_adc_ < 0 || leak_adc_ >= num_adc_chs_) {
        leak_state_ = 0;
        return;
    }

    float leak_adc_volts =
        latest_adc_sample_.v[leak_adc_];

    if (leak_adc_volts < leak_threshold_) {
        if (!leak_state_) {
            leak_state_ = 1;
            leak_state_alert();
        }
    } else {
        if (leak_state_) {
            leak_state_ = 0;
            leak_state_alert();
        }
    }
}

void FlowDetection::manual_flow_meter_monitor()
{
    if (manual_flow_meter_ < 0 ||
        manual_flow_meter_ >= num_adc_chs_) {
        manual_flow_meter_state_ = 0;
        return;
    }

    float flow =
        latest_adc_sample_.v[manual_flow_meter_];

    float lo = nominal_flow_rate_ - flow_rate_tolerance_;
    float hi = nominal_flow_rate_ + flow_rate_tolerance_;

    if (flow < lo || flow > hi) {
        if (!manual_flow_meter_state_) {
            manual_flow_meter_state_ = 1;
            manual_flow_meter_alert();
        }
    } else {
        if (manual_flow_meter_state_) {
            manual_flow_meter_state_ = 0;
            manual_flow_meter_alert();
        }
    }
}

// ================= Update Loop =================
void FlowDetection::update()
{
    if (!sampling_enabled_)
        return;

    if (!dma_channel_is_busy(dma_rx_chan_) &&
        !dma_complete_) {
        sample_adc();
    }

    if (dma_complete_) {
        leak_monitor();
        manual_flow_meter_monitor();
        dma_complete_ = false;
    }
}