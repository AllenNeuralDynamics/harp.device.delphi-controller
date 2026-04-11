# channel_config.json — Schema Reference

`gui/channel_config.json` is auto-created on first run with defaults. It persists user configuration between sessions and across machines (commit it to the repo once configured).

---

## Full Schema

```json
{
  "dashboard_valve_indices": [0, 1, 2, 3],
  "dashboard_flow_plot_indices": [0, 1],

  "type_defaults": {
    "analog":           { "slope": 1.0,   "offset": 0.0 },
    "flow_meter_100mL": { "slope": 0.01,  "offset": 0.0 },
    "flow_meter_1L":    { "slope": 0.1,   "offset": 0.0 }
  },

  "valve_channels": [
    {
      "index": 0,
      "name": "Valve 0",
      "type": "on_off",
      "dashboard": true
    }
    // ... 15 more entries (indices 0–15)
  ],

  "flow_adc_channels": [
    {
      "index": 0,
      "name": "Odor",
      "type": "flow_meter_100mL",
      "conversion": { "slope": 0.01, "offset": 0.0 },
      "leak_detection": {
        "enabled": false,
        "min": 0.0,
        "max": 100.0
      },
      "dashboard_plot": true
    }
    // ... 7 more entries (indices 0–7)
  ]
}
```

---

## Field Descriptions

### Top-level

| Field | Type | Description |
|---|---|---|
| `dashboard_valve_indices` | list[int] | Which valve channel indices (0–15) appear on Dashboard. Max 4. |
| `dashboard_flow_plot_indices` | list[int] | Which ADC channel indices (0–7) get a live plot on Dashboard. |
| `type_defaults` | object | Default slope/offset per flow meter type, applied when a channel type is changed. |

### valve_channels[N]

| Field | Type | Description |
|---|---|---|
| `index` | int | 0–15, matches hardware valve index |
| `name` | str | Display name (user-editable in Valves tab) |
| `type` | str | `"on_off"` or `"proportional"` |
| `dashboard` | bool | Whether this valve appears in Dashboard valve card |

### flow_adc_channels[N]

| Field | Type | Description |
|---|---|---|
| `index` | int | 0–7, matches ADC channel index |
| `name` | str | Display name (user-editable in Flow/ADC tab) |
| `type` | str | `"analog"`, `"flow_meter_100mL"`, or `"flow_meter_1L"` |
| `conversion.slope` | float | Multiplier: `value = slope * adc_counts + offset` |
| `conversion.offset` | float | Additive offset in output units (mL/min) |
| `leak_detection.enabled` | bool | Whether this channel is the active leak sensor |
| `leak_detection.min` | float | Minimum acceptable value (alert if below) |
| `leak_detection.max` | float | Maximum acceptable value (alert if above) |
| `dashboard_plot` | bool | Whether this channel gets a live plot card on Dashboard |

---

## Conversion Equation

```
display_value [mL/min] = slope * raw_adc_counts + offset
```

The slope/offset for each type are starting-point defaults. They should be tuned to match actual sensor calibration. Per-channel overrides in `flow_adc_channels[N].conversion` take precedence over `type_defaults`.

Future: a calibration workflow will update per-channel slope/offset automatically.
