import tkinter as tk

def switch_frame(frame):
    frame.tkraise()

class BSDimensionWindow:
    def __init__(self, root, prev_frame):
        self.root = root
        self.prev_frame = prev_frame

        # Add title label
        lbl_title = tk.Label(root, text="BS Geometry Constraints:", font=("Arial", 12, "bold"), anchor="w")
        lbl_title.grid(row=0, column=0, columnspan=2, sticky="w", pady=10, padx=20)

        # Define input labels and fields
        labels = [
            "Input root length:", "Input tip length:", "Input MIN root OD:",
            "Input MAX root OD:", "Input MIN overall length:", "Input MAX overall length:",
            "Input clearance:", "Input thidm(??):", "BS ID:", "Increment Width:", "Increment Length:"
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
        btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white", command=lambda: [self.get_data(), switch_frame(prev_frame)])
        btn_ok.grid(row=0, column=0, padx=10)

    def get_data(self):
        """Return all data from the input fields."""
        data = {}
        for label, entry in self.entries.items():
            data[label] = entry.get()
        return data

    def set_data(self, data):
        """Set data to the input fields."""
        for label, value in data.items():
            if label in self.entries:
                self.entries[label].delete(0, tk.END)
                self.entries[label].insert(0, value)

def main():
    root = tk.Tk()
    prev_frame = tk.Frame(root)
    prev_frame.grid(row=0, column=0, sticky="nsew")
    app = BSDimensionWindow(root, prev_frame)
    root.mainloop()

if __name__ == "__main__":
    main()