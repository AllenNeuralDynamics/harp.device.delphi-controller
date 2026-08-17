# pid_timing.py — Design Notes

## ADC register
- Register 82 (`LatestRawAdcSample`) returns 8 raw uint16 values packed as a Float-typed payload (16 bytes total: 8 × 2 bytes, little-endian `<8H`).
- Read with `HarpMessage.ReadFloat(...)` despite the uint16 contents — this matches firmware expectations.

## Harp timestamp
- `reply.timestamp` is the device-side timestamp embedded in the HarpMessage reply.
- The timestamp is present whenever the PayloadType has the timestamp flag set, which is the case for `LatestRawAdcSample` replies.
- This timestamp is used directly as the `harp_timestamp` CSV column.

## Poll rate
- Controlled by `--rate` argument (default 800 Hz).
- Actual rate is bottlenecked by the serial round-trip time (1 Mbaud). Observed ~795 Hz in practice.
- `time.perf_counter()` is used for sub-millisecond interval precision.
- Poke state is evaluated before ADC in each loop iteration so state is settled before sampling.

## CSV output
- Filename: `analog_values_YYYYMMDD_HHMMSS.csv` — timestamped at script start.
- Columns: `harp_timestamp, ch0, ch1, ch2, ch3, ch4, ch5, ch6, ch7`
- `poke_YYYYMMDD_HHMMSS.csv` shares the same timestamp. Columns: `harp_timestamp, odor1, odor2, poke`
- Both files are written to the `data/` directory (created automatically).

## Channel map
| Channel | Assignment |
|---------|------------|
| ch0 | Proportional valve 0 flow meter (ADC pin 26) |
| ch1 | Proportional valve 1 flow meter (ADC pin 27) |
| ch2 | Proportional valve 2 flow meter (ADC pin 28) |
| ch3 | Manual flow meter |
| ch4 | (unassigned in this script) |
| ch5 | (unassigned in this script) |
| ch6 | (unassigned in this script) |
| ch7 | Target sensor for odor-onset timing |

## Simulated poke / odor alternation
- Controlled by `--poke-on` and `--poke-off` (default 1.0s each).
- Alternates odor1 (`QueuedOdorMask=0x0001`) and odor2 (`QueuedOdorMask=0x0002`) on each poke-on.
- Odor1 is always active at script start.
- Poke is triggered via `ForceFSM=1` / `ForceFSM=0` writes.

## Ch7 onset detection
- After each poke-on (any odor), the script arms ch7 detection.
- Detection fires when ch7 exceeds `--ch7-threshold` (default 100) within `--ch7-timeout` seconds (default 0.5s).
- Elapsed time is measured in Harp time: `harp_ts - active_poke_harp_ts`, printed in ms.
- The odor label (odor1 or odor2) is included in the print.

## Hardware behavior of ch7 (observed)
- Ch7 responds consistently ~60-62ms after every **odor2** poke-on.
- Ch7 responds ~27ms after **odor1** poke 1 only (first poke, tubing primed from startup).
- Ch7 does NOT respond to subsequent odor1 pokes (pokes 3, 5, 7...).
- Conclusion: ch7 is physically on or near the odor2 path. The fast odor1 response on poke 1 is likely due to residual odor1 already in the tubing from initialization.

## Debugging history
- A `ch7_armed` rising-edge gate caused missed detections when valve transients kept ch7 above baseline.
- Keeping `waiting_for_ch7=True` across odor2 pokes caused false 2-second detections: a ch7 rise ~70ms after the odor2 poke was incorrectly attributed to the preceding odor1 poke timestamp.
- The fix: arm detection on EVERY poke (not just odor1), with a 0.5s timeout window. This correctly fires once per poke-on cycle and correctly labels which odor triggered it.
- The poke block must run before the ADC block in each loop iteration to prevent a same-iteration race where ADC sets a detection flag that poke-on then resets.
