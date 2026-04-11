# Tab: Flow/ADC

Configure the 8 ADC channels — type, conversion equations, and leak detection thresholds.

---

## Layout

Split screen:

```
┌──────────────────┬──────────────────────────────────────┐
│  Channel List    │  Channel Detail                      │
│                  │                                      │
│  # │ Name │ Type │  Type: [Flow Meter 100mL ▼]          │
│  0 │ ...  │ FM   │  Conversion ...                      │
│  1 │ ...  │ FM   │  Leak Detection ...                  │
│  ...              │  Calibration (future) ...            │
│  7 │ ...  │ ADC  │                                      │
└──────────────────┴──────────────────────────────────────┘
```

---

## Left Panel: Channel List

8 channels (ADC inputs 0–7, mapped to `AdcMask` reg 81 and `LatestRawAdcSample` reg 83).

Each row:
- **Index** — 0–7
- **Name** — editable; committed on `<Return>` or focus-out. Name is reflected immediately in the Dashboard plot titles and Flow Rates card.
- **Type badge** — short label: `ADC`, `FM100`, `FM1L`
- **Plot** checkbox — when checked, adds this channel's live plot and flow rate row to the Dashboard. Changes take effect immediately. Channels 0 and 1 are checked by default.

---

## Right Panel: Channel Detail

### Type Dropdown

Options: `Analog` | `Flow Meter 100mL` | `Flow Meter 1L`

Type affects what conversion is applied to raw ADC counts before display.

---

### Conversion Equation

Linear conversion: `value = slope * adc_counts + offset`

- Default slope/offset values are defined per type in `channel_config.json` under `type_defaults`
- User can override per-channel in editable fields
- Saved to `channel_config.json` on change

```
Conversion
──────────────────────────────
Slope:   [ 0.01234  ]   (mL/min per count)
Offset:  [ 0.000    ]
Preview: 1000 counts → 12.34 mL/min
```

`channel_config.json` schema for type defaults:
```json
"type_defaults": {
  "flow_meter_100mL": { "slope": 0.01, "offset": 0.0 },
  "flow_meter_1L":    { "slope": 0.1,  "offset": 0.0 },
  "analog":           { "slope": 1.0,  "offset": 0.0 }
}
```

---

### Leak Detection

Applies to whichever channel is designated as the leak sensor (`LeakAdcChannel`, reg 86, S8).

```
Leak Detection
──────────────────────────────
Enable:  [ checkbox ]
Min:     [ 0.00  ]   mL/min
Max:     [ 5.00  ]   mL/min

Writes:
  LeakAdcChannel (reg 86) → S8, this channel's index (or -1 to disable)
  LeakThreshold  (reg 87) → Float  [Note: firmware uses single threshold;
                                    min/max stored locally, alert fires if
                                    value outside range based on LeakState]
```

Note: The hardware `LeakThreshold` register (reg 87) is a single float. The GUI stores both min and max in `channel_config.json` and triggers the Dashboard alert based on the polled `LeakState` (reg 88) being non-zero.

---

### Calibration (Future)

```
Calibration                    [Coming Soon — grayed out]
──────────────────────────────
[ Run Calibration ]   (disabled)
Last calibrated: —
```

Placeholder section, disabled. Calibration workflow to be defined in a future roadmap item.

---

## ADC Enable Mask

The `AdcMask` register (reg 81, U16) enables individual ADC channels. This is written automatically based on which channels have a type assigned (non-Analog channels are enabled; pure Analog channels may or may not be — TBD).

Alternatively, expose a global "Enable Sampling" toggle at the top of this tab that sets `EnableAdcSampling` (reg 84) and `AdcSamplingRate` (reg 85).
