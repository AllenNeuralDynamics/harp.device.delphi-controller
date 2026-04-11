import customtkinter as ctk
from widgets.tile import Tile
from utils import bind_scroll_wheel


_CHANNEL_TYPES = ("Analog", "Flow Meter 100mL", "Flow Meter 1L")


class FlowAdcTab(ctk.CTkFrame):
    def __init__(self, master, **kwargs):
        super().__init__(master, fg_color="transparent", **kwargs)

        self.grid_columnconfigure(0, weight=0, minsize=280)
        self.grid_columnconfigure(1, weight=1)
        self.grid_rowconfigure(0, weight=1)

        self._selected_index: int | None = None
        self._channel_rows: list[dict] = []
        self.on_change: callable | None = None

        self._build_channel_list()
        self._build_detail_panel()

    # ── Left: Channel List ─────────────────────────────────────────────────────

    def _build_channel_list(self):
        tile = Tile(self, title="ADC Channels")
        tile.grid(row=0, column=0, sticky="nsew", padx=(0, 6))

        # Header
        header = ctk.CTkFrame(tile.content, fg_color="transparent")
        header.pack(fill="x", pady=(0, 4))
        for text, width in (("#", 28), ("Name", 110), ("Type", 72), ("Plot", 36)):
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

        for i in range(8):
            self._add_channel_row(scroll, i)

        bind_scroll_wheel(scroll)  # must be called after all rows are added

    def _add_channel_row(self, parent, index: int):
        select = lambda e, idx=index: self._select_channel(idx)

        row_frame = ctk.CTkFrame(parent, fg_color="transparent", cursor="hand2")
        row_frame.pack(fill="x", pady=1)
        row_frame.bind("<Button-1>", select)

        idx_label = ctk.CTkLabel(row_frame, text=str(index), width=28, anchor="w")
        idx_label.pack(side="left", padx=(0, 4))
        idx_label.bind("<Button-1>", select)

        name_var = ctk.StringVar(value=f"Channel {index}")
        name_entry = ctk.CTkEntry(row_frame, textvariable=name_var, width=110)
        name_entry.pack(side="left", padx=(0, 4))
        name_entry.bind("<FocusIn>", select, add="+")

        type_badge = ctk.CTkLabel(row_frame, text="ADC", width=72, anchor="w", text_color=("gray40", "gray60"))
        type_badge.pack(side="left", padx=(0, 4))
        type_badge.bind("<Button-1>", select)

        plot_var = ctk.BooleanVar(value=index < 2)
        plot_var.trace_add("write", lambda *_: self._on_config_change())
        ctk.CTkCheckBox(row_frame, variable=plot_var, text="", width=36, checkbox_width=18, checkbox_height=18).pack(side="left")

        name_entry.bind("<FocusOut>", lambda e: self._on_config_change())
        name_entry.bind("<Return>", lambda e: self._on_config_change())

        self._channel_rows.append({
            "frame": row_frame,
            "name_var": name_var,
            "type_badge": type_badge,
            "plot_var": plot_var,
        })

    # ── Right: Detail Panel ────────────────────────────────────────────────────

    def _build_detail_panel(self):
        self._detail_tile = Tile(self, title="Channel Detail")
        self._detail_tile.grid(row=0, column=1, sticky="nsew", padx=(6, 0))

        self._placeholder = ctk.CTkLabel(
            self._detail_tile.content,
            text="Select a channel from the list",
            text_color=("gray40", "gray60"),
        )
        self._placeholder.place(relx=0.5, rely=0.5, anchor="center")

        self._detail_frame = ctk.CTkFrame(self._detail_tile.content, fg_color="transparent")
        self._build_detail_contents(self._detail_frame)

    def _build_detail_contents(self, parent):
        # Type selector
        type_row = ctk.CTkFrame(parent, fg_color="transparent")
        type_row.pack(fill="x", pady=(0, 12))
        ctk.CTkLabel(type_row, text="Type", anchor="w", width=180).pack(side="left")
        self._type_var = ctk.StringVar(value="Analog")
        ctk.CTkOptionMenu(
            type_row,
            variable=self._type_var,
            values=list(_CHANNEL_TYPES),
            width=180,
            command=self._on_type_change,
        ).pack(side="left", padx=(8, 0))

        sep = ctk.CTkFrame(parent, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
        sep.pack(fill="x", pady=(0, 12))

        # Conversion equation
        ctk.CTkLabel(parent, text="Conversion Equation", font=ctk.CTkFont(weight="bold"), anchor="w").pack(fill="x")
        for label_text, key in (("Slope", "slope"), ("Offset", "offset")):
            row = ctk.CTkFrame(parent, fg_color="transparent")
            row.pack(fill="x", pady=3)
            ctk.CTkLabel(row, text=label_text, anchor="w", width=180).pack(side="left")
            ctk.CTkEntry(row, placeholder_text="1.0", width=120).pack(side="left", padx=(8, 0))
        ctk.CTkLabel(
            parent,
            text="value = slope × adc_counts + offset",
            font=ctk.CTkFont(size=11),
            text_color=("gray40", "gray60"),
            anchor="w",
        ).pack(fill="x", pady=(2, 12))

        sep2 = ctk.CTkFrame(parent, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
        sep2.pack(fill="x", pady=(0, 12))

        # Leak detection
        ctk.CTkLabel(parent, text="Leak Detection", font=ctk.CTkFont(weight="bold"), anchor="w").pack(fill="x")
        leak_enable_row = ctk.CTkFrame(parent, fg_color="transparent")
        leak_enable_row.pack(fill="x", pady=(4, 6))
        ctk.CTkLabel(leak_enable_row, text="Enable", anchor="w", width=180).pack(side="left")
        ctk.CTkCheckBox(leak_enable_row, text="").pack(side="left", padx=(8, 0))

        for label_text in ("Min (mL/min)", "Max (mL/min)"):
            row = ctk.CTkFrame(parent, fg_color="transparent")
            row.pack(fill="x", pady=3)
            ctk.CTkLabel(row, text=label_text, anchor="w", width=180).pack(side="left")
            ctk.CTkEntry(row, placeholder_text="0.00", width=120).pack(side="left", padx=(8, 0))

        sep3 = ctk.CTkFrame(parent, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
        sep3.pack(fill="x", pady=(12, 12))

        # Calibration placeholder
        ctk.CTkLabel(parent, text="Calibration", font=ctk.CTkFont(weight="bold"), anchor="w").pack(fill="x")
        ctk.CTkLabel(
            parent,
            text="Coming soon",
            text_color=("gray40", "gray60"),
            anchor="w",
        ).pack(fill="x", pady=(4, 0))
        ctk.CTkButton(parent, text="Run Calibration", state="disabled", width=160).pack(anchor="w", pady=(6, 0))

    # ── Interaction ────────────────────────────────────────────────────────────

    def _select_channel(self, index: int):
        self._selected_index = index
        self._placeholder.place_forget()
        self._detail_frame.pack(fill="both", expand=True)

    def _on_type_change(self, value: str):
        if self._selected_index is None:
            return
        badge_text = {"Analog": "ADC", "Flow Meter 100mL": "FM100", "Flow Meter 1L": "FM1L"}
        self._channel_rows[self._selected_index]["type_badge"].configure(
            text=badge_text.get(value, "ADC")
        )
        # TODO: update channel_config.json

    def _on_config_change(self):
        if self.on_change:
            self.on_change(self.get_channel_configs())

    def get_channel_configs(self) -> list[dict]:
        """Return current config for all 8 channels."""
        return [
            {"name": row["name_var"].get(), "plot_enabled": row["plot_var"].get()}
            for row in self._channel_rows
        ]
