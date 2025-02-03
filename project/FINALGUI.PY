import tkinter as tk
from designBS_module import DesignWindow
from riserCapacities_module import riserCapacities
from projectInfo_module import ProjectInfoWindow
from riserInfo_module import RiserInfoWindow
from bsMaterialScreen_module import BSMaterialWindow
from bsDimensionScreen_module import BSDimensionWindow

def switch_frame(frame):
    frame.tkraise()

def create_main_screen(frame, designBS_frame):
    
    btn_design_bs = tk.Button(frame, text="DESIGN BS", command=lambda:switch_frame(designBS_frame), width=100, height=2, 
                          bg="#333333", fg="white")
    btn_design_bs.pack(pady=10)  # Vertical spacing

    btn_check_bs = tk.Button(frame, text="CHECK IF EXISTING BS IS SUITABLE", width=100, height=2, 
                         bg="#333333", fg="white", command=switch_frame)
    btn_check_bs.pack(pady=50)

    btn_run_cases = tk.Button(frame, text="RUN LOAD CASES ON EXISTING BS", width=100, height=2, 
                          bg="#333333", fg="white", command=switch_frame)
    btn_run_cases.pack(pady=30)  # Vertical spacing
    return frame



def main():
    #create frames for all the screens
    root = tk.Tk()
    root.title("GUI with Buttons and Input Fields")

    root.protocol("WM_DELETE_WINDOW", root.quit)

    # Create main frame
    main_frame = tk.Frame(root)
    main_frame.grid(row=0, column=0, sticky="nsew")
    # Create designBS frame
    designBS_frame = tk.Frame(root)
    designBS_frame.grid(row=0, column=0, sticky="nsew")
    designBS_app = DesignWindow(designBS_frame, main_frame, root)
    #populate main screen
    create_main_screen(main_frame, designBS_frame)
    main_frame.tkraise()

    root.mainloop()
    print("Application closed")
    root.destroy()  
# Main application window


# Run the application
main()