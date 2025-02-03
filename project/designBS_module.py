import tkinter as tk
from tkinter import filedialog
import json
import os
from projectInfo_module import ProjectInfoWindow
from riserInfo_module import RiserInfoWindow
from riserCapacities_module import riserCapacities
from bsMaterialScreen_module import BSMaterialWindow
from bsDimensionScreen_module import BSDimensionWindow
from analysisScreen_module import AnalysisScreen
from riserResponse_module import RiserResponseWindow

def switch_frame(frame):
    frame.tkraise()

class DesignWindow:
    def __init__(self, frame, main_frame, root):
        self.frame = frame
        self.main_frame = main_frame
        self.root = root

        # Create projectInfo frame
        projectInfo_frame = tk.Frame(root)
        projectInfo_frame.grid(row=0, column=0, sticky="nsew")
        self.projectInfo_app = ProjectInfoWindow(projectInfo_frame, frame)
        self.btn_project_info = tk.Button(self.frame, text="INPUT PROJECT INFORMATION", width=30, height=2,
                                     bg="#333333", fg="white", command=lambda: switch_frame(projectInfo_frame))
        self.btn_project_info.grid(row=1, column=0, pady=10)

        # Create riserInfo frame
        riserInfo_frame = tk.Frame(root)
        riserInfo_frame.grid(row=0, column=0, sticky="nsew")
        self.riserInfo_app = RiserInfoWindow(riserInfo_frame, frame)
        self.btn_riser_info = tk.Button(self.frame, text="INPUT RISER INFORMATION", width=30, height=2,
                                   bg="#333333", fg="white", command=lambda: switch_frame(riserInfo_frame))
        self.btn_riser_info.grid(row=2, column=0, pady=10)

        # Create riserCapacities frame
        riserCapacities_frame = tk.Frame(root)
        riserCapacities_frame.grid(row=0, column=0, sticky="nsew")
        self.riserCapacities_app = riserCapacities(riserCapacities_frame, frame)
        self.btn_riser_capacities = tk.Button(self.frame, text="INPUT RISER CAPACITIES", width=30, height=2,
                                         bg="#333333", fg="white", command=lambda: switch_frame(riserCapacities_frame))
        self.btn_riser_capacities.grid(row=3, column=0, pady=10)

        # Create riserResponse frame
        riserResponse_frame = tk.Frame(root)
        riserResponse_frame.grid(row=0, column=0, sticky="nsew")
        self.riserResponse_app = RiserResponseWindow(riserResponse_frame, frame)
        self.btn_riser_response = tk.Button(self.frame, text="INPUT RISER RESPONSE", width=30, height=2,
                                       bg="#333333", fg="white", command=lambda: switch_frame(riserResponse_frame))
        self.btn_riser_response.grid(row=4, column=0, pady=10)

        # Create bsDimension frame
        bsDimension_frame = tk.Frame(root)
        bsDimension_frame.grid(row=0, column=0, sticky="nsew")
        self.bsDimension_app = BSDimensionWindow(bsDimension_frame, frame)

        # Create bsMaterial frame
        bsMaterial_frame = tk.Frame(root)
        bsMaterial_frame.grid(row=0, column=0, sticky="nsew")
        self.bsMaterial_app = BSMaterialWindow(bsMaterial_frame, switch_frame, bsDimension_frame)
        self.btn_bs_material = tk.Button(self.frame, text="INPUT BS MATERIAL", width=30, height=2,
                                    bg="#333333", fg="white", command=lambda: switch_frame(bsMaterial_frame))
        self.btn_bs_material.grid(row=6, column=0, pady=10)

        # Add a button to create the analysis screen
        btn_create_analysis = tk.Button(self.frame, text="CREATE ANALYSIS SCREEN", width=30, height=2,
                                        bg="#333333", fg="white", command=self.create_analysis_screen)
        btn_create_analysis.grid(row=9, column=0, pady=10)

        # Add a button to save data to a file
        btn_save_data = tk.Button(self.frame, text="SAVE DATA", width=30, height=2,
                                  bg="#333333", fg="white", command=self.save_data)
        btn_save_data.grid(row=2, column=3, pady=10)

        # Add a button to load data from a file
        btn_load_data = tk.Button(self.frame, text="LOAD DATA", width=30, height=2,
                                  bg="#333333", fg="white", command=self.load_data)
        btn_load_data.grid(row=1, column=3, pady=10)

        btn_print_data = tk.Button(self.frame, text="check DATA", width=30, height=2,
                                      bg="#333333", fg="white", command=self.check_data)
        btn_print_data.grid(row=3, column=3, pady=10)

    def get_data(self):
        data = {
            "project_info": self.projectInfo_app.get_data(),
            "riser_info": self.riserInfo_app.get_data(),
            "riser_capacities": self.riserCapacities_app.get_data(),
            "riser_response": self.riserResponse_app.get_data(),
            "bs_dimension": self.bsDimension_app.get_data(),
            "bs_material": self.bsMaterial_app.get_data()
        }
        print(data)
        return data
    
    def check_data(self):
        data = self.get_data()
        all_valid = True  # Track if all fields are valid

        # Define required fields for each category and corresponding buttons
        required_fields = {
            "project_info": ["project_name", "client", "designer_name"],
            "riser_info": [
                "Riser Identification", "Outer Diameter", "Outer Diameter Tolerance",
                "Mass Per Unit Length", "Axial Stiffness", "Bending Stiffness",
                "Torsial Stiffness", "Riser Length"
            ],
            "bs_dimension": [
                "Input root length:", "Input tip length:", "Input MIN root OD:",
                "Input MAX root OD:", "Input MIN overall length:", "Input MAX overall length:",
                "Input clearance:", "Input thidm(??):", "BS ID:",
                "Increment Width:", "Increment Length:"
            ],
            "bs_material": ["Section 1"],  # At least one section must exist
        }

        # Define button references (excluding bs_dimension because it's linked to bs_material)
        buttons = {
            "project_info": self.btn_project_info,
            "riser_info": self.btn_riser_info,
            "bs_material": self.btn_bs_material,  # This button depends on bs_dimension
            "riser_capacities": self.btn_riser_capacities,
            "riser_response": self.btn_riser_response
        }

        # Validate required fields
        for category, fields in required_fields.items():
            if category == "bs_dimension":
                continue  # Skip bs_dimension for now, handle separately

            missing_fields = [
                field for field in fields
                if field not in data[category] or data[category][field] in [None, "", []]
            ]

            # Special case: bs_material must have at least one valid section
            if category == "bs_material" and not any(data["bs_material"].values()):
                missing_fields.append("At least one material section")

            if missing_fields:
                print(f"Missing fields in {category}: {', '.join(missing_fields)}")
                buttons[category].config(bg="red")
                all_valid = False
            else:
                buttons[category].config(bg="green")

        # Special case: BS Material button turns red if any BS Dimension fields are missing
        missing_bs_dimension_fields = [
            field for field in required_fields["bs_dimension"]
            if field not in data["bs_dimension"] or data["bs_dimension"][field] in [None, "", []]
        ]
        if missing_bs_dimension_fields:
            print(f"Missing fields in bs_dimension: {', '.join(missing_bs_dimension_fields)}")
            self.btn_bs_material.config(bg="red")  # BS Material button turns red
            all_valid = False
        else:
            buttons["bs_material"].config(bg="green")

        # ✅ Check riser_capacities for non-empty (normal, abnormal) tuples
        def has_valid_data(entry):
            """Returns True if both lists inside the tuple contain data."""
            return isinstance(entry, tuple) and len(entry) == 2 and all(len(lst) > 0 for lst in entry)

        if not has_valid_data(data["riser_capacities"].get("normal")) or not has_valid_data(data["riser_capacities"].get("abnormal")):
            print("Missing or empty riser capacities data.")
            self.btn_riser_capacities.config(bg="red")
            all_valid = False
        else:
            self.btn_riser_capacities.config(bg="green")

        # ✅ Check riser_response for non-empty (normal, abnormal) tuples
        if not has_valid_data(data["riser_response"].get("normal")) or not has_valid_data(data["riser_response"].get("abnormal")):
            print("Missing or empty riser response data.")
            self.btn_riser_response.config(bg="red")
            all_valid = False
        else:
            self.btn_riser_response.config(bg="green")

        # Print data if everything is valid
        if all_valid:
            print("All data fields are valid:")
            for category, content in data.items():
                print(f"{category}: {content}")
        else:
            print("Some data fields are missing or invalid.")




    def save_data(self):
        data = self.get_data()
        project_name = data["project_info"].get("project_name", "default_project")
        default_filename = f"{project_name}_saved_data.json"
        filename = filedialog.asksaveasfilename(
            initialfile=default_filename,
            defaultextension=".json",
            filetypes=[("JSON files", "*.json"), ("All files", "*.*")],
            title="Save file"
        )
        if filename:
            with open(filename, "w") as file:
                json.dump(data, file, indent=4)
            print(f"Data saved to {filename}")

    def load_data(self):
        filename = filedialog.askopenfilename(
            title="Select file",
            filetypes=(("JSON files", "*.json"), ("All files", "*.*"))
        )
        if filename:
            with open(filename, "r") as file:
                data = json.load(file)
            self.projectInfo_app.set_data(data["project_info"])
            self.riserInfo_app.set_data(data["riser_info"])
            self.riserCapacities_app.set_data(data["riser_capacities"])
            self.riserResponse_app.set_data(data["riser_response"])
            self.bsDimension_app.set_data(data["bs_dimension"])
            self.bsMaterial_app.set_data(data["bs_material"])
            print(f"Data loaded from {filename}")

    def create_analysis_screen(self):
        data = self.get_data()
        analysis_frame = tk.Frame(self.root)
        analysis_frame.grid(row=0, column=0, sticky="nsew")
        analysis_app = AnalysisScreen(analysis_frame, self.frame, data)
        switch_frame(analysis_frame)

def main():
    root = tk.Tk()
    root.title("Design BS Module")
    root.protocol("WM_DELETE_WINDOW", root.quit)


    main_frame = tk.Frame(root)
    main_frame.grid(row=0, column=0, sticky="nsew")

    designBS_frame = tk.Frame(root)
    designBS_frame.grid(row=0, column=0, sticky="nsew")

    designBS_app = DesignWindow(designBS_frame, main_frame, root)

    # Add a button to switch to the designBS_frame
    btn_open_designBS = tk.Button(main_frame, text="Open Design BS GUI", command=lambda: switch_frame(designBS_frame))
    btn_open_designBS.grid(row=0, column=0, pady=10)

    # Raise the main_frame initially
    main_frame.tkraise()

    root.mainloop()
    print("Application closed")
    root.destroy()

if __name__ == "__main__":
    main()