# Tab: Dashboard

Live readout of device state. No configuration — read-only (except valve toggles).

---

## Layout

Grid of cards. Suggested layout:

```
┌─────────────────────┬─────────────────────┐
│   Flow Rates        │   Valve Controls    │
│   (numeric)         │   (toggles)         │
├──────────┬──────────┴──────────┬──────────┤
│ Flow     │ Flow     │ Flow     │ Flow     │
│ Plot 0   │ Plot 1   │ Plot 2   │ Plot 3   │
└──────────┴──────────┴──────────┴──────────┘
[LEAK ALERT BANNER — hidden unless active]
```

The number of visible plots and their titles are driven by the Flow/ADC tab — channels with **Plot** checked appear here. The layout reflows into 2 columns; rows are added as needed.

---

## Card: Flow Rates

- Source: `LatestFlowRate` (reg 82), 4x float, polled by DeviceManager
- Displays one row per channel that has **Plot** checked on the Flow/ADC tab (same set as the live plots)
- Channel names come from the Flow/ADC tab; rows rebuild automatically when plot selection or names change
- Value updates at ~1 Hz; plot curves update at ~5 Hz

```
Flow Rates
──────────────────────
Odor      :  12.34 mL/min
Exhaust   :   8.91 mL/min
```

---

## Card: Valve Controls

- Shows whichever valve channels have **Dashboard** checked on the Valves tab (any number, including zero)
- Rebuilt automatically via `sync_valves(configs)` whenever the Valves tab checkbox or name changes — same pattern as `sync_plots` for the Flow/ADC tab
- When no valves are selected, shows a "Select valves on the Valves tab" placeholder
- Each row: channel name + CTk toggle switch
- Toggle ON → `HarpMessage.WriteU16(AppRegs.ValvesSet, 1 << channel_index)`
- Toggle OFF → `HarpMessage.WriteU16(AppRegs.ValvesClear, 1 << channel_index)`
- State synced from `ValvesState` (reg 32) on each poll; `update_valve_states(bitmask)` maps each switch to its real hardware channel index

---

## Cards: Live Flow Plots

- One card per channel with **Plot** checked on the Flow/ADC tab
- Up to 8 cards (one per ADC channel), 2-column layout with rows added as needed
- Card title: `"Flow — <channel name>"` — updates live when the name is edited on the Flow/ADC tab
- Each card: scrolling time-series, last 10 seconds of data, matplotlib embedded via `FigureCanvasTkAgg`
- Y-axis: mL/min (converted via channel's slope/offset from `channel_config.json`)
- X-axis: elapsed seconds
- Redrawn at ~5 Hz

Implementation notes:
- `LivePlot` keeps a `collections.deque(maxlen=100)` per channel. `_redraw()` filters to the last 10s and sets `ax.set_xlim(now-10, now)` to hold the window fixed.
- Plot widgets are pooled and never destroyed (only hidden/reused) to avoid crashes from pending matplotlib `after_idle` callbacks on destroyed Tk widgets.
- `FigureCanvasTkAgg` is subclassed (`_Canvas`) with `_update_device_pixel_ratio` overridden as a no-op. This prevents matplotlib from auto-detecting the Retina 2× scale factor, which Homebrew Tk does not handle correctly (it would display the 2× bitmap at 2× logical size). Figure DPI is set to 50 to compensate for the remaining scale difference on Retina displays.

---

## Leak Alert Banner

- Normally hidden (`grid_remove()` or `pack_forget()`)
- Becomes visible (red background, bold text) when `LeakState` (reg 88) is non-zero
- Text: "⚠ Leak Detected — Channel {N}" where N is from `LeakAdcChannel` (reg 86)
- Dismissed automatically when `LeakState` returns to 0 on next poll
- Banner spans full width, placed at bottom of tab
