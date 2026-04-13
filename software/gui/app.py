import customtkinter as ctk
from device_manager import DeviceManager
from tabs.dashboard import DashboardTab
from tabs.valves import ValvesTab
from tabs.flow_adc import FlowAdcTab
from tabs.config_tab import ConfigTab


class App(ctk.CTk):
    def __init__(self):
        super().__init__()
        self.title("Delphi Controller")
        self.geometry("1280x800")
        self.minsize(1000, 640)

        self.dm = DeviceManager()

        self._build_header()
        self._build_tabs()
        self._drain_queue()

    def _build_header(self):
        header = ctk.CTkFrame(self, height=44, corner_radius=0, fg_color=("gray85", "gray17"))
        header.pack(fill="x")
        header.pack_propagate(False)

        ctk.CTkLabel(
            header,
            text="Delphi Controller",
            font=ctk.CTkFont(size=15, weight="bold"),
        ).pack(side="left", padx=16, pady=10)

        # Connection status chip (right side) — updated by set_connection_status()
        self._status_dot = ctk.CTkLabel(
            header, text="●", text_color="#e05555", font=ctk.CTkFont(size=16)
        )
        self._status_dot.pack(side="right", padx=(0, 16))

        self._status_label = ctk.CTkLabel(header, text="Disconnected", text_color=("gray40", "gray60"))
        self._status_label.pack(side="right", padx=(0, 4))

    def _build_tabs(self):
        self.tabview = ctk.CTkTabview(self)
        self.tabview.pack(fill="both", expand=True, padx=10, pady=(4, 10))

        for name in ("Dashboard", "Valves", "Flow / ADC", "Config"):
            self.tabview.add(name)
            tab = self.tabview.tab(name)
            tab.grid_columnconfigure(0, weight=1)
            tab.grid_rowconfigure(0, weight=1)

        self.dashboard_tab = DashboardTab(self.tabview.tab("Dashboard"), device_manager=self.dm)
        self.dashboard_tab.grid(row=0, column=0, sticky="nsew")

        self.valves_tab = ValvesTab(self.tabview.tab("Valves"), device_manager=self.dm)
        self.valves_tab.grid(row=0, column=0, sticky="nsew")

        self.flow_adc_tab = FlowAdcTab(self.tabview.tab("Flow / ADC"))
        self.flow_adc_tab.grid(row=0, column=0, sticky="nsew")

        self.config_tab = ConfigTab(
            self.tabview.tab("Config"),
            device_manager=self.dm,
            on_connect=lambda: self.set_connection_status(True),
            on_disconnect=lambda: self.set_connection_status(False),
        )
        self.config_tab.grid(row=0, column=0, sticky="nsew")

        # Wire Flow/ADC tab → Dashboard plots
        self.flow_adc_tab.on_change = lambda configs: self.dashboard_tab.sync_plots(configs)
        self.dashboard_tab.sync_plots(self.flow_adc_tab.get_channel_configs())

        # Wire Valves tab → Dashboard valve controls
        self.valves_tab.on_change = lambda configs: self.dashboard_tab.sync_valves(configs)
        self.dashboard_tab.sync_valves(self.valves_tab.get_valve_configs())

    def _drain_queue(self):
        """Pull all pending poll results off the DeviceManager queue and fan
        out updates to tabs.  Reschedules itself every 200 ms on the GUI thread."""
        try:
            while True:
                data = self.dm.queue.get_nowait()
                self.dashboard_tab.push_live_data(data["flow_rates"])
                self.dashboard_tab.update_valve_states(data["valves_state"])
                self.valves_tab.update_duty_cycles(data.get("duty_cycles", []))
        except Exception:
            pass
        self.after(200, self._drain_queue)

    def set_connection_status(self, connected: bool):
        if connected:
            self._status_dot.configure(text_color="#55cc77")
            self._status_label.configure(text="Connected")
        else:
            self._status_dot.configure(text_color="#e05555")
            self._status_label.configure(text="Disconnected")
