# Delphi Controller GUI — Roadmap

GUI for testing and operating the Delphi Harp hardware device. Replaces ad-hoc CLI test scripts with a structured, multi-tab interface.

## Status

| Feature | Status |
|---|---|
| Project scaffold (main.py, app.py, device_manager.py) | Complete |
| Config tab — connection card | Complete |
| Config tab — register cards (cameras, PID, poke, odor) | Layout complete — PID write wired; cameras/poke/odor not wired |
| Config tab — message log | Complete — all `DeviceManager.send()` calls logged with register name, value, reply; ValveConfigs replies decoded as hit/hold/dur |
| Valves tab — on/off toggle | Complete — ValvesSet/ValvesClear wired; ValveConfigs (hit/hold/dur) read and write wired per channel |
| Valves tab — proportional | Layout complete — ADC, PID enable, target flow rate wired; duty cycle readout wired to poll |
| Flow/ADC tab | Layout complete — not wired |
| Dashboard tab | Layout complete — demo data running; plots and flow rate labels wired to Flow/ADC tab |
| DeviceManager polling thread | Complete — polls flow rates, ADC, leak state, valve state, duty cycles at 5 Hz |
| Harp message sends (valves) | Complete — tested on hardware |
| Harp message sends (PID, cameras, etc.) | Partially wired — untested |
| Data logging (CSV) | Not started |
| Flow meter calibration | Future |

## Known Issues

- **Mouse wheel scroll on CTkScrollableFrame does not work on macOS.** Multiple approaches attempted (enter/leave bind, recursive child binding, `bind_all` with bounds check) — none reliably captured scroll events delivered to child widgets inside the scrollable frame. Workaround: click and drag the scrollbar. Revisit when upgrading CustomTkinter or switching to a different scroll approach.

- **ValveConfigs hit/hold/dur semantics:** `hit_duration_us=0` means skip the hit phase and apply hold duty immediately. With `hold=0.0` (firmware default for valves 0–2), the valve will not open even when set via ValvesSet. Set `hold > 0` (e.g. 1.0) via Write Config before toggling. `dur > 0` enables a brief high-current hit phase before dropping to hold — useful for overcoming static friction.

---

## Tech Stack

| Component | Library |
|---|---|
| GUI framework | `customtkinter` |
| Live plots | `matplotlib` + `FigureCanvasTkAgg` |
| Serial port detection | `pyserial` (`serial.tools.list_ports`) |
| Device communication | `pyharp` (already in repo) |
| Register definitions | `app_registers_refactor.py` |

Dependencies are declared in `pyproject.toml`. Install and run with uv:
```bash
cd software
uv run python gui/main.py
```

---

## File Structure

```
software/
├── ROADMAP.md                        # This file
├── pyproject.toml                    # uv/pip dependencies (no build backend)
├── docs/
│   ├── architecture.md               # Threading model, DeviceManager, message patterns
│   ├── tab_dashboard.md              # Dashboard tab spec
│   ├── tab_valves.md                 # Valves tab spec
│   ├── tab_flow_adc.md               # Flow/ADC tab spec
│   ├── tab_config.md                 # Config tab spec
│   └── channel_config.md             # channel_config.json schema
├── gui/
│   ├── main.py                       # Entry point: `uv run python gui/main.py`
│   ├── app.py                        # Main CTk window, tab bar, status chip in header
│   ├── device_manager.py             # Device connection + background polling thread (stub)
│   ├── utils.py                      # Shared helpers (bind_scroll_wheel)
│   ├── channel_config.json           # Persisted channel settings
│   ├── tabs/
│   │   ├── __init__.py
│   │   ├── dashboard.py              # Flow rates, valve toggles, live plots, leak banner
│   │   ├── valves.py                 # 16-channel list + detail panel
│   │   ├── flow_adc.py               # 8-channel list + detail panel
│   │   └── config_tab.py             # Connection + cameras + PID + poke + odor
│   └── widgets/
│       ├── __init__.py
│       ├── tile.py                   # Reusable card/tile container widget
│       └── live_plot.py              # Matplotlib 10-second rolling time-series card
├── app_registers_refactor.py         # Harp register address enum (source of truth)
├── prototype_testing.py              # Reference: read_float4_from_u8, polling loop
└── test_refactor_fimrware.py         # Reference: WriteHarpMessage/PayloadType patterns
```

---

## Tabs Overview

See `docs/` for full specs.

| Tab | Purpose |
|---|---|
| **Dashboard** | Live flow rate readouts, configurable valve toggles, live plots, leak alert |
| **Valves** | Configure all 16 valve channels (on/off vs proportional), select dashboard valves |
| **Flow/ADC** | Configure 8 ADC channels (type, conversion equations, leak detection thresholds) |
| **Config** | Device connection, camera triggers, PID gains, poke port, odor mask |

---

## Running

```bash
cd software
uv run python gui/main.py
```

On first run with no device connected, the app opens normally. Connect via the Config tab.

> **Note:** `pyproject.toml` has no `[build-system]` — uv installs dependencies only, it does not try to build the project as a package.

---

## Next Steps (feature-by-feature order)

1. **DeviceManager** — background polling thread, queue-based updates to Dashboard
2. **Config tab — Connect** — wire connect/disconnect to DeviceManager
3. **Dashboard — live data** — replace demo ticker with real poll drain
4. **Valves tab** — wire on/off toggles and proportional valve writes
5. **Flow/ADC tab** — wire conversion equations and leak detection registers
6. **Config tab — register cards** — wire PID, cameras, poke, odor mask writes
7. **Data logging** — CSV output from poll drain

## Future Work

- Flow meter calibration UI (placeholder exists in Flow/ADC tab)
- Event-driven reads via `device.get_events()` in addition to polling
- Mouse wheel scroll on CTkScrollableFrame (see Known Issues)
- Per-session settings persistence (window size, last-used COM port)
