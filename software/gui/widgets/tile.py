import customtkinter as ctk


class Tile(ctk.CTkFrame):
    """Reusable card widget with a bold title and a content area below."""

    def __init__(self, master, title: str = "", **kwargs):
        kwargs.setdefault("corner_radius", 8)
        super().__init__(master, **kwargs)

        self._title_label: ctk.CTkLabel | None = None
        if title:
            self._title_label = ctk.CTkLabel(
                self,
                text=title,
                font=ctk.CTkFont(size=13, weight="bold"),
                anchor="w",
            )
            self._title_label.pack(fill="x", padx=12, pady=(10, 0))

            sep = ctk.CTkFrame(self, height=1, corner_radius=0, fg_color=("gray70", "gray35"))
            sep.pack(fill="x", padx=12, pady=(6, 0))

        # Public content frame — callers pack/grid children into this
        self.content = ctk.CTkFrame(self, fg_color="transparent")
        self.content.pack(fill="both", expand=True, padx=12, pady=(8, 12))

    def set_title(self, title: str):
        if self._title_label is not None:
            self._title_label.configure(text=title)
