# Tab: Config

Device connection and hardware configuration registers.

---

## Layout

Scrollable column of cards:

```
┌──────────────────────────────────────────┐
│  Connection                              │
├──────────────────────────────────────────┤
│  Camera Triggers                         │
├──────────────────────────────────────────┤
│  PID Gains                               │
├──────────────────────────────────────────┤
│  Poke Port                               │
├──────────────────────────────────────────┤
│  Odor Mask                               │
└──────────────────────────────────────────┘
```

A compact status chip (green/red dot + "Connected"/"Disconnected" text) is visible in the tab label row so connection state is glanceable from any tab.

---

## Card: Connection

```
Connection
──────────────────────────────────────────
Port:    [ COM20 ▼ ]  [ Refresh ]
         [ Connect ]   ● Connected

CSV Log: [ Start Logging ]
         Saving to: delphi_log_2026-04-10_123456.csv
```

- Port dropdown populated via `serial.tools.list_ports.comports()` at startup and on Refresh
- Connect → `DeviceManager.connect(port)`, calls `device.info()` output to a small text box below
- Disconnect → `DeviceManager.disconnect()`
- CSV logging toggle starts/stops `DataLogger`; shows current file path when active

---

## Card: Camera Triggers

Two sub-sections, Cam0 and Cam1.

```
Camera Triggers
──────────────────────────────────────────
Cam0
  Enable:      [ OFF ●──── ON ]      reg 75 (EnableCam0Trigger, U8)
  Frame Rate:  [ 30  ] fps           reg 73 (Cam0FrameRate, U32)
  Duty Cycle:  [ 0.50 ] (0.0–1.0)   reg 74 (Cam0DutyCycle, Float)
  Pin State:   0                     reg 72 (Cam0PinState, read-only)

Cam1
  Enable:      [ OFF ●──── ON ]      reg 79 (EnableCam1Trigger, U8)
  Frame Rate:  [ 30  ] fps           reg 77 (Cam1FrameRate, U32)
  Duty Cycle:  [ 0.50 ] (0.0–1.0)   reg 78 (Cam1DutyCycle, Float)
  Pin State:   0                     reg 76 (Cam1PinState, read-only)
```

Writes fire on focus-out or Enter from each input field.

---

## Card: PID Gains

```
PID Gains
──────────────────────────────────────────
Update Rate:  [ 100.0 ] Hz       reg 94 (PidUpdateFrequency, Float)

Kp:  [ 1.000 ]                   \
Ki:  [ 0.100 ]                    ├─ reg 95 (PidGains, 3x Float)
Kd:  [ 0.010 ]                   /

[ Write Gains ]   ← explicit button; packs via struct.pack("<fff", kp, ki, kd)
                     uses WriteHarpMessage(PayloadType.Float, ..., PidGains)
```

PID gains are written together as a single 12-byte payload (not individually).

---

## Card: Poke Port

```
Poke Port
──────────────────────────────────────────
Pin:        [ 0  ] (0–7)    reg 59 (PokePin, U8)
Inverted:   [ OFF ●──── ON ] reg 60 (PokePinInverted, U8)

Live State:
  Poke State:      0         reg 61 (PokeState, polled)
  Raw Poke State:  0         reg 62 (RawPokeState, polled)
  Poke Count:      0         reg 63 (PokeDometer, polled)
```

Pin and Inverted write on change. Live state fields update from DeviceManager poll (add these registers to poll list if needed, or read on tab focus).

---

## Card: Odor Mask

```
Odor Mask                                reg 66 (QueuedOdorMask, U16)
──────────────────────────────────────────
Valve:  0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15
        ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐  ☐

[ Write Mask ]   Writes U16 bitmask: bit N = valve N selected

Timing (all U32, microseconds)
  Odor Setup Time:        [ 1000000  ] µs   reg 67
  Min Odor Delivery Time: [ 2000000  ] µs   reg 68
  Max Odor Delivery Time: [ 5000000  ] µs   reg 69
  Minimum Poke Time:      [ 500000   ] µs   reg 70
  Odor Dwell Time:        [ 1000000  ] µs   reg 71
```

Timing fields write individually on focus-out. Each calls `WriteU32(reg, value)`.
