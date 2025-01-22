import tkinter as tk

def switch_frame(frame):
    frame.tkraise()



class RiserInfoWindow:
    def __init__(self, root, prev_frame):
        self.root = root

        # Add title label
        lbl_title = tk.Label(root, text="RISER INFORMATION", font=("Arial", 14))
        lbl_title.pack(pady=10)

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

        for field in fields:
            lbl = tk.Label(root, text=f"{field}:", anchor="w")
            lbl.pack(fill="x", padx=20)
            entry = tk.Entry(root)
            entry.pack(fill="x", padx=20, pady=5)
            self.entries[field] = entry

        # Add OK and CANCEL buttons
        OK_buttons = tk.Button(root, text="OK", width=10, height=2, bg="#333333", fg="white", command=lambda: switch_frame(prev_frame))
        OK_buttons.pack(pady=20)

    def get_data(self):
        """Return all data from the input fields."""
        data = {}
        for field, entry in self.entries.items():
            data[field] = entry.get()
        return data
    def print_data(self):
        print(self.get_data())

def main():
    root = tk.Tk()
    app = RiserInfoWindow(root)
    root.mainloop()

if __name__ == "__main__":
    main()