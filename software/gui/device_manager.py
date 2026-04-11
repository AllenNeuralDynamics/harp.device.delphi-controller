"""
DeviceManager — stub for layout development.
Real pyharp integration wired in once the GUI structure is complete.
"""


class DeviceManager:
    def __init__(self):
        self.is_connected = False
        self._device = None

    def connect(self, port: str):
        # TODO: self._device = Device(port); start poll thread
        self.is_connected = True

    def disconnect(self):
        # TODO: stop poll thread; self._device.disconnect()
        self.is_connected = False

    def send(self, harp_message):
        # TODO: return self._device.send(harp_message.frame)
        raise RuntimeError("DeviceManager not yet connected to hardware")
