import sys
import os

# Allow imports from gui/, software/, and software/pyharp/ (the pyharp package root)
_gui_dir = os.path.dirname(os.path.abspath(__file__))
_software_dir = os.path.dirname(_gui_dir)
sys.path.insert(0, _gui_dir)
sys.path.insert(0, _software_dir)
sys.path.insert(0, os.path.join(_software_dir, "pyharp"))

import customtkinter as ctk
from app import App

if __name__ == "__main__":
    ctk.set_appearance_mode("dark")
    ctk.set_default_color_theme("blue")
    app = App()
    app.mainloop()
