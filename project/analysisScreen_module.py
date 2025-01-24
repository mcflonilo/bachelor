import tkinter as tk

def switch_frame(frame):
    frame.tkraise()

class AnalysisScreen:
    def __init__(self, root, prev_frame, data):
        self.root = root

        # Create a canvas and a scrollbar
        canvas = tk.Canvas(self.root)
        scrollbar = tk.Scrollbar(self.root, orient="vertical", command=canvas.yview)
        scrollable_frame = tk.Frame(canvas)

        scrollable_frame.bind(
            "<Configure>",
            lambda e: canvas.configure(
                scrollregion=canvas.bbox("all")
            )
        )

        canvas.create_window((0, 0), window=scrollable_frame, anchor="nw")
        canvas.configure(yscrollcommand=scrollbar.set)

        # Pack the canvas and scrollbar
        canvas.pack(side="left", fill="both", expand=True)
        scrollbar.pack(side="right", fill="y")

        # Add title label
        lbl_title = tk.Label(scrollable_frame, text="DISPLAY DATA", font=("Arial", 14))
        lbl_title.pack(pady=10)

        # Display project information
        project_info = data.get("project_info", {})
        self.display_section(scrollable_frame, "Project Information", project_info)

        # Display riser information
        riser_info = data.get("riser_info", {})
        self.display_section(scrollable_frame, "Riser Information", riser_info)

        # Display riser capacities
        riser_capacities = data.get("riser_capacities", {})
        self.display_riser_capacities(scrollable_frame, riser_capacities)

        # Display BS dimensions
        bs_dimensions = data.get("bs_dimension", {})
        self.display_section(scrollable_frame, "BS Dimensions", bs_dimensions)

        # Display BS materials
        bs_materials = data.get("bs_material", {})
        self.display_bs_materials(scrollable_frame, bs_materials)

        # Add a return button
        btn_return = tk.Button(scrollable_frame, text="Return", width=10, height=2, bg="#333333", fg="white", command=lambda: switch_frame(prev_frame))
        btn_return.pack(pady=20)

    def display_section(self, parent, title, section_data):
        lbl_section_title = tk.Label(parent, text=title, font=("Arial", 12, "bold"), anchor="w")
        lbl_section_title.pack(fill="x", padx=20, pady=5)
        for key, value in section_data.items():
            lbl = tk.Label(parent, text=f"{key}: {value}", anchor="w")
            lbl.pack(fill="x", padx=40, pady=5)

    def display_riser_capacities(self, parent, capacities):
        lbl_section_title = tk.Label(parent, text="Riser Capacities", font=("Arial", 12, "bold"), anchor="w")
        lbl_section_title.pack(fill="x", padx=20, pady=5)
        for key, values in capacities.items():
            lbl_key = tk.Label(parent, text=f"{key.capitalize()} Operation:", font=("Arial", 10, "bold"), anchor="w")
            lbl_key.pack(fill="x", padx=40, pady=5)
            for value in values:
                lbl_value = tk.Label(parent, text=f"Curvature: {value[0]}, Tension: {value[1]}", anchor="w")
                lbl_value.pack(fill="x", padx=60, pady=2)

    def display_bs_materials(self, parent, materials):
        lbl_section_title = tk.Label(parent, text="BS Materials", font=("Arial", 12, "bold"), anchor="w")
        lbl_section_title.pack(fill="x", padx=20, pady=5)
        for section, entries in materials.items():
            lbl_section = tk.Label(parent, text=f"{section}:", font=("Arial", 10, "bold"), anchor="w")
            lbl_section.pack(fill="x", padx=40, pady=5)
            for key, value in entries.items():
                lbl = tk.Label(parent, text=f"{key}: {value}", anchor="w")
                lbl.pack(fill="x", padx=60, pady=2)

def main():
    root = tk.Tk()
    prev_frame = tk.Frame(root)
    prev_frame.grid(row=0, column=0, sticky="nsew")
    data = {
        'project_info': {'project_name': 'seafs', 'client': 'efs', 'designer_name': 'efsef'},
        'riser_info': {'Riser Identification': 'sef', 'Outer Diameter': 'sef', 'Outer Diameter Tolerance': 'fsefsef', 'Mass Per Unit Length': 'sef', 'Axial Stiffness': 'sefse', 'Bending Stiffness': 'fsefs', 'Torsial Stiffness': 'sefef', 'Riser Length': 'efefs'},
        'riser_capacities': {'normal': [(0.08197, 0.0), (0.07752, 87.0), (0.07353, 174.0), (0.06944, 261.0), (0.06536, 348.0), (0.05714, 522.0), (0.04902, 696.0), (0.04082, 870.0), (0.03268, 1044.0), (0.02451, 1218.0)], 'abnormal': [(0.11765, 0.0), (0.11236, 133.0), (0.10753, 266.0), (0.10204, 399.0), (0.09709, 532.0), (0.08696, 798.0), (0.07519, 1064.0), (0.0625, 1330.0), (0.05, 1596.0), (0.03125, 1676.0)], 'interpolation': []},
        'bs_dimension': {'Input root length:': 'saefsef', 'Input tip length:': 'sef', 'Input MIN root OD:': 'sefs', 'Input MAX root OD:': 'efe', 'Input MIN overall length:': 'fefsefe', 'Input MAX overall length:': 'fsefe', 'Input clearance:': 'fsd', 'Input thidm(??):': 'fefsdf', 'BS ID:': 'e'},
        'bs_material': {'Section 1': {'material_characteristics': 'Steel', 'elastic_modules': '23'}, 'Section 2': {'material_characteristics': 'Titanium', 'elastic_modules': '3'}}
    }
    app = AnalysisScreen(root, prev_frame, data)
    root.mainloop()

if __name__ == "__main__":
    main()