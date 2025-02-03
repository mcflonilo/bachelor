import tkinter as tk

def switch_frame(frame):
    frame.tkraise()

class RiserInfoWindow:
    def __init__(self, root, prev_frame):
        self.root = root
        self.prev_frame = prev_frame

        # Add title label
        lbl_title = tk.Label(root, text="RISER INFORMATION", font=("Arial", 14))
        lbl_title.grid(row=0, column=0, columnspan=2, pady=10)

        # Create labels and entry fields for riser attributes
        fields = [
            "Riser Identification", 
            "Outer Diameter", 
            "Outer Diameter Tolerance", 
            "Mass Per Unit Length", 
            "Axial Stiffness", 
            "Bending Stiffness", 
            "Torsial Stiffness", 
            "Riser Length"
        ]
        self.entries = {}

        for i, field in enumerate(fields):
            lbl = tk.Label(root, text=f"{field}:", anchor="w")
            lbl.grid(row=i + 1, column=0, sticky="w", padx=20, pady=5)
            entry = tk.Entry(root)
            entry.grid(row=i + 1, column=1, padx=20, pady=5)
            self.entries[field] = entry

        # Add OK and CANCEL buttons
        frame_buttons = tk.Frame(root)
        frame_buttons.grid(row=len(fields) + 1, column=0, columnspan=2, pady=20)
        btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white", command=lambda: [self.get_data(), switch_frame(prev_frame)])
        btn_ok.grid(row=0, column=0, padx=10)

    def get_data(self):
        """Return all data from the input fields."""
        data = {}
        for field, entry in self.entries.items():
            data[field] = entry.get()
        return data

    def set_data(self, data):
        """Set data to the input fields."""
        for field, value in data.items():
            if field in self.entries:
                self.entries[field].delete(0, tk.END)
                self.entries[field].insert(0, value)

def main():
    root = tk.Tk()
    prev_frame = tk.Frame(root)
    prev_frame.grid(row=0, column=0, sticky="nsew")
    app = RiserInfoWindow(root, prev_frame)
    root.mainloop()

if __name__ == "__main__":
    main()