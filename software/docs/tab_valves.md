# Tab: Valves

Configure all 16 valve channels and control them individually.

---

## Layout

Split screen, 30/70 or similar:

```
┌──────────────────┬──────────────────────────────────────┐
│  Channel List    │  Channel Detail                      │
│                  │                                      │
│  # │ Name │Type │  [Selected channel details here]      │
│  0 │ ...  │ O/O │                                      │
│  1 │ ...  │ O/O │                                      │
│  ...              │                                      │
│ 15 │ ...  │ Prop│                                      │
└──────────────────┴──────────────────────────────────────┘
```

---

## Left Panel: Channel List

Scrollable list of all 16 valve channels (registers `ValveConfigs0-15`, addresses 35–50).

Each row contains:
- **Index** — 0–15 (read-only label)
- **Name** — editable text field (stored in `channel_config.json`, not a hardware register)
- **Type** dropdown — `On/Off` | `Proportional`
- **Dashboard** checkbox — marks this channel to appear in the Dashboard valve card; any number may be checked

Changing the Type dropdown:
- Updates `channel_config.json`
- Refreshes the right panel if this channel is selected

---

## Right Panel: Channel Detail

Shows detail for whichever channel is clicked in the left panel.

### If type = On/Off

```
Channel 3 — Valve Name
─────────────────────────────
State:   [  OFF  ●────  ON  ]  ← CTk toggle switch

Reads ValvesState (reg 32) bit [channel_index] to init toggle state.
Toggle ON  → WriteU16(ValvesSet,   1 << index)
Toggle OFF → WriteU16(ValvesClear, 1 << index)
```

### If type = Proportional

Only valve indices 0, 1, 2 support proportional control (hardware limitation).
If a channel > 2 is set to Proportional, show a warning label.

Proportional valve registers (for valve N = 0, 1, or 2):
- `ProportionalValveNAdc` (regs 96, 100, 104) — U8, ADC channel for feedback
- `ProportionalValveNEnablePid` (regs 97, 101, 105) — U8, PID enable flag
- `ProportionalValveNDutyCycle` (regs 98, 102, 106) — Float, current duty cycle (read-only display)
- `ProportionalValveNTargetFlowRate` (regs 99, 103, 107) — Float, setpoint in mL/min

```
Channel 0 — Proportional Valve
─────────────────────────────────────────
ADC Channel:      [  0  ▼]          (dropdown 0–7)
PID Enabled:      [  OFF  ●────  ON ]
Duty Cycle:       0.00 %            (live readout, updated from poll)
Target Flow Rate: [  0.00  ] mL/min  (editable, WriteFloat on change)
```

Writes:
- ADC channel → `WriteU8(ProportionalValveNAdc, value)`
- PID enable → `WriteU8(ProportionalValveNEnablePid, 0 or 1)`
- Target flow → `WriteFloat(ProportionalValveNTargetFlowRate, value)`
- Duty cycle is read-only, polled from DeviceManager

---

## Persistence

Channel names, types, and dashboard selections are stored in `channel_config.json` (not written to the device). This file is loaded at startup and saved whenever a field changes.
