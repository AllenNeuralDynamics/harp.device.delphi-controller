import math
import random
import customtkinter as ctk
from widgets.tile import Tile
from widgets.live_plot import LivePlot


class DashboardTab(ctk.CTkFrame):
    def __init__(self, master, **kwargs):
        super().__init__(master, fg_color="transparent", **kwargs)

        self.grid_columnconfigure(0, weight=1)
        self.grid_columnconfigure(1, weight=1)
        self.grid_rowconfigure(0, weight=0)
        self.grid_rowconfigure(1, weight=1)
        self.grid_rowconfigure(2, weight=0)  # leak banner

        self._build_flow_rates_card()
        self._build_valve_controls_card()
        self._build_plot_cards()
        self._build_leak_banner()
        self._demo_t = 0.0
        self._demo_tick_count = 0
        self._tick_demo()

    # ── Flow Rates ─────────────────────────────────────────────────────────────

    def _build_flow_rates_card(self):
        tile = Tile(self, title="Flow Rates")
        tile.grid(row=0, column=0, sticky="nsew", padx=(0, 4), pady=(0, 6))
        self._flow_rates_content = tile.content
        self._flow_labels: list[ctk.CTkLabel] = []

    # ── Valve Controls ─────────────────────────────────────────────────────────

    def _build_valve_controls_card(self):
        self._valve_tile = Tile(self, title="Valve Controls")
        self._valve_tile.grid(row=0, column=1, sticky="nsew", padx=(4, 0), pady=(0, 6))
        self._valve_content = self._valve_tile.content
        self._valve_switches: list[tuple[int, ctk.CTkSwitch]] = []  # (valve_index, switch)

        self._valve_placeholder = ctk.CTkLabel(
            self._valve_content,
            text="Select valves on the Valves tab",
            text_color=("gray40", "gray60"),
            font=ctk.CTkFont(size=11),
        )
        self._valve_placeholder.pack(anchor="w", pady=(0, 6))

    def sync_valves(self, configs: list[dict]):
        """Rebuild valve control switches from Valves tab config.

        configs: list of dicts with keys 'index', 'name', 'dash_enabled'.
        Only valves with dash_enabled=True appear here.
        """
        for widget in self._valve_content.winfo_children():
            widget.destroy()
        self._valve_switches.clear()

        enabled = [c for c in configs if c["dash_enabled"]]

        if not enabled:
            self._valve_placeholder = ctk.CTkLabel(
                self._valve_content,
                text="Select valves on the Valves tab",
                text_color=("gray40", "gray60"),
                font=ctk.CTkFont(size=11),
            )
            self._valve_placeholder.pack(anchor="w", pady=(0, 6))
            return

        for c in enabled:
            row = ctk.CTkFrame(self._valve_content, fg_color="transparent")
            row.pack(fill="x", pady=3)
            ctk.CTkLabel(row, text=c["name"], anchor="w", width=100).pack(side="left")
            sw = ctk.CTkSwitch(
                row, text="", width=46,
                command=lambda idx=c["index"]: self._on_valve_toggle(idx),
            )
            sw.pack(side="right")
            self._valve_switches.append((c["index"], sw))

    def _on_valve_toggle(self, valve_index: int):
        # TODO: send ValvesSet / ValvesClear harp message
        pass

    # ── Flow Plots ─────────────────────────────────────────────────────────────

    def _build_plot_cards(self):
        self._plot_frame = ctk.CTkFrame(self, fg_color="transparent")
        self._plot_frame.grid(row=1, column=0, columnspan=2, sticky="nsew", pady=(0, 6))
        self._plot_pool: list[LivePlot] = []  # all ever-created plots (never destroyed)
        self._plots: list[LivePlot] = []      # currently visible subset

    def sync_plots(self, configs: list[dict]):
        """Show/hide plots from the pool based on channel configs ({name, plot_enabled}).

        Plots are never destroyed — only hidden — to avoid crashing on pending
        Tkinter after_idle callbacks that matplotlib schedules via draw_idle().
        """
        enabled = [c for c in configs if c["plot_enabled"]]

        # Hide all pooled plots
        for p in self._plot_pool:
            p.grid_forget()

        # Grow pool if more slots are needed
        while len(self._plot_pool) < len(enabled):
            # title=" " (non-empty) ensures Tile creates the label widget so set_title() works
            p = LivePlot(self._plot_frame, title=" ", units="mL/min")
            self._plot_pool.append(p)

        self._plots = self._plot_pool[: len(enabled)]

        if not enabled:
            self._rebuild_flow_rate_labels([])
            return

        cols = 2 if len(enabled) > 1 else 1
        rows = math.ceil(len(enabled) / cols)

        for col in range(cols):
            self._plot_frame.grid_columnconfigure(col, weight=1)
        for row in range(rows):
            self._plot_frame.grid_rowconfigure(row, weight=1)

        for idx, c in enumerate(enabled):
            p = self._plots[idx]
            p.set_title(f"Flow — {c['name']}")
            p.reset()
            row_idx = idx // cols
            col_idx = idx % cols
            p.grid(
                row=row_idx,
                column=col_idx,
                sticky="nsew",
                padx=(0, 4 if col_idx < cols - 1 else 0),
                pady=(0, 4 if row_idx < rows - 1 else 0),
            )

        self._rebuild_flow_rate_labels([c["name"] for c in enabled])

    def _rebuild_flow_rate_labels(self, names: list[str]):
        for widget in self._flow_rates_content.winfo_children():
            widget.destroy()
        self._flow_labels.clear()
        for name in names:
            row = ctk.CTkFrame(self._flow_rates_content, fg_color="transparent")
            row.pack(fill="x", pady=2)
            ctk.CTkLabel(row, text=name, anchor="w", width=100).pack(side="left")
            val = ctk.CTkLabel(row, text="-- mL/min", anchor="e", text_color=("gray40", "gray60"))
            val.pack(side="right")
            self._flow_labels.append(val)

    # ── Leak Banner ────────────────────────────────────────────────────────────

    def _build_leak_banner(self):
        self._leak_banner = ctk.CTkFrame(self, fg_color="#7a1f1f", corner_radius=6, height=36)
        self._leak_banner.grid_propagate(False)
        self._leak_label = ctk.CTkLabel(
            self._leak_banner,
            text="⚠  Leak Detected",
            text_color="white",
            font=ctk.CTkFont(size=13, weight="bold"),
        )
        self._leak_label.place(relx=0.5, rely=0.5, anchor="center")
        # Hidden by default — call show_leak_alert() to display
        self._leak_visible = False

    def show_leak_alert(self, channel: int | None = None):
        text = f"⚠  Leak Detected — Channel {channel}" if channel is not None else "⚠  Leak Detected"
        self._leak_label.configure(text=text)
        if not self._leak_visible:
            self._leak_banner.grid(row=2, column=0, columnspan=2, sticky="ew", pady=(0, 4))
            self._leak_visible = True

    def hide_leak_alert(self):
        if self._leak_visible:
            self._leak_banner.grid_remove()
            self._leak_visible = False

    # ── Demo data (remove when hardware is connected) ──────────────────────────

    def _tick_demo(self):
        self._demo_t += 0.2
        self._demo_tick_count += 1
        flow_values = []
        for i, plot in enumerate(self._plots):
            phase = i * math.pi / 2.5
            val = 37.5 + 37.5 * math.sin(2 * math.pi * self._demo_t / 10 + phase)
            val += random.gauss(0, 1.2)
            val = max(0.0, min(75.0, val))
            plot.push(val)
            flow_values.append(val)
        if self._demo_tick_count % 5 == 0:
            self.update_flow_rates(flow_values)
        self.after(200, self._tick_demo)

    # ── Public update API (called by App on poll drain) ────────────────────────

    def update_flow_rates(self, values: list[float]):
        for label, val in zip(self._flow_labels, values):
            label.configure(text=f"{val:.2f} mL/min")

    def update_valve_states(self, bitmask: int):
        for valve_index, sw in self._valve_switches:
            if (bitmask >> valve_index) & 1:
                sw.select()
            else:
                sw.deselect()
