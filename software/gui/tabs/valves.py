import customtkinter as ctk
from widgets.tile import Tile
from utils import bind_scroll_wheel


_VALVE_TYPES = ("On/Off", "Proportional")


class ValvesTab(ctk.CTkFrame):
    def __init__(self, master, **kwargs):
        super().__init__(master, fg_color="transparent", **kwargs)

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

        bind_scroll_wheel(scroll)  # must be called after all rows are added

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
        dash_cb = ctk.CTkCheckBox(row_frame, variable=dash_var, text="", width=80, checkbox_width=18, checkbox_height=18)
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

        # On/Off detail frame
        self._onoff_frame = ctk.CTkFrame(self._detail_tile.content, fg_color="transparent")
        self._onoff_switch = ctk.CTkSwitch(self._onoff_frame, text="Active", command=self._on_onoff_toggle)
        self._onoff_switch.pack(anchor="w", pady=4)

        # Proportional detail frame
        self._prop_frame = ctk.CTkFrame(self._detail_tile.content, fg_color="transparent")
        self._build_proportional_detail(self._prop_frame)

    def _build_proportional_detail(self, parent):
        fields = [
            ("ADC Channel", "0"),
            ("Target Flow Rate (mL/min)", "0.00"),
            ("Duty Cycle (read-only)", "--"),
        ]
        for label_text, default in fields:
            row = ctk.CTkFrame(parent, fg_color="transparent")
            row.pack(fill="x", pady=4)
            ctk.CTkLabel(row, text=label_text, anchor="w", width=200).pack(side="left")
            ctk.CTkEntry(row, placeholder_text=default, width=100).pack(side="left", padx=(8, 0))

        pid_row = ctk.CTkFrame(parent, fg_color="transparent")
        pid_row.pack(fill="x", pady=4)
        ctk.CTkLabel(pid_row, text="PID Enabled", anchor="w", width=200).pack(side="left")
        ctk.CTkSwitch(pid_row, text="").pack(side="left", padx=(8, 0))

    # ── Interaction ────────────────────────────────────────────────────────────

    def _select_channel(self, index: int):
        self._selected_index = index
        valve_type = self._valve_rows[index]["type_var"].get()
        name = self._valve_rows[index]["name_var"].get()
        self._detail_tile.configure(  # update tile title
        )
        self._detail_placeholder.place_forget()
        self._onoff_frame.pack_forget()
        self._prop_frame.pack_forget()

        if valve_type == "On/Off":
            self._onoff_frame.pack(fill="both", expand=True)
        else:
            if index > 2:
                ctk.CTkLabel(
                    self._prop_frame,
                    text=f"⚠  Proportional control only supported on channels 0–2",
                    text_color="#e09955",
                ).pack(anchor="w", pady=(0, 8))
            self._prop_frame.pack(fill="both", expand=True)

    def _on_config_change(self):
        if self.on_change:
            self.on_change(self.get_valve_configs())

    def get_valve_configs(self) -> list[dict]:
        """Return a list of dicts with name and dash_enabled for each valve."""
        return [
            {"index": i, "name": row["name_var"].get(), "dash_enabled": row["dash_var"].get()}
            for i, row in enumerate(self._valve_rows)
        ]

    def _on_type_change(self, index: int, value: str):
        if self._selected_index == index:
            self._select_channel(index)

    def _on_onoff_toggle(self):
        # TODO: send ValvesSet / ValvesClear
        pass
