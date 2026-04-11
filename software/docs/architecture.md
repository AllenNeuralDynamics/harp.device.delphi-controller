# Architecture

> **Implementation status:** GUI scaffold complete. DeviceManager is a stub — no hardware calls wired yet. All sections below describe the intended final architecture; stubs are noted where applicable.

## Overview

The GUI is structured around a central `DeviceManager` that owns the serial connection and runs a background polling thread. All tabs interact with the device through `DeviceManager` — they never call `pyharp` directly.

---

## DeviceManager (`gui/device_manager.py`) — stub, not yet wired to hardware

### Responsibilities
- Owns the `Device(com_port)` instance
- Runs a background daemon thread that polls registers at a fixed rate (default 5 Hz)
- Queues poll results for safe consumption on the main (GUI) thread
- Exposes a `send()` method for one-off writes from the UI

### Connection lifecycle
```python
dm = DeviceManager()
dm.connect("COM20")    # creates Device, starts poll thread
dm.disconnect()        # stops thread, closes Device
dm.is_connected        # bool property
```

### Background polling thread
Polls these registers on every tick:
- `LatestFlowRate` (reg 82) — 4x float, little-endian, 16 bytes total
- `LatestRawAdcSample` (reg 83) — U16 array
- `LeakState` (reg 88) — U8
- `ValvesState` (reg 32) — U16

Results are placed into a `queue.Queue` as dicts:
```python
{
    "flow_rates": [f0, f1, f2, f3],        # list[float]
    "adc_samples": [a0, ..., a7],           # list[int]
    "leak_state": 0,                        # int
    "valves_state": 0b0000000000000000,     # int (bitmask)
    "timestamp": 1234567890.123,            # float (time.time())
}
```

### GUI thread consuming the queue
`app.py` uses `ctk.after(200, self._drain_queue)` to pull from the queue and fan out updates to tabs. No direct thread→widget calls are made.

### Send helper
```python
reply = dm.send(HarpMessage.WriteU8(address, value))
# dm.send() calls device.send(msg.frame) and returns the reply message
```

### Unpacking helpers (from prototype_testing.py)
```python
def read_float4_from_u8(u8_array):
    buf = bytes(bytearray(u8_array))
    return struct.unpack("<4f", buf)  # 4 little-endian floats

def pack_pid_config(kp, ki, kd):
    return struct.pack("<fff", kp, ki, kd)  # 12 bytes
```

---

## Threading Model

```
Main thread (GUI)
  └─ ctk.after(200ms) → _drain_queue() → update widgets

Daemon thread (DeviceManager._poll_loop)
  └─ polls device registers
  └─ puts results → queue.Queue (thread-safe)
```

The daemon thread is stopped by setting a `threading.Event` before calling `device.disconnect()`.

---

## Message Patterns

All message patterns are from `pyharp`:

### Simple read
```python
from pyharp.messages import HarpMessage
reply = device.send(HarpMessage.ReadFloat(address).frame)
value = reply.payload  # varies by type
```

### Simple write
```python
reply = device.send(HarpMessage.WriteU8(address, value).frame)
reply = device.send(HarpMessage.WriteFloat(address, value).frame)
```

### Complex write (multi-float payload)
```python
from pyharp.messages import WriteHarpMessage, PayloadType
import struct

pid_bytes = struct.pack("<fff", kp, ki, kd)
msg = WriteHarpMessage(
    PayloadType.Float,
    pid_bytes,
    DelphiOnlyAppRegs.PidGains,
    offset=len(pid_bytes) - 1
)
device.send(msg.frame)
```

### Events
```python
for msg in device.get_events():
    if msg.address == DelphiOnlyAppRegs.LeakState:
        handle_leak(msg.payload[0])
```

---

## Shared Utilities (`gui/utils.py`)

### bind_scroll_wheel
```python
from utils import bind_scroll_wheel
bind_scroll_wheel(scroll_frame)  # call AFTER all child widgets are added
```

Attempts to enable mouse-wheel scrolling on `CTkScrollableFrame` using `bind_all` with a pointer-bounds check. **Currently not working on macOS** — scroll events delivered to child widgets are not captured. Workaround: use the scrollbar directly. Call is still included so the fix can be dropped in without touching other files.

---

## Register Source of Truth

All register addresses are in `app_registers_refactor.py`:
- `AppRegs` — addresses 32–58 (valves, GPIO)
- `DelphiOnlyAppRegs` — addresses 59–107 (poke, FSM, odor, flow, ADC, PID, cameras, proportional valves)
