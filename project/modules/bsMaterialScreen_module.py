import tkinter as tk
from tkinter import ttk

class BSMaterialWindow:
    def __init__(self, root, next_frame, show_frame, prev_frame):
        self.root = root
        self.next_frame = next_frame
        self.show_frame = show_frame

        # Add "No. of sections to create" input
        lbl_no_of_sections = tk.Label(self.root, text="No. of sections to create:")
        lbl_no_of_sections.grid(row=0, column=0, sticky="w", padx=20, pady=10)
        self.entry_no_of_sections = tk.Entry(self.root, width=10)
        self.entry_no_of_sections.grid(row=0, column=1, sticky="w", padx=10)

        # Add button to create sections
        create_sections_button = tk.Button(self.root, text="Create Sections", command=self.create_sections)
        create_sections_button.grid(row=0, column=2, padx=10, pady=10)

        #add return button
        return_button = tk.Button(self.root, text="Return", command=lambda: prev_frame.tkraise())
        return_button.grid(row=0, column=3, padx=10, pady=10)


        # Dictionary to store section entries
        self.section_entries = {}

    def create_sections(self):
        try:
            no_of_sections = int(self.entry_no_of_sections.get())
        except ValueError:
            tk.messagebox.showerror("Invalid input", "Please enter a valid number")
            return

        # Clear existing sections
        for widget in self.root.winfo_children():
            if isinstance(widget, tk.Label) or isinstance(widget, tk.Entry) or isinstance(widget, ttk.Combobox) or isinstance(widget, tk.Canvas) or isinstance(widget, tk.Button):
                widget.destroy()

        # Recreate the input field and button
        lbl_no_of_sections = tk.Label(self.root, text="No. of sections to create:")
        lbl_no_of_sections.grid(row=0, column=0, sticky="w", padx=20, pady=10)
        self.entry_no_of_sections = tk.Entry(self.root, width=10)
        self.entry_no_of_sections.grid(row=0, column=1, sticky="w", padx=10)
        create_sections_button = tk.Button(self.root, text="Create Sections", command=self.create_sections)
        create_sections_button.grid(row=0, column=2, padx=10, pady=10)

        # Create the specified number of sections
        self.section_entries = {}
        for i in range(no_of_sections):
            self.create_bs_section(self.root, f"Section {i + 1}", i * 5 + 1)

        # Add OK button to switch frames
        ok_button = tk.Button(self.root, text="OK", command=lambda: self.show_frame(self.next_frame))
        ok_button.grid(row=no_of_sections * 5 + 2, column=0, columnspan=3, pady=20)

    def create_bs_section(self, parent, section_title, row):
        # Add section title
        lbl_section_title = tk.Label(parent, text=section_title, font=("Arial", 12, "bold"), anchor="w")
        lbl_section_title.grid(row=row, column=0, columnspan=2, sticky="w", pady=10, padx=20)

        # Add "Material Characteristics" dropdown
        lbl_material_characteristics = tk.Label(parent, text="Material Characteristics:")
        lbl_material_characteristics.grid(row=row + 2, column=0, sticky="w", padx=20, pady=5)
        
        combo_materials = ttk.Combobox(parent, values=["Silicon", "Titanium", "Copper", "Steel", "Plastic", "+ add new"], width=20)
        combo_materials.grid(row=row + 3, column=0, sticky="w", padx=20)

        # Add "Elastic modules" input field
        lbl_elastic_modules = tk.Label(parent, text="Elastic modules:")
        lbl_elastic_modules.grid(row=row + 3, column=1, sticky="w", padx=10)
        entry_elastic_modules = tk.Entry(parent, width=15)
        entry_elastic_modules.grid(row=row + 3, column=2, sticky="w", padx=10)

        # Store entries in the dictionary
        self.section_entries[section_title] = {
            "material_characteristics": combo_materials,
            "elastic_modules": entry_elastic_modules
        }

        # Add separator line
        tk.Canvas(parent, height=2, width=580, bg="black").grid(row=row + 4, column=0, columnspan=3, pady=10)

    def get_data(self):
        """Return all data from the input fields."""
        data = {}
        for section, entries in self.section_entries.items():
            data[section] = {
                "material_characteristics": entries["material_characteristics"].get(),
                "elastic_modules": entries["elastic_modules"].get()
            }
        return data
    
    def set_data(self, data):
        """Set data to the input fields."""
        self.entry_no_of_sections.delete(0, tk.END)
        self.entry_no_of_sections.insert(0, len(data))

        # Clear existing sections
        for widget in self.root.winfo_children():
            if isinstance(widget, tk.Label) or isinstance(widget, tk.Entry) or isinstance(widget, ttk.Combobox) or isinstance(widget, tk.Canvas) or isinstance(widget, tk.Button):
                widget.destroy()

        # Recreate the input field and button
        lbl_no_of_sections = tk.Label(self.root, text="No. of sections to create:")
        lbl_no_of_sections.grid(row=0, column=0, sticky="w", padx=20, pady=10)
        self.entry_no_of_sections = tk.Entry(self.root, width=10)
        self.entry_no_of_sections.grid(row=0, column=1, sticky="w", padx=10)
        create_sections_button = tk.Button(self.root, text="Create Sections", command=self.create_sections)
        create_sections_button.grid(row=0, column=2, padx=10, pady=10)

        # Create the sections based on the data
        self.section_entries = {}
        for i, (section, values) in enumerate(data.items()):
            self.create_bs_section(self.root, section, i * 5 + 1)
            self.section_entries[section]["material_characteristics"].set(values["material_characteristics"])
            self.section_entries[section]["elastic_modules"].delete(0, tk.END)
            self.section_entries[section]["elastic_modules"].insert(0, values["elastic_modules"])

        # Add OK button to switch frames
        ok_button = tk.Button(self.root, text="OK", command=lambda: self.show_frame(self.next_frame))
        ok_button.grid(row=len(data) * 5 + 2, column=0, columnspan=3, pady=20)


def main():
    switch_frame = lambda frame: frame.tkraise()
    root = tk.Tk()
    next_frame = tk.Frame(root)
    next_frame.grid(row=0, column=0, sticky="nsew")
    app = BSMaterialWindow(root, switch_frame, next_frame)
    root.mainloop()

if __name__ == "__main__":
    main()