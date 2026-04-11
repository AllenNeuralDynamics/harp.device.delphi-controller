import collections
import time

import customtkinter as ctk
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg

from widgets.tile import Tile


class _Canvas(FigureCanvasTkAgg):
    """FigureCanvasTkAgg with HiDPI auto-detection disabled.

    On macOS Retina + Homebrew Tk, matplotlib sets device_pixel_ratio=2 but
    Tk's PhotoImage does not downscale the resulting bitmap, so the plot
    renders at 2× logical size.  Overriding _update_device_pixel_ratio as a
    no-op keeps the ratio at 1 so the bitmap matches the logical canvas size.
    """
    def _update_device_pixel_ratio(self, event=None):
        pass


class LivePlot(Tile):
    """A Tile that embeds a scrolling matplotlib time-series chart."""

    WINDOW_SECONDS = 10
    MAX_POINTS = 100  # 10s at ~5 Hz with headroom

    def __init__(self, master, title: str = "Flow", units: str = "mL/min", **kwargs):
        super().__init__(master, title=title, **kwargs)
        self.units = units

        self._times: collections.deque = collections.deque(maxlen=self.MAX_POINTS)
        self._values: collections.deque = collections.deque(maxlen=self.MAX_POINTS)
        self._t0 = time.time()

        _bg = "#2b2b2b"
        _ax_bg = "#242424"

        self._fig, self._ax = plt.subplots(figsize=(4, 2.2), dpi=50)
        self._fig.patch.set_facecolor(_bg)
        self._ax.set_facecolor(_ax_bg)
        self._ax.tick_params(colors="#aaaaaa", labelsize=8)
        for spine in self._ax.spines.values():
            spine.set_edgecolor("#555555")
        self._ax.set_xlabel("s", color="#aaaaaa", fontsize=8)
        self._ax.set_ylabel(units, color="#aaaaaa", fontsize=8)
        self._fig.tight_layout(pad=1.2)

        self._mpl_canvas = _Canvas(self._fig, master=self.content)
        self._mpl_canvas.get_tk_widget().configure(bg=_bg, highlightthickness=0)
        self._mpl_canvas.get_tk_widget().pack(fill="both", expand=True)

    def push(self, value: float):
        """Add a new data point and redraw."""
        t = time.time() - self._t0
        self._times.append(t)
        self._values.append(value)
        self._redraw()

    def reset(self):
        """Clear accumulated data and redraw blank."""
        self._times.clear()
        self._values.clear()
        self._t0 = time.time()
        self._ax.clear()
        self._ax.set_facecolor("#242424")
        self._ax.tick_params(colors="#aaaaaa", labelsize=8)
        for spine in self._ax.spines.values():
            spine.set_edgecolor("#555555")
        self._ax.set_ylabel(self.units, color="#aaaaaa", fontsize=8)
        self._mpl_canvas.draw_idle()

    def _redraw(self):
        now = time.time() - self._t0
        cutoff = now - self.WINDOW_SECONDS

        self._ax.clear()
        self._ax.set_facecolor("#242424")
        self._ax.tick_params(colors="#aaaaaa", labelsize=8)
        for spine in self._ax.spines.values():
            spine.set_edgecolor("#555555")
        self._ax.set_ylabel(self.units, color="#aaaaaa", fontsize=8)
        self._ax.set_xlim(cutoff, now)

        times = list(self._times)
        values = list(self._values)
        if times:
            visible = [(t, v) for t, v in zip(times, values) if t >= cutoff]
            if visible:
                ts, vs = zip(*visible)
                self._ax.plot(ts, vs, color="#4da6ff", linewidth=1.2)

        self._mpl_canvas.draw_idle()
