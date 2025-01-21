import tkinter as tk
from designBSwindow_module import DesignWindow # type: ignore
from maxcurve_module import DataPlotter

def switch_frame(frame):
    frame.tkraise()

def create_main_screen():
    root = tk.Tk()
    root.title("Tkinter Project")
    root.geometry("600x600")  # Set window size

# Create a frame to organize the buttons on the main screen
    frame = tk.Frame(root, padx=20, pady=20)
    frame.pack(expand=True)

# Add buttons in a column with space between them
    btn_design_bs = tk.Button(frame, text="DESIGN BS", command=switch_frame, width=100, height=2, 
                          bg="#333333", fg="white")
    btn_design_bs.pack(pady=10)  # Vertical spacing

# Create the button
    btn_check_bs = tk.Button(frame, text="CHECK IF EXISTING BS IS SUITABLE", width=100, height=2, 
                         bg="#333333", fg="white", command=switch_frame)
    btn_check_bs.pack(pady=50)

    btn_run_cases = tk.Button(frame, text="RUN LOAD CASES ON EXISTING BS", width=100, height=2, 
                          bg="#333333", fg="white", command=switch_frame)
    btn_run_cases.pack(pady=30)  # Vertical spacing
    return root



def main():
    #create frames for all the screens
    root = tk.Tk()
    root.title("GUI with Buttons and Input Fields")

    # Create main frame
    main_frame = tk.Frame(root)
    main_frame.grid(row=0, column=0, sticky="nsew")

    # Create maxcurve frame
    maxcurve_frame = tk.Frame(root)
    maxcurve_frame.grid(row=0, column=0, sticky="nsew")

    button5 = tk.Button(main_frame, text="Open MaxCurve GUI", command=lambda: switch_frame(maxcurve_frame))
    button5.grid(row=3, column=3, padx=10, pady=10)

    maxcurve_app = DataPlotter(maxcurve_frame)
    # Add a return button to switch back to the main_frame
    return_button = tk.Button(maxcurve_frame, text="Return", command=lambda: switch_frame(main_frame))
    return_button.pack(pady=10)

    main_frame.tkraise()

    root.mainloop()

# Main application window


# Run the application
main()