# End-valve switching causes flow spikes

## Observation
When the end valve switches state, the downstream flow sensor shows a big spike
that settles back to the correct value over time. Seen with both ends of the
switch (open->closed and closed->open).

## Theory
The end valve is downstream of the proportional valves. Switching it changes
the downstream flow resistance, which causes a pressure transient across the
proportional valve. The PID can't anticipate this disturbance from the flow
signal alone, so it sees a sudden error after the fact, the integral winds up
during the transient, and the controller over-corrects before settling.

The spike is physically pressure-driven, not a control instability.

## What we tried
- Increased Kp/Ki/Kd and bumped the PID update rate from 200 Hz to 400 Hz
  (Kp 2.0->3.5, Ki 0.75->1.5, Kd 0.18->0.25). Did not improve the spikes.
  Reverted.

## Why a Python-only "hold the proportional valves for 100 ms across the
## end-valve switch" doesn't work today
`ProportionalValveControl::set_pid_enabled(false)` in
`firmware/inc/proportional_valve_control.h:47-54` is a HARD SAFETY OFF: it
zeros the duty cycle, sets the hold output to 0, and de-energizes the valve.
`update()` in `firmware/src/proportional_valve_control.cpp:127-136` then
keeps rewriting `duty_cycle_ = 0` on every tick. So you can't freeze the
duty cycle from Python by toggling enable or by writing the DutyCycle
register; the firmware overrides it.

## Remediation options (ranked by leverage)

1. **Mechanical fix** — add a bypass / dummy load that engages when the end
   valve closes, so total downstream resistance stays roughly constant.
   Most robust; requires hardware.

2. **Feedforward correction in software** — characterize the steady-state
   duty cycle the PID converges to in each end-valve state (open vs closed).
   At the moment of the end-valve switch, write the new steady-state duty
   cycle as a step so the PID only cleans up the residual instead of
   chasing the disturbance from scratch. Requires either:
   - Firmware support for an open-loop bias that survives a PID tick, or
   - A "PID pause + manual duty cycle" mode in firmware.
   Needs offline characterization.

3. **PID pause/freeze in firmware** — add a new control state that
   suspends PID computation while preserving `duty_cycle_` and keeping the
   valve energized. Python triggers pause just before each end-valve
   transition and clears it ~100 ms later. ~10 lines of firmware plus a
   new register. Directly suppresses the over-correction without trying
   to predict it.

## Slew-rate limiter (related, but separate issue)
`firmware/src/proportional_valve_control.cpp:109` hard-codes
`du_max_per_s = 0.6f`, which caps how fast the PID can change duty cycle.
This is the main reason gain bumps in Python didn't help. Raising this
would speed up recovery *after* the spike, but won't prevent the spike
itself.
