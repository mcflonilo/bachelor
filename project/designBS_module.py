import tkinter as tk
import json
import os
from projectInfo_module import ProjectInfoWindow
from riserInfo_module import RiserInfoWindow
from riserCapacities_module import riserCapacities
from bsMaterialScreen_module import BSMaterialWindow
from bsDimensionScreen_module import BSDimensionWindow
from analysisScreen_module import AnalysisScreen

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
        btn_project_info = tk.Button(self.frame, text="INPUT PROJECT INFORMATION", width=30, height=2,
                                     bg="#333333", fg="white", command=lambda: switch_frame(projectInfo_frame))
        btn_project_info.pack(pady=10)

        # Create riserInfo frame
        riserInfo_frame = tk.Frame(root)
        riserInfo_frame.grid(row=0, column=0, sticky="nsew")
        self.riserInfo_app = RiserInfoWindow(riserInfo_frame, frame)
        btn_riser_info = tk.Button(self.frame, text="INPUT RISER INFORMATION", width=30, height=2,
                                   bg="#333333", fg="white", command=lambda: switch_frame(riserInfo_frame))
        btn_riser_info.pack(pady=10)

        # Create riserCapacities frame
        riserCapacities_frame = tk.Frame(root)
        riserCapacities_frame.grid(row=0, column=0, sticky="nsew")
        self.riserCapacities_app = riserCapacities(riserCapacities_frame, frame)
        btn_riser_capacities = tk.Button(self.frame, text="INPUT RISER CAPACITIES", width=30, height=2,
                                         bg="#333333", fg="white", command=lambda: switch_frame(riserCapacities_frame))
        btn_riser_capacities.pack(pady=10)

        # Create riserResponse frame
        riserResponse_frame = tk.Frame(root)
        riserResponse_frame.grid(row=0, column=0, sticky="nsew")
        btn_riser_response = tk.Button(self.frame, text="INPUT RISER RESPONSE", width=30, height=2,
                                       bg="#333333", fg="white", command=lambda: switch_frame(riserResponse_frame))
        btn_riser_response.pack(pady=10)

        # Create bsDimension frame
        bsDimension_frame = tk.Frame(root)
        bsDimension_frame.grid(row=0, column=0, sticky="nsew")
        self.bsDimension_app = BSDimensionWindow(bsDimension_frame, frame)

        # Create bsMaterial frame
        bsMaterial_frame = tk.Frame(root)
        bsMaterial_frame.grid(row=0, column=0, sticky="nsew")
        self.bsMaterial_app = BSMaterialWindow(bsMaterial_frame, switch_frame, bsDimension_frame)
        btn_bs_material = tk.Button(self.frame, text="INPUT BS MATERIAL", width=30, height=2,
                                    bg="#333333", fg="white", command=lambda: switch_frame(bsMaterial_frame))
        btn_bs_material.pack(pady=10)

        # Add a button to create the analysis screen
        btn_create_analysis = tk.Button(self.frame, text="CREATE ANALYSIS SCREEN", width=30, height=2,
                                        bg="#333333", fg="white", command=self.create_analysis_screen)
        btn_create_analysis.pack(pady=10)

        # Add a button to save data to a file
        btn_save_data = tk.Button(self.frame, text="SAVE DATA", width=30, height=2,
                                  bg="#333333", fg="white", command=self.save_data)
        btn_save_data.pack(pady=10)

    def get_data(self):
        data = {
            "project_info": self.projectInfo_app.get_data(),
            "riser_info": self.riserInfo_app.get_data(),
            "riser_capacities": self.riserCapacities_app.get_data(),
            "bs_dimension": self.bsDimension_app.get_data(),
            "bs_material": self.bsMaterial_app.get_data()
        }
        return data

    def save_data(self):
        data = self.get_data()
        project_name = data["project_info"].get("project_name", "default_project")
        filename = f"{project_name}_saved_data.json"
        with open(filename, "w") as file:
            json.dump(data, file, indent=4)
        print(f"Data saved to {filename}")

    def create_analysis_screen(self):
        data = self.get_data()
        analysis_frame = tk.Frame(self.root)
        analysis_frame.grid(row=0, column=0, sticky="nsew")
        analysis_app = AnalysisScreen(analysis_frame, self.frame, data)
        analysis_frame.grid_remove()
        switch_frame(analysis_frame)

def main():
    root = tk.Tk()
    root.title("Design BS Module")

    main_frame = tk.Frame(root)
    main_frame.grid(row=0, column=0, sticky="nsew")

    designBS_frame = tk.Frame(root)
    designBS_frame.grid(row=0, column=0, sticky="nsew")

    designBS_app = DesignWindow(designBS_frame, main_frame, root)

    # Add a button to switch to the designBS_frame
    btn_open_designBS = tk.Button(main_frame, text="Open Design BS GUI", command=lambda: switch_frame(designBS_frame))
    btn_open_designBS.pack(pady=10)

    # Raise the main_frame initially
    main_frame.tkraise()

    root.mainloop()

if __name__ == "__main__":
    main()