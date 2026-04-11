"""Shared GUI utilities."""

import sys
import customtkinter as ctk


def bind_scroll_wheel(scrollable_frame: ctk.CTkScrollableFrame) -> None:
    """
    Fix mouse-wheel scrolling on CTkScrollableFrame.

    Recursive widget binding doesn't reach the actual underlying tk widgets
    inside CTk wrappers. Instead, register a global bind_all handler that
    checks pointer bounds on every scroll event — only scrolls when the
    cursor is within this frame's screen bounds.
    """
    canvas = scrollable_frame._parent_canvas

    if sys.platform == "darwin":
        def _scroll(event):
            if not scrollable_frame.winfo_ismapped():
                return
            x = scrollable_frame.winfo_rootx()
            y = scrollable_frame.winfo_rooty()
            w = scrollable_frame.winfo_width()
            h = scrollable_frame.winfo_height()
            px = scrollable_frame.winfo_pointerx()
            py = scrollable_frame.winfo_pointery()
            if x <= px <= x + w and y <= py <= y + h:
                canvas.yview_scroll(int(-1 * event.delta), "units")
    else:
        def _scroll(event):
            if not scrollable_frame.winfo_ismapped():
                return
            x = scrollable_frame.winfo_rootx()
            y = scrollable_frame.winfo_rooty()
            w = scrollable_frame.winfo_width()
            h = scrollable_frame.winfo_height()
            px = scrollable_frame.winfo_pointerx()
            py = scrollable_frame.winfo_pointery()
            if x <= px <= x + w and y <= py <= y + h:
                canvas.yview_scroll(int(-1 * (event.delta / 120)), "units")

    scrollable_frame.bind_all("<MouseWheel>", _scroll, add="+")
