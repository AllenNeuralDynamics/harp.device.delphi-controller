import customtkinter as ctk
import serial.tools.list_ports
from widgets.tile import Tile
from utils import bind_scroll_wheel


class ConfigTab(ctk.CTkFrame):
    def __init__(self, master, **kwargs):
        super().__init__(master, fg_color="transparent", **kwargs)

        self.grid_columnconfigure(0, weight=1)
        self.grid_rowconfigure(0, weight=1)

        scroll = ctk.CTkScrollableFrame(self, fg_color="transparent")
        scroll.grid(row=0, column=0, sticky="nsew")
        scroll.grid_columnconfigure(0, weight=1)

        self._build_connection_card(scroll)
        self._build_camera_card(scroll)
        self._build_pid_card(scroll)
        self._build_poke_card(scroll)
        self._build_odor_card(scroll)

        bind_scroll_wheel(scroll)  # must be called after all cards are added

    # ── Connection ─────────────────────────────────────────────────────────────

    def _build_connection_card(self, parent):
        tile = Tile(parent, title="Connection")
        tile.pack(fill="x", pady=(0, 8))

        # Port row
        port_row = ctk.CTkFrame(tile.content, fg_color="transparent")
        port_row.pack(fill="x", pady=(0, 6))

        ctk.CTkLabel(port_row, text="Port", anchor="w", width=120).pack(side="left")

        self._port_var = ctk.StringVar()
        self._port_menu = ctk.CTkOptionMenu(port_row, variable=self._port_var, values=["—"], width=160)
        self._port_menu.pack(side="left", padx=(0, 8))

        ctk.CTkButton(port_row, text="Refresh", width=80, command=self._refresh_ports).pack(side="left")

        # Connect row
        connect_row = ctk.CTkFrame(tile.content, fg_color="transparent")
        connect_row.pack(fill="x", pady=(0, 8))
        ctk.CTkLabel(connect_row, text="", width=120).pack(side="left")  # spacer

        self._connect_btn = ctk.CTkButton(
            connect_row, text="Connect", width=120, command=self._on_connect
        )
        self._connect_btn.pack(side="left", padx=(0, 12))

        self._status_label = ctk.CTkLabel(connect_row, text="● Disconnected", text_color="#e05555")
        self._status_label.pack(side="left")

        # Device info readout
        sep = ctk.CTkFrame(tile.content, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
        sep.pack(fill="x", pady=(0, 8))

        self._device_info = ctk.CTkTextbox(tile.content, height=60, state="disabled")
        self._device_info.pack(fill="x")

        # CSV logging row
        sep2 = ctk.CTkFrame(tile.content, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
        sep2.pack(fill="x", pady=(8, 8))

        csv_row = ctk.CTkFrame(tile.content, fg_color="transparent")
        csv_row.pack(fill="x")
        ctk.CTkLabel(csv_row, text="CSV Logging", anchor="w", width=120).pack(side="left")
        self._log_btn = ctk.CTkButton(csv_row, text="Start Logging", width=120, command=self._on_log_toggle)
        self._log_btn.pack(side="left", padx=(0, 12))
        self._log_path_label = ctk.CTkLabel(csv_row, text="", text_color=("gray40", "gray60"))
        self._log_path_label.pack(side="left")

        self._is_connected = False
        self._is_logging = False
        self._refresh_ports()

    def _refresh_ports(self):
        ports = [p.device for p in serial.tools.list_ports.comports()]
        if not ports:
            ports = ["No ports found"]
        self._port_menu.configure(values=ports)
        self._port_var.set(ports[0])

    def _on_connect(self):
        if self._is_connected:
            # TODO: device_manager.disconnect()
            self._is_connected = False
            self._connect_btn.configure(text="Connect")
            self._status_label.configure(text="● Disconnected", text_color="#e05555")
        else:
            port = self._port_var.get()
            # TODO: device_manager.connect(port)
            self._is_connected = True
            self._connect_btn.configure(text="Disconnect")
            self._status_label.configure(text="● Connected", text_color="#55cc77")
            self._device_info.configure(state="normal")
            self._device_info.delete("1.0", "end")
            self._device_info.insert("1.0", f"Connected to {port}\n[device info will appear here]")
            self._device_info.configure(state="disabled")

    def _on_log_toggle(self):
        if self._is_logging:
            # TODO: data_logger.stop()
            self._is_logging = False
            self._log_btn.configure(text="Start Logging")
            self._log_path_label.configure(text="")
        else:
            # TODO: data_logger.start()
            self._is_logging = True
            self._log_btn.configure(text="Stop Logging")
            self._log_path_label.configure(text="delphi_log_<timestamp>.csv")

    # ── Camera Triggers ────────────────────────────────────────────────────────

    def _build_camera_card(self, parent):
        tile = Tile(parent, title="Camera Triggers")
        tile.pack(fill="x", pady=(0, 8))

        for cam_idx in range(2):
            ctk.CTkLabel(
                tile.content,
                text=f"Cam {cam_idx}",
                font=ctk.CTkFont(weight="bold"),
                anchor="w",
            ).pack(fill="x", pady=(0 if cam_idx == 0 else 8, 4))

            for label_text, placeholder in (
                ("Frame Rate (fps)", "30"),
                ("Duty Cycle (0–1)", "0.50"),
            ):
                row = ctk.CTkFrame(tile.content, fg_color="transparent")
                row.pack(fill="x", pady=2)
                ctk.CTkLabel(row, text=label_text, anchor="w", width=180).pack(side="left")
                ctk.CTkEntry(row, placeholder_text=placeholder, width=120).pack(side="left", padx=(8, 0))

            enable_row = ctk.CTkFrame(tile.content, fg_color="transparent")
            enable_row.pack(fill="x", pady=2)
            ctk.CTkLabel(enable_row, text="Enable", anchor="w", width=180).pack(side="left")
            ctk.CTkSwitch(enable_row, text="").pack(side="left", padx=(8, 0))

            pin_row = ctk.CTkFrame(tile.content, fg_color="transparent")
            pin_row.pack(fill="x", pady=2)
            ctk.CTkLabel(pin_row, text="Pin State (read-only)", anchor="w", width=180).pack(side="left")
            ctk.CTkLabel(pin_row, text="--", text_color=("gray40", "gray60")).pack(side="left", padx=(8, 0))

    # ── PID Gains ──────────────────────────────────────────────────────────────

    def _build_pid_card(self, parent):
        tile = Tile(parent, title="PID Gains")
        tile.pack(fill="x", pady=(0, 8))

        for label_text, placeholder in (
            ("Update Rate (Hz)", "100.0"),
            ("Kp", "1.000"),
            ("Ki", "0.100"),
            ("Kd", "0.010"),
        ):
            row = ctk.CTkFrame(tile.content, fg_color="transparent")
            row.pack(fill="x", pady=3)
            ctk.CTkLabel(row, text=label_text, anchor="w", width=180).pack(side="left")
            ctk.CTkEntry(row, placeholder_text=placeholder, width=120).pack(side="left", padx=(8, 0))

        ctk.CTkButton(tile.content, text="Write Gains", width=120, command=self._on_write_pid).pack(
            anchor="w", pady=(8, 0)
        )

    def _on_write_pid(self):
        # TODO: pack_pid_config(kp, ki, kd) → WriteHarpMessage
        pass

    # ── Poke Port ──────────────────────────────────────────────────────────────

    def _build_poke_card(self, parent):
        tile = Tile(parent, title="Poke Port")
        tile.pack(fill="x", pady=(0, 8))

        for label_text, widget_type, placeholder in (
            ("Pin (0–7)", "entry", "0"),
            ("Inverted", "switch", None),
        ):
            row = ctk.CTkFrame(tile.content, fg_color="transparent")
            row.pack(fill="x", pady=3)
            ctk.CTkLabel(row, text=label_text, anchor="w", width=180).pack(side="left")
            if widget_type == "entry":
                ctk.CTkEntry(row, placeholder_text=placeholder, width=120).pack(side="left", padx=(8, 0))
            else:
                ctk.CTkSwitch(row, text="").pack(side="left", padx=(8, 0))

        sep = ctk.CTkFrame(tile.content, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
        sep.pack(fill="x", pady=(6, 8))

        for label_text in ("Poke State", "Raw Poke State", "Poke Count"):
            row = ctk.CTkFrame(tile.content, fg_color="transparent")
            row.pack(fill="x", pady=2)
            ctk.CTkLabel(row, text=label_text, anchor="w", width=180).pack(side="left")
            ctk.CTkLabel(row, text="--", text_color=("gray40", "gray60")).pack(side="left", padx=(8, 0))

    # ── Odor Mask ──────────────────────────────────────────────────────────────

    def _build_odor_card(self, parent):
        tile = Tile(parent, title="Odor Mask")
        tile.pack(fill="x", pady=(0, 8))

        ctk.CTkLabel(tile.content, text="Valve Selection (QueuedOdorMask)", anchor="w").pack(fill="x", pady=(0, 6))

        # 16 checkboxes in two rows of 8
        self._odor_vars: list[ctk.BooleanVar] = []
        for group_start in (0, 8):
            row = ctk.CTkFrame(tile.content, fg_color="transparent")
            row.pack(fill="x", pady=2)
            for i in range(group_start, group_start + 8):
                col_frame = ctk.CTkFrame(row, fg_color="transparent")
                col_frame.pack(side="left", padx=6)
                var = ctk.BooleanVar(value=False)
                ctk.CTkCheckBox(col_frame, variable=var, text="", checkbox_width=18, checkbox_height=18).pack()
                ctk.CTkLabel(col_frame, text=str(i), font=ctk.CTkFont(size=10)).pack()
                self._odor_vars.append(var)

        ctk.CTkButton(tile.content, text="Write Mask", width=120, command=self._on_write_odor_mask).pack(
            anchor="w", pady=(8, 0)
        )

        sep = ctk.CTkFrame(tile.content, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
        sep.pack(fill="x", pady=(10, 8))

        ctk.CTkLabel(tile.content, text="Timing", font=ctk.CTkFont(weight="bold"), anchor="w").pack(fill="x")

        timing_fields = [
            ("Odor Setup Time (µs)", "1000000"),
            ("Min Delivery Time (µs)", "2000000"),
            ("Max Delivery Time (µs)", "5000000"),
            ("Min Poke Time (µs)", "500000"),
            ("Dwell Time (µs)", "1000000"),
        ]
        for label_text, placeholder in timing_fields:
            row = ctk.CTkFrame(tile.content, fg_color="transparent")
            row.pack(fill="x", pady=3)
            ctk.CTkLabel(row, text=label_text, anchor="w", width=220).pack(side="left")
            ctk.CTkEntry(row, placeholder_text=placeholder, width=140).pack(side="left", padx=(8, 0))

    def _on_write_odor_mask(self):
        # TODO: compute bitmask, WriteU16(QueuedOdorMask, mask)
        pass
