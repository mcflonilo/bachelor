import tkinter as tk


import tkinter as tk
from tkinter import messagebox

class MultipleBSDimensionWindow:
    def __init__(self, root, prev_frame, show_frame):
        self.root = root
        self.prev_frame = prev_frame
        self.show_frame = show_frame

        self.BS_data = []  # Stores multiple BS entries
        self.current_bs_index = 0  # Tracks current BS entry being filled

        # Define labels for each BS
        self.labels = ["root length:", "cone length:", "tip length:", "root OD:", "tip OD:", "ID:"]
        self.entries = {}

        # Title Label
        lbl_title = tk.Label(root, text="BS Geometry Constraints:", font=("Arial", 12, "bold"), anchor="w")
        lbl_title.grid(row=0, column=0, columnspan=2, sticky="w", pady=10, padx=20)

        # Input for number of BS
        lbl_no_bs = tk.Label(root, text="Number of BS:", anchor="w")
        lbl_no_bs.grid(row=1, column=0, sticky="w", padx=20, pady=5)

        self.entry_no_bs = tk.Entry(root, width=20)
        self.entry_no_bs.grid(row=1, column=1, padx=10, pady=5)

        # OK Button to generate fields
        btn_no_bs = tk.Button(root, text="OK", width=10, height=2, bg="#333333", fg="white",
                              command=self.start_entry_process)
        btn_no_bs.grid(row=1, column=2, padx=10)

        # Frame for dynamic entries
        self.entry_frame = tk.Frame(root)
        self.entry_frame.grid(row=2, column=0, columnspan=3, padx=20, pady=10)

        # Buttons Frame
        self.frame_buttons = tk.Frame(root)
        self.frame_buttons.grid(row=3, column=0, columnspan=3, pady=20)

        # Next Button (appears dynamically)
        self.btn_next = tk.Button(self.frame_buttons, text="Next", width=10, height=2, bg="#333333", fg="white",
                                  command=self.save_and_next)

        # Finish Button (appears after last BS entry)
        self.btn_finish = tk.Button(self.frame_buttons, text="Finish", width=10, height=2, bg="green", fg="white",
                                    command=self.finish_entry)

    def start_entry_process(self):
        """Starts the process of entering BS data based on user input."""
        try:
            self.total_bs = int(self.entry_no_bs.get())  # Get user input
            if self.total_bs <= 0:
                raise ValueError
            self.BS_data = []  # Reset data storage
            self.current_bs_index = 0  # Reset counter
            self.entry_no_bs.config(state="disabled")  # Lock input to prevent changes
            self.btn_next.grid(row=0, column=0, padx=10)  # Show "Next" button
            self.generate_entries()
        except ValueError:
            messagebox.showerror("Invalid Input", "Please enter a valid positive integer for the number of BS.")

    def generate_entries(self):
        """Generates entry fields for the current BS."""
        # Clear existing entries
        for widget in self.entry_frame.winfo_children():
            widget.destroy()

        # Display BS index
        lbl_bs = tk.Label(self.entry_frame, text=f"BS {self.current_bs_index + 1}:", font=("Arial", 10, "bold"))
        lbl_bs.grid(row=0, column=0, columnspan=2, pady=5)

        self.entries = {}  # Reset entries for new BS

        for i, label_text in enumerate(self.labels):
            lbl = tk.Label(self.entry_frame, text=label_text, anchor="w")
            lbl.grid(row=i + 1, column=0, sticky="w", padx=10, pady=5)

            entry = tk.Entry(self.entry_frame, width=20)
            entry.grid(row=i + 1, column=1, padx=10, pady=5)
            self.entries[label_text] = entry

    def validate_entries(self):
        """Checks if all fields are filled with valid numbers."""
        for label, entry in self.entries.items():
            value = entry.get().strip()
            if not value:
                messagebox.showerror("Missing Input", f"Please enter a value for '{label}'.")
                return False
            try:
                float(value)  # Ensure it's a valid number
            except ValueError:
                messagebox.showerror("Invalid Input", f"'{label}' must be a valid number.")
                return False
        return True

    def save_and_next(self):
        """Validates, saves current BS data, and moves to the next."""
        if not self.validate_entries():
            return  # Stop if validation fails

        bs_entry_data = {label: float(entry.get().strip()) for label, entry in self.entries.items()}
        self.BS_data.append(bs_entry_data)

        if self.current_bs_index + 1 < self.total_bs:
            self.current_bs_index += 1  # Move to next BS
            self.generate_entries()
        else:
            # Hide "Next" and show "Finish" button
            self.btn_next.grid_remove()
            self.btn_finish.grid(row=0, column=0, padx=10)

    def finish_entry(self):
        """Finalizes data entry and goes back to the previous screen."""
        print("Final BS Data:", self.BS_data)  # Debugging output
        self.show_frame(self.prev_frame)  # Switch to the previous screen

    def get_data(self):
        """Return all data from the input fields."""
        data = {}
        for label, entry in self.entries.items():
            data[label] = entry.get()
        return data


def switch_frame(frame):
    frame.tkraise()

def main():
    root = tk.Tk()
    prev_frame = tk.Frame(root)
    prev_frame.grid(row=0, column=0, sticky="nsew")
    app = MultipleBSDimensionWindow(root, prev_frame, switch_frame)
    root.mainloop()

if __name__ == "__main__":
    main()