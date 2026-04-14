import logging
import struct
import customtkinter as ctk
from pyharp.messages import HarpMessage, WriteHarpMessage, PayloadType
from app_registers_refactor import AppRegs, DelphiOnlyAppRegs
from widgets.tile import Tile
from utils import bind_scroll_wheel

logger = logging.getLogger(__name__)

_VALVE_TYPES = ("On/Off", "Proportional")

# (Adc, EnablePid, DutyCycle, TargetFlowRate) registers for proportional valves 0–2
_PROP_REGS = [
    (DelphiOnlyAppRegs.ProportionalValve0Adc, DelphiOnlyAppRegs.ProportionalValve0EnablePid,
     DelphiOnlyAppRegs.ProportionalValve0DutyCycle, DelphiOnlyAppRegs.ProportionalValve0TargetFlowRate),
    (DelphiOnlyAppRegs.ProportionalValve1Adc, DelphiOnlyAppRegs.ProportionalValve1EnablePid,
     DelphiOnlyAppRegs.ProportionalValve1DutyCycle, DelphiOnlyAppRegs.ProportionalValve1TargetFlowRate),
    (DelphiOnlyAppRegs.ProportionalValve2Adc, DelphiOnlyAppRegs.ProportionalValve2EnablePid,
     DelphiOnlyAppRegs.ProportionalValve2DutyCycle, DelphiOnlyAppRegs.ProportionalValve2TargetFlowRate),
]


class ValvesTab(ctk.CTkFrame):
    def __init__(self, master, device_manager=None, **kwargs):
        super().__init__(master, fg_color="transparent", **kwargs)
        self._dm = device_manager

        self.grid_columnconfigure(0, weight=0, minsize=360)
        self.grid_columnconfigure(1, weight=1)
        self.grid_rowconfigure(0, weight=1)

        self._selected_index: int | None = None
        self._valve_rows: list[dict] = []
        self.on_change: callable | None = None

        self._build_channel_list()
        self._build_detail_panel()

    # ── Left: Channel List ─────────────────────────────────────────────────────

    def _build_channel_list(self):
        tile = Tile(self, title="Valve Channels")
        tile.grid(row=0, column=0, sticky="nsew", padx=(0, 6))

        # Header row
        header = ctk.CTkFrame(tile.content, fg_color="transparent")
        header.pack(fill="x", pady=(0, 4))
        for text, width in (("#", 28), ("Name", 110), ("Type", 130), ("Dashboard", 80)):
            ctk.CTkLabel(
                header,
                text=text,
                width=width,
                anchor="w",
                font=ctk.CTkFont(size=11),
                text_color=("gray40", "gray60"),
            ).pack(side="left", padx=(0, 4))

        sep = ctk.CTkFrame(tile.content, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
        sep.pack(fill="x", pady=(0, 4))

        scroll = ctk.CTkScrollableFrame(tile.content, fg_color="transparent")
        scroll.pack(fill="both", expand=True)

        for i in range(16):
            self._add_valve_row(scroll, i)

        bind_scroll_wheel(scroll)

    def _add_valve_row(self, parent, index: int):
        select = lambda e, idx=index: self._select_channel(idx)

        row_frame = ctk.CTkFrame(parent, fg_color="transparent", cursor="hand2")
        row_frame.pack(fill="x", pady=1)
        row_frame.bind("<Button-1>", select)

        idx_label = ctk.CTkLabel(row_frame, text=str(index), width=28, anchor="w")
        idx_label.pack(side="left", padx=(0, 4))
        idx_label.bind("<Button-1>", select)

        name_var = ctk.StringVar(value=f"Valve {index}")
        name_entry = ctk.CTkEntry(row_frame, textvariable=name_var, width=110)
        name_entry.pack(side="left", padx=(0, 4))
        name_entry.bind("<FocusIn>", select, add="+")

        type_var = ctk.StringVar(value="On/Off")
        type_menu = ctk.CTkOptionMenu(
            row_frame,
            variable=type_var,
            values=list(_VALVE_TYPES),
            width=130,
            command=lambda val, idx=index: self._on_type_change(idx, val),
        )
        type_menu.pack(side="left", padx=(0, 4))
        type_menu.bind("<Button-1>", select, add="+")

        dash_var = ctk.BooleanVar(value=index < 4)
        dash_var.trace_add("write", lambda *_: self._on_config_change())
        dash_cb = ctk.CTkCheckBox(row_frame, variable=dash_var, text="", width=80,
                                   checkbox_width=18, checkbox_height=18)
        dash_cb.pack(side="left")

        name_entry.bind("<FocusOut>", lambda e: self._on_config_change())
        name_entry.bind("<Return>", lambda e: self._on_config_change())

        self._valve_rows.append({
            "frame": row_frame,
            "name_var": name_var,
            "type_var": type_var,
            "dash_var": dash_var,
        })

    # ── Right: Detail Panel ────────────────────────────────────────────────────

    def _build_detail_panel(self):
        self._detail_tile = Tile(self, title="Channel Detail")
        self._detail_tile.grid(row=0, column=1, sticky="nsew", padx=(6, 0))

        self._detail_placeholder = ctk.CTkLabel(
            self._detail_tile.content,
            text="Select a channel from the list",
            text_color=("gray40", "gray60"),
        )
        self._detail_placeholder.place(relx=0.5, rely=0.5, anchor="center")

        # ── On/Off detail ──
        self._onoff_frame = ctk.CTkFrame(self._detail_tile.content, fg_color="transparent")

        self._onoff_switch = ctk.CTkSwitch(self._onoff_frame, text="Active",
                                            command=self._on_onoff_toggle)
        self._onoff_switch.pack(anchor="w", pady=(4, 2))

        self._onoff_status = ctk.CTkLabel(
            self._onoff_frame, text="", font=ctk.CTkFont(size=11),
            text_color=("gray40", "gray60"),
        )
        self._onoff_status.pack(anchor="w", pady=(0, 8))

        sep = ctk.CTkFrame(self._onoff_frame, height=1, corner_radius=0,
                           fg_color=("gray70", "gray35"))
        sep.pack(fill="x", pady=(0, 8))

        ctk.CTkLabel(self._onoff_frame, text="Valve Config (ValveConfigs register)",
                     font=ctk.CTkFont(weight="bold"), anchor="w").pack(fill="x", pady=(0, 4))

        for label, attr, default in (
            ("Hit Output (0–1)", "_onoff_hit_var", "1.00"),
            ("Hold Output (0–1)", "_onoff_hold_var", "1.00"),
            ("Hit Duration (µs, 0=hold)", "_onoff_dur_var", "0"),
        ):
            row = ctk.CTkFrame(self._onoff_frame, fg_color="transparent")
            row.pack(fill="x", pady=3)
            ctk.CTkLabel(row, text=label, anchor="w", width=200).pack(side="left")
            var = ctk.StringVar(value=default)
            setattr(self, attr, var)
            ctk.CTkEntry(row, textvariable=var, width=100).pack(side="left", padx=(8, 0))

        ctk.CTkButton(self._onoff_frame, text="Write Config", width=120,
                      command=self._on_write_valve_config).pack(anchor="w", pady=(8, 0))

        # ── Proportional detail ──
        self._prop_frame = ctk.CTkFrame(self._detail_tile.content, fg_color="transparent")
        self._build_proportional_detail(self._prop_frame)

    def _build_proportional_detail(self, parent):
        # Warning — only visible for channels > 2
        self._prop_warning = ctk.CTkLabel(
            parent,
            text="⚠  Proportional control only supported on channels 0–2",
            text_color="#e09955",
        )

        # ADC Channel
        adc_row = ctk.CTkFrame(parent, fg_color="transparent")
        adc_row.pack(fill="x", pady=4)
        ctk.CTkLabel(adc_row, text="ADC Channel", anchor="w", width=200).pack(side="left")
        self._prop_adc_var = ctk.StringVar(value="0")
        ctk.CTkOptionMenu(
            adc_row,
            variable=self._prop_adc_var,
            values=[str(i) for i in range(8)],
            width=100,
            command=lambda _: self._on_prop_adc_change(),
        ).pack(side="left", padx=(8, 0))

        # PID Enabled
        pid_row = ctk.CTkFrame(parent, fg_color="transparent")
        pid_row.pack(fill="x", pady=4)
        ctk.CTkLabel(pid_row, text="PID Enabled", anchor="w", width=200).pack(side="left")
        self._prop_pid_switch = ctk.CTkSwitch(pid_row, text="",
                                               command=self._on_prop_pid_toggle)
        self._prop_pid_switch.pack(side="left", padx=(8, 0))

        # Duty Cycle (read-only, updated from poll)
        dc_row = ctk.CTkFrame(parent, fg_color="transparent")
        dc_row.pack(fill="x", pady=4)
        ctk.CTkLabel(dc_row, text="Duty Cycle", anchor="w", width=200).pack(side="left")
        self._prop_duty_label = ctk.CTkLabel(dc_row, text="--", text_color=("gray40", "gray60"))
        self._prop_duty_label.pack(side="left", padx=(8, 0))

        # Target Flow Rate
        target_row = ctk.CTkFrame(parent, fg_color="transparent")
        target_row.pack(fill="x", pady=4)
        ctk.CTkLabel(target_row, text="Target Flow Rate (mL/min)", anchor="w", width=200).pack(side="left")
        self._prop_target_var = ctk.StringVar(value="0.00")
        self._prop_target_entry = ctk.CTkEntry(target_row, textvariable=self._prop_target_var, width=100)
        self._prop_target_entry.pack(side="left", padx=(8, 0))
        self._prop_target_entry.bind("<Return>", lambda e: self._on_prop_target_change())
        self._prop_target_entry.bind("<FocusOut>", lambda e: self._on_prop_target_change())

    # ── Interaction ────────────────────────────────────────────────────────────

    def _select_channel(self, index: int):
        self._selected_index = index
        valve_type = self._valve_rows[index]["type_var"].get()

        self._detail_placeholder.place_forget()
        self._onoff_frame.pack_forget()
        self._prop_frame.pack_forget()

        if valve_type == "On/Off":
            self._onoff_frame.pack(fill="both", expand=True)
            self._populate_onoff(index)
        else:
            if index > 2:
                self._prop_warning.pack(anchor="w", pady=(0, 8))
            else:
                self._prop_warning.pack_forget()
                self._populate_prop(index)
            self._prop_frame.pack(fill="both", expand=True)

    def _populate_onoff(self, index: int):
        """Init the on/off switch and config fields from hardware state if connected."""
        if self._dm is None or not self._dm.is_connected:
            return
        try:
            reply = self._dm.send(HarpMessage.ReadU16(AppRegs.ValvesState))
            bitmask = int(reply.payload[0])
            if (bitmask >> index) & 1:
                self._onoff_switch.select()
            else:
                self._onoff_switch.deselect()
        except Exception as exc:
            logger.warning("Could not read ValvesState: %s", exc)
        try:
            cfg_reg = AppRegs.ValveConfigs0 + index
            reply = self._dm.send(HarpMessage.ReadU8(cfg_reg))
            raw = bytes(bytearray(reply.payload))
            if len(raw) >= 12:
                hit, hold, dur = struct.unpack_from("<ffI", raw)
                self._onoff_hit_var.set(f"{hit:.2f}")
                self._onoff_hold_var.set(f"{hold:.2f}")
                self._onoff_dur_var.set(str(dur))
        except Exception as exc:
            logger.warning("Could not read ValveConfigs%d: %s", index, exc)

    def _populate_prop(self, index: int):
        """Populate proportional detail widgets from hardware registers."""
        if self._dm is None or not self._dm.is_connected:
            return
        adc_reg, pid_reg, _dc_reg, target_reg = _PROP_REGS[index]
        try:
            adc_val = int(self._dm.send(HarpMessage.ReadU8(adc_reg)).payload[0])
            self._prop_adc_var.set(str(adc_val))
        except Exception as exc:
            logger.warning("Could not read ProportionalValve%dAdc: %s", index, exc)
        try:
            pid_val = int(self._dm.send(HarpMessage.ReadU8(pid_reg)).payload[0])
            if pid_val:
                self._prop_pid_switch.select()
            else:
                self._prop_pid_switch.deselect()
        except Exception as exc:
            logger.warning("Could not read ProportionalValve%dEnablePid: %s", index, exc)
        try:
            target_val = float(self._dm.send(HarpMessage.ReadFloat(target_reg)).payload[0])
            self._prop_target_var.set(f"{target_val:.2f}")
        except Exception as exc:
            logger.warning("Could not read ProportionalValve%dTargetFlowRate: %s", index, exc)

    def _on_type_change(self, index: int, value: str):
        if self._selected_index == index:
            self._select_channel(index)

    def _on_config_change(self):
        if self.on_change:
            self.on_change(self.get_valve_configs())

    # ── Hardware writes ────────────────────────────────────────────────────────

    def _on_onoff_toggle(self):
        if self._selected_index is None or self._dm is None or not self._dm.is_connected:
            self._onoff_status.configure(
                text="Not connected" if (self._dm is None or not self._dm.is_connected) else "",
                text_color="#e05555",
            )
            return
        state = self._onoff_switch.get()
        try:
            if state:
                self._dm.send(HarpMessage.WriteU16(AppRegs.ValvesSet, 1 << self._selected_index))
            else:
                self._dm.send(HarpMessage.WriteU16(AppRegs.ValvesClear, 1 << self._selected_index))
            self._onoff_status.configure(
                text=f"Sent {'Set' if state else 'Clear'} bit {self._selected_index}",
                text_color="#55cc77",
            )
        except Exception as exc:
            logger.warning("On/Off toggle error (valve %d): %s", self._selected_index, exc)
            self._onoff_status.configure(text=f"Error: {exc}", text_color="#e05555")

    def _on_write_valve_config(self):
        if self._selected_index is None or self._dm is None or not self._dm.is_connected:
            self._onoff_status.configure(text="Not connected", text_color="#e05555")
            return
        try:
            hit = float(self._onoff_hit_var.get())
            hold = float(self._onoff_hold_var.get())
            dur = int(self._onoff_dur_var.get())
        except ValueError as exc:
            self._onoff_status.configure(text=f"Invalid value: {exc}", text_color="#e05555")
            return
        try:
            payload = struct.pack("<ffI", hit, hold, dur)
            cfg_reg = AppRegs.ValveConfigs0 + self._selected_index
            msg = WriteHarpMessage(PayloadType.U8, payload, cfg_reg, offset=len(payload) - 1)
            self._dm.send(msg)
            self._onoff_status.configure(
                text=f"Config written (hit={hit:.2f}, hold={hold:.2f}, dur={dur}µs)",
                text_color="#55cc77",
            )
        except Exception as exc:
            logger.warning("ValveConfig write error (valve %d): %s", self._selected_index, exc)
            self._onoff_status.configure(text=f"Error: {exc}", text_color="#e05555")

    def _on_prop_adc_change(self):
        if self._selected_index is None or self._selected_index > 2:
            return
        if self._dm is None or not self._dm.is_connected:
            return
        adc_reg = _PROP_REGS[self._selected_index][0]
        try:
            val = int(self._prop_adc_var.get())
            self._dm.send(HarpMessage.WriteU8(adc_reg, val))
        except Exception as exc:
            logger.warning("Could not write ProportionalValve%dAdc: %s", self._selected_index, exc)

    def _on_prop_pid_toggle(self):
        if self._selected_index is None or self._selected_index > 2:
            return
        if self._dm is None or not self._dm.is_connected:
            return
        pid_reg = _PROP_REGS[self._selected_index][1]
        try:
            self._dm.send(HarpMessage.WriteU8(pid_reg, int(self._prop_pid_switch.get())))
        except Exception as exc:
            logger.warning("Could not write ProportionalValve%dEnablePid: %s", self._selected_index, exc)

    def _on_prop_target_change(self):
        if self._selected_index is None or self._selected_index > 2:
            return
        if self._dm is None or not self._dm.is_connected:
            return
        target_reg = _PROP_REGS[self._selected_index][3]
        try:
            val = float(self._prop_target_var.get())
            self._dm.send(HarpMessage.WriteFloat(target_reg, val))
        except Exception as exc:
            logger.warning("Could not write ProportionalValve%dTargetFlowRate: %s", self._selected_index, exc)

    # ── Poll drain ─────────────────────────────────────────────────────────────

    def update_duty_cycles(self, duty_cycles: list[float]):
        """Update the duty cycle readout if a proportional valve is currently selected."""
        if (self._selected_index is None
                or self._selected_index > 2
                or self._valve_rows[self._selected_index]["type_var"].get() != "Proportional"):
            return
        idx = self._selected_index
        if idx < len(duty_cycles):
            self._prop_duty_label.configure(text=f"{duty_cycles[idx] * 100:.1f}%")

    # ── Public API ─────────────────────────────────────────────────────────────

    def get_valve_configs(self) -> list[dict]:
        return [
            {"index": i, "name": row["name_var"].get(), "dash_enabled": row["dash_var"].get()}
            for i, row in enumerate(self._valve_rows)
        ]
