import tkinter as tk

def switch_frame(frame):
    frame.tkraise()

class BSDimensionWindow:
    def __init__(self, root):
        self.root = root
        # Add title label
        lbl_title = tk.Label(root, text="BS Geometry Constraints:", font=("Arial", 12, "bold"), anchor="w")
        lbl_title.grid(row=0, column=0, columnspan=2, sticky="w", pady=10, padx=20)

        # Define input labels and fields
        labels = [
            "Input root length:", "Input tip length:", "Input MIN root OD:",
            "Input MAX root OD:", "Input MIN overall length:", "Input MAX overall length:",
            "Input clearance:", "Input thidm(??):", "BS ID:"
        ]
        self.entries = {}

        for i, label_text in enumerate(labels):
            lbl = tk.Label(root, text=label_text, anchor="w")
            lbl.grid(row=i + 1, column=0, sticky="w", padx=20, pady=5)
            entry = tk.Entry(root, width=20)
            entry.grid(row=i + 1, column=1, padx=10, pady=5)
            self.entries[label_text] = entry

        # Add OK and CANCEL buttons
        frame_buttons = tk.Frame(root)
        frame_buttons.grid(row=len(labels) + 1, column=0, columnspan=2, pady=20)
        btn_ok = tk.Button(frame_buttons, text="OK", command=self.get_data)
        btn_ok.pack(side=tk.LEFT, padx=10)
        btn_cancel = tk.Button(frame_buttons, text="CANCEL", command=root.quit)
        btn_cancel.pack(side=tk.LEFT, padx=10)

    def get_data(self):
        """Return all data from the input fields."""
        data = {}
        for label, entry in self.entries.items():
            data[label] = entry.get()
        return data

def main():
    root = tk.Tk()
    app = BSDimensionWindow(root)
    root.mainloop()

if __name__ == "__main__":
    main()