import tkinter as tk
from projectInfo_module import ProjectInfoWindow
from riserInfo_module import RiserInfoWindow
from riserCapacities_module import riserCapacities
from bsMaterialScreen_module import BSMaterialWindow
from bsDimensionScreen_module import BSDimensionWindow

def switch_frame(frame):
    frame.tkraise()

class DesignWindow:
    def __init__(self, frame, main_frame, root):
        self.frame = frame


        # Create projectInfo frame
        projectInfo_frame = tk.Frame(root)
        projectInfo_frame.grid(row=0, column=0, sticky="nsew")
        projectInfo_app = ProjectInfoWindow(projectInfo_frame)
        projectInfo_frame.lower()  # Hide the frame initially
        btn_project_info = tk.Button(self.frame, text="INPUT PROJECT INFORMATION", width=30, height=2,
                                     bg="#333333", fg="white", command=lambda: switch_frame(projectInfo_frame))
        btn_project_info.grid(row=1, column=0, pady=10)

        # Create riserInfo frame
        riserInfo_frame = tk.Frame(root)
        riserInfo_frame.grid(row=0, column=0, sticky="nsew")
        riserInfo_app = RiserInfoWindow(riserInfo_frame, frame)
        riserInfo_frame.lower()  # Hide the frame initially
        btn_riser_info = tk.Button(self.frame, text="INPUT RISER INFORMATION", width=30, height=2,
                                   bg="#333333", fg="white", command=lambda: switch_frame(riserInfo_frame))
        btn_riser_info.grid(row=2, column=0, pady=10)

        # Create riserCapacities frame
        riserCapacities_frame = tk.Frame(root)
        riserCapacities_frame.grid(row=0, column=0, sticky="nsew")
        riserCapacities_app = riserCapacities(riserCapacities_frame)
        riserCapacities_frame.lower()  # Hide the frame initially
        btn_riser_capacities = tk.Button(self.frame, text="INPUT RISER CAPACITIES", width=30, height=2,
                                         bg="#333333", fg="white", command=lambda: switch_frame(riserCapacities_frame))
        btn_riser_capacities.grid(row=3, column=0, pady=10)

        # Create riserResponse frame
        riserResponse_frame = tk.Frame(root)
        riserResponse_frame.grid(row=0, column=0, sticky="nsew")
        riserResponse_frame.lower()  # Hide the frame initially
        btn_riser_response = tk.Button(self.frame, text="INPUT RISER RESPONSE", width=30, height=2,
                                       bg="#333333", fg="white", command=lambda: switch_frame(riserResponse_frame))
        btn_riser_response.grid(row=4, column=0, pady=10)

        # Create bsDimension frame
        bsDimension_frame = tk.Frame(root)
        bsDimension_frame.grid(row=0, column=0, sticky="nsew")
        bsDimension_app = BSDimensionWindow(bsDimension_frame)
        bsDimension_frame.lower()  # Hide the frame initially

        # Create bsMaterial frame
        bsMaterial_frame = tk.Frame(root)
        bsMaterial_frame.grid(row=0, column=0, sticky="nsew")
        bsMaterial_app = BSMaterialWindow(bsMaterial_frame, bsDimension_frame)
        bsMaterial_frame.lower()  # Hide the frame initially
        btn_bs_material = tk.Button(self.frame, text="INPUT BS MATERIAL", width=30, height=2,
                                    bg="#333333", fg="white", command=lambda: switch_frame(bsMaterial_frame))
        btn_bs_material.grid(row=5, column=0, pady=10)

        # Add a return button to switch back to the main_frame
        btn_return = tk.Button(self.frame, text="Return", width=30, height=2,
                               bg="#333333", fg="white", command=lambda: switch_frame(main_frame))
        btn_return.grid(row=6, column=0, pady=10)

        # button to print data from riserInfo
        btn_print_data = tk.Button(self.frame, text="Print data", width=30, height=2,
                                   bg="#333333", fg="white", command=riserInfo_app.print_data)
        btn_print_data.grid(row=7, column=0, pady=10)



def main():
    root = tk.Tk()
    root.title("Design BS Module")

    main_frame = tk.Frame(root)
    main_frame.grid(row=0, column=0, sticky="nsew")

    designBS_frame = tk.Frame(root)
    designBS_frame.grid(row=0, column=0, sticky="nsew")

    designBS_app = DesignWindow(designBS_frame, main_frame)


    # Raise the main_frame initially
    main_frame.tkraise()

    root.mainloop()

if __name__ == "__main__":
    main()