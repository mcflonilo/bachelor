import tkinter as tk
from tkinter import ttk
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import numpy as np
# Function to handle "Input Riser Capacities"
def open_riser_capacities_screen():
    # Create a new window for riser capacities input
    riser_capacities_window = tk.Toplevel(root)
    riser_capacities_window.title("Riser Capacities")
    riser_capacities_window.geometry("1100x300")  # Adjust window size as needed

    # Add title label
    lbl_title = tk.Label(riser_capacities_window, text="RISER CAPACITIES", font=("Arial", 14))
    lbl_title.grid(row=0, column=0, columnspan=6, pady=10)

    # Normal Operation Section
    lbl_normal = tk.Label(riser_capacities_window, text="• Normal operation (80% utilization)", anchor="w", font=("Arial", 12))
    lbl_normal.grid(row=1, column=0, sticky="w", padx=20, pady=5)

    lbl_normal_data_points = tk.Label(riser_capacities_window, text="No. of data points:")
    lbl_normal_data_points.grid(row=1, column=1, sticky="e", padx=5)
    entry_normal_data_points = tk.Entry(riser_capacities_window, width=10)
    entry_normal_data_points.grid(row=1, column=2, padx=5)

    # Abnormal Operation Section
    lbl_abnormal = tk.Label(riser_capacities_window, text="• Abnormal operation (100% utilization)", anchor="w", font=("Arial", 12))
    lbl_abnormal.grid(row=1, column=3, sticky="w", padx=20, pady=5)

    lbl_abnormal_data_points = tk.Label(riser_capacities_window, text="No. of data points:")
    lbl_abnormal_data_points.grid(row=1, column=4, sticky="e", padx=5)
    entry_abnormal_data_points = tk.Entry(riser_capacities_window, width=10)
    entry_abnormal_data_points.grid(row=1, column=5, padx=5)

    # Tables for Normal and Abnormal operations
    def create_table(parent, start_row, start_column, labels=("Curvature", "Tension")):
        # Header row
        for col, label in enumerate(labels):
            tk.Label(parent, text=label, width=15, anchor="center", font=("Arial", 10, "bold")).grid(row=start_row, column=start_column + col, padx=5, pady=5)

        # Input rows
        for row in range(5):
            tk.Entry(parent, width=15).grid(row=start_row + row + 1, column=start_column, padx=5, pady=2)
            tk.Entry(parent, width=15).grid(row=start_row + row + 1, column=start_column + 1, padx=5, pady=2)

    # Add Normal and Abnormal operation tables side by side
    create_table(riser_capacities_window, start_row=2, start_column=0)
    create_table(riser_capacities_window, start_row=2, start_column=3)

    # Add OK and CANCEL buttons
    frame_buttons = tk.Frame(riser_capacities_window)
    frame_buttons.grid(row=8, column=0, columnspan=6, pady=20)

    btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white")
    btn_ok.pack(side=tk.LEFT, padx=10)

    btn_cancel = tk.Button(frame_buttons, text="CANCEL", width=10, height=2, bg="#333333", fg="white", 
                           command=riser_capacities_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)
def open_riser_RESPONSE_screen():
    # Create a new window for riser capacities input
    riser_capacities_window = tk.Toplevel(root)
    riser_capacities_window.title("Riser Response")
    riser_capacities_window.geometry("1100x300")  # Adjust window size as needed

    # Add title label
    lbl_title = tk.Label(riser_capacities_window, text="RISER Response", font=("Arial", 14))
    lbl_title.grid(row=0, column=0, columnspan=6, pady=10)

    # Normal Operation Section
    lbl_normal = tk.Label(riser_capacities_window, text="• Normal operation (80% utilization)", anchor="w", font=("Arial", 12))
    lbl_normal.grid(row=1, column=0, sticky="w", padx=20, pady=5)

    lbl_normal_data_points = tk.Label(riser_capacities_window, text="No. of data points:")
    lbl_normal_data_points.grid(row=1, column=1, sticky="e", padx=5)
    entry_normal_data_points = tk.Entry(riser_capacities_window, width=10)
    entry_normal_data_points.grid(row=1, column=2, padx=5)

    # Abnormal Operation Section
    lbl_abnormal = tk.Label(riser_capacities_window, text="• Abnormal operation (100% utilization)", anchor="w", font=("Arial", 12))
    lbl_abnormal.grid(row=1, column=3, sticky="w", padx=20, pady=5)

    lbl_abnormal_data_points = tk.Label(riser_capacities_window, text="No. of data points:")
    lbl_abnormal_data_points.grid(row=1, column=4, sticky="e", padx=5)
    entry_abnormal_data_points = tk.Entry(riser_capacities_window, width=10)
    entry_abnormal_data_points.grid(row=1, column=5, padx=5)

    # Tables for Normal and Abnormal operations
    def create_table(parent, start_row, start_column, labels=("Angle", "Tension")):
        # Header row
        for col, label in enumerate(labels):
            tk.Label(parent, text=label, width=15, anchor="center", font=("Arial", 10, "bold")).grid(row=start_row, column=start_column + col, padx=5, pady=5)

        # Input rows
        for row in range(5):
            tk.Entry(parent, width=15).grid(row=start_row + row + 1, column=start_column, padx=5, pady=2)
            tk.Entry(parent, width=15).grid(row=start_row + row + 1, column=start_column + 1, padx=5, pady=2)

    # Add Normal and Abnormal operation tables side by side
    create_table(riser_capacities_window, start_row=2, start_column=0)
    create_table(riser_capacities_window, start_row=2, start_column=3)

    # Add OK and CANCEL buttons
    frame_buttons = tk.Frame(riser_capacities_window)
    frame_buttons.grid(row=8, column=0, columnspan=6, pady=20)

    btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white")
    btn_ok.pack(side=tk.LEFT, padx=10)

    btn_cancel = tk.Button(frame_buttons, text="CANCEL", width=10, height=2, bg="#333333", fg="white", 
                           command=riser_capacities_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)
# Function to handle "Input BS Information"
def open_bs_information_screen():
    # Create a new window for BS Information input
    bs_info_window = tk.Toplevel(root)
    bs_info_window.title("BS Information")
    bs_info_window.geometry("600x500")  # Adjust window size as needed

    def create_bs_section(parent, section_title, row):
        # Add section title
        lbl_section_title = tk.Label(parent, text=section_title, font=("Arial", 12, "bold"), anchor="w")
        lbl_section_title.grid(row=row, column=0, columnspan=2, sticky="w", pady=10, padx=20)

        # Add "No. of materials to include" input
        lbl_no_of_materials = tk.Label(parent, text="• No. of materials to include: (få en input box her)")
        lbl_no_of_materials.grid(row=row + 1, column=0, sticky="w", padx=20)
        entry_no_of_materials = tk.Entry(parent, width=10)
        entry_no_of_materials.grid(row=row + 1, column=1, sticky="w", padx=10)

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

        # Add separator line
        tk.Canvas(parent, height=2, width=580, bg="black").grid(row=row + 4, column=0, columnspan=3, pady=10)

    # Add BS Materials 1/2 section
    create_bs_section(bs_info_window, "BS Materials 1/2", row=0)

    # Add BS Materials 2/2 section
    create_bs_section(bs_info_window, "BS Materials 2/2", row=6)

    # Add OK and CANCEL buttons
    frame_buttons = tk.Frame(bs_info_window)
    frame_buttons.grid(row=12, column=0, columnspan=3, pady=20)

    btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white",
                       command=lambda: [bs_info_window.destroy(), open_bs_geometry_constraints_screen()])
    btn_ok.pack(side=tk.LEFT, padx=10)

    btn_cancel = tk.Button(frame_buttons, text="CANCEL", width=10, height=2, bg="#333333", fg="white", 
                           command=bs_info_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)
def open_run_analysis_screen():
    # Create a new window for Run Analysis
    analysis_window = tk.Toplevel()
    analysis_window.title("Run Analysis Screen")
    analysis_window.geometry("800x800")

    # Placeholder frame for plots
    plot_frame = tk.Frame(analysis_window, bg="white", width=600, height=400)
    plot_frame.pack(pady=10)

    # Label for results
    results_label = tk.Label(analysis_window, text="Results:")
    results_label.pack()

    
    # Generate dummy data
    x = np.linspace(0, 1, 100)
    y1 = 1 - x**2  # Dummy curve for Riser Capacity
    y2 = x**2      # Dummy curve for Riser Response

    # Plotting
    fig, axes = plt.subplots(2, 2, figsize=(8, 6))  # Subplots for 4 plots
    fig.subplots_adjust(wspace=0.4, hspace=0.6)

    # Normal Operation
    axes[0, 0].plot(x, y1, color='red')
    axes[0, 0].set_title("Riser Capacity (Normal)")
    axes[0, 0].set_xlabel("Curvature")
    axes[0, 0].set_ylabel("Tension")
    axes[0, 1].plot(x, y2, color='blue')
    axes[0, 1].set_title("Riser Response (Normal)")
    axes[0, 1].set_xlabel("Curvature")
    axes[0, 1].set_ylabel("Tension")

        # Abnormal Operation
    axes[1, 0].plot(x, y1 + 0.2, color='red')
    axes[1, 0].set_title("Riser Capacity (Abnormal)")
    axes[1, 0].set_xlabel("Curvature")
    axes[1, 0].set_ylabel("Tension")
    axes[1, 1].plot(x, y2 - 0.2, color='blue')
    axes[1, 1].set_title("Riser Response (Abnormal)")
    axes[1, 1].set_xlabel("Curvature")
    axes[1, 1].set_ylabel("Tension")

    # Clear previous plot if any
    for widget in plot_frame.winfo_children():
        widget.destroy()

    # Embed the new plot in Tkinter
    canvas = FigureCanvasTkAgg(fig, master=plot_frame)
    canvas_widget = canvas.get_tk_widget()
    canvas_widget.pack()

    # Display the plot
    canvas.draw()


    btn_ok = tk.Button(analysis_window, text="OK", width=10, height=2, bg="#333333", fg="white")
    btn_ok.pack(side=tk.LEFT, padx=10)

    # Cancel button to close the window
    btn_cancel = tk.Button(analysis_window, text="CANCEL", width=10, height=2, bg="#333333", fg="white", 
                           command=analysis_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)
# Function to handle "BS Geometry Constraints" screen
def open_bs_geometry_constraints_screen():
    # Create a new window for BS Geometry Constraints
    bs_geometry_window = tk.Toplevel(root)
    bs_geometry_window.title("BS Geometry Constraints")
    bs_geometry_window.geometry("500x400")  # Adjust window size as needed

    # Add title label
    lbl_title = tk.Label(bs_geometry_window, text="BS Geometry Constraints:", font=("Arial", 12, "bold"), anchor="w")
    lbl_title.grid(row=0, column=0, columnspan=2, sticky="w", pady=10, padx=20)

    # Define input labels and fields
    labels = [
        "Input root length:", "Input tip length:", "Input MIN root OD:",
        "Input MAX root OD:", "Input MIN overall length:", "Input MAX overall length:",
        "Input clearance:", "Input thidm(??):", "BS ID:"
    ]

    for i, label_text in enumerate(labels):
        lbl = tk.Label(bs_geometry_window, text=label_text, anchor="w")
        lbl.grid(row=i + 1, column=0, sticky="w", padx=20, pady=5)
        entry = tk.Entry(bs_geometry_window, width=20)
        entry.grid(row=i + 1, column=1, padx=10, pady=5)

    # Add OK and CANCEL buttons
    frame_buttons = tk.Frame(bs_geometry_window)
    frame_buttons.grid(row=len(labels) + 1, column=0, columnspan=2, pady=20)

    
    btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white",
                       command=lambda: [bs_geometry_window.destroy(), open_analysis_screen()])
    btn_ok.pack(side=tk.LEFT, padx=10)

    btn_cancel = tk.Button(frame_buttons, text="CANCEL", width=10, height=2, bg="#333333", fg="white",
                           command=bs_geometry_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)
# Function to handle "Input Riser Information"
def open_analysis_screen():
    # Create the new window
    analysis_window = tk.Toplevel(root)
    analysis_window.title("Run Analysis")
    analysis_window.geometry("600x400")  # Adjust window size as needed
    analysis_window.configure(bg="white")  # Set background color

    # Add "Run Analysis" label
    lbl_run_analysis = tk.Label(analysis_window, text="Run Analysis:", font=("Arial", 10), bg="white")
    lbl_run_analysis.place(x=20, y=20)  # Position at the top left

    # Add "Thread count" label and input field
    lbl_thread_count = tk.Label(analysis_window, text="Thread count:", font=("Arial", 10), bg="white")
    lbl_thread_count.place(x=250, y=70)  # Centered near the top
    entry_thread_count = tk.Entry(analysis_window, width=10)
    entry_thread_count.place(x=350, y=70)

    # Add buttons for changing parameters and finding optimal BS
    btn_change_riser = tk.Button(
        analysis_window, text="Change Riser Model Parameters", width=30, height=2, bg="#e0e0e0"
    )
    btn_change_riser.place(x=200, y=120)

    btn_change_fe = tk.Button(
        analysis_window, text="Change FE-analysis Parameters", width=30, height=2, bg="#e0e0e0"
    )
    btn_change_fe.place(x=200, y=180)

    btn_find_optimal_bs = tk.Button(
        analysis_window, text="Find Optimal BS!", width=30, height=2, bg="#e0e0e0"
    )
    btn_find_optimal_bs.place(x=200, y=240)

    # Add RUN and CANCEL buttons at the bottom right
    btn_run = tk.Button(
        analysis_window, text="RUN", width=10, height=2, bg="#333333", fg="white", command=open_run_analysis_screen,
    )
    btn_run.place(x=450, y=320)

    btn_cancel = tk.Button(
        analysis_window, text="CANCEL", width=10, height=2, bg="#333333", fg="white", command=analysis_window.destroy
    )
    btn_cancel.place(x=540, y=320)

def open_riser_info_screen():
    # Create a new window for riser information input
    riser_info_window = tk.Toplevel(root)
    riser_info_window.title("Riser Information")
    riser_info_window.geometry("400x600")  # Adjust window size as needed

    # Add title label
    lbl_title = tk.Label(riser_info_window, text="RISER INFORMATION", font=("Arial", 14))
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
    entries = {}

    for field in fields:
        lbl = tk.Label(riser_info_window, text=f"{field}:", anchor="w")
        lbl.pack(fill="x", padx=20)
        entry = tk.Entry(riser_info_window)
        entry.pack(fill="x", padx=20, pady=5)
        entries[field] = entry

    # Add OK and CANCEL buttons
    frame_buttons = tk.Frame(riser_info_window)
    frame_buttons.pack(pady=20)

    btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white")
    btn_ok.pack(side=tk.LEFT, padx=10)

    # Cancel button to close the window
    btn_cancel = tk.Button(frame_buttons, text="CANCEL", width=10, height=2, bg="#333333", fg="white", 
                           command=riser_info_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)

# Function to handle "Input Project Information"
def open_project_info_screen():
    # Create a new window for project information input
    project_info_window = tk.Toplevel(root)
    project_info_window.title("Project Information")
    project_info_window.geometry("400x400")  # Adjust window size as needed

    # Add title label
    lbl_title = tk.Label(project_info_window, text="PROJECT INFORMATION", font=("Arial", 14))
    lbl_title.pack(pady=10)

    # Project Name
    lbl_project_name = tk.Label(project_info_window, text="PROJECT NAME:", anchor="w")
    lbl_project_name.pack(fill="x", padx=20)
    entry_project_name = tk.Entry(project_info_window)
    entry_project_name.pack(fill="x", padx=20, pady=5)

    # Client
    lbl_client = tk.Label(project_info_window, text="CLIENT:", anchor="w")
    lbl_client.pack(fill="x", padx=20)
    entry_client = tk.Entry(project_info_window)
    entry_client.pack(fill="x", padx=20, pady=5)

    # Designer Name
    lbl_designer_name = tk.Label(project_info_window, text="DESIGNER NAME:", anchor="w")
    lbl_designer_name.pack(fill="x", padx=20)
    entry_designer_name = tk.Entry(project_info_window)
    entry_designer_name.pack(fill="x", padx=20, pady=5)

    # Add OK and CANCEL buttons
    frame_buttons = tk.Frame(project_info_window)
    frame_buttons.pack(pady=20)

    btn_ok = tk.Button(frame_buttons, text="OK", width=10, height=2, bg="#333333", fg="white")
    btn_ok.pack(side=tk.LEFT, padx=10)

    # Cancel button to close the window
    btn_cancel = tk.Button(frame_buttons, text="CANCEL", width=10, height=2, bg="#333333", fg="white", 
                           command=project_info_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)

# Function to open the "Design Optimal BS" screen
def open_design_screen():
    # Create a new window for "DESIGN OPTIMAL BS"
    design_window = tk.Toplevel(root)
    design_window.title("Design Optimal BS")
    design_window.geometry("400x400")  # Adjust window size as needed

    # Add buttons in the new screen
    btn_project_info = tk.Button(design_window, text="INPUT PROJECT INFORMATION", width=30, height=2, 
                                 bg="#333333", fg="white", command=open_project_info_screen)
    btn_project_info.pack(pady=10)

    btn_riser_info = tk.Button(design_window, text="INPUT RISER INFORMATION", width=30, height=2, 
                               bg="#333333", fg="white", command=open_riser_info_screen)
    btn_riser_info.pack(pady=10)

    btn_riser_capacities = tk.Button(design_window, text="INPUT RISER CAPACITIES", width=30, height=2, 
                                     bg="#333333", fg="white",command=open_riser_capacities_screen)
    btn_riser_capacities.pack(pady=10)

    btn_riser_response = tk.Button(design_window, text="INPUT RISER RESPONSE", width=30, height=2, 
                                   bg="#333333", fg="white",command=open_riser_RESPONSE_screen)
    btn_riser_response.pack(pady=10)

    btn_bs_info = tk.Button(design_window, text="INPUT BS INFORMATION", width=30, height=2, 
                            bg="#333333", fg="white",command=open_bs_information_screen)
    btn_bs_info.pack(pady=10)

    # Add OK and CANCEL buttons
    frame = tk.Frame(design_window)
    frame.pack(pady=20)

    btn_ok = tk.Button(frame, text="OK", width=10, height=2, bg="#333333", fg="white")
    btn_ok.pack(side=tk.LEFT, padx=10)

    # Cancel button to close the window
    btn_cancel = tk.Button(frame, text="CANCEL", width=10, height=2, bg="#333333", fg="white", 
                           command=design_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)
def show_check_screen():
    # Create a new top-level window
    check_window = tk.Toplevel()
    check_window.title("Check Existing BS")
    
    # Set window size
    check_window.geometry("1000x1000")
    check_window.configure(bg="white")
    
    # Create label
    label = tk.Label(check_window, text="Check if existing BS is suitable", bg="white", font=("Arial", 14))
    label.pack(pady=20)
    
    # Create a placeholder for the question mark
    question_label = tk.Label(check_window, text="?", bg="white", font=("Arial", 48))
    question_label.pack(pady=20)
    
    # Create OK button
    btn_ok = tk.Button(check_window, text="OK", width=10, height=2, bg="#333333", fg="white")
    btn_ok.pack(side=tk.LEFT, padx=20, pady=20)
    
    # Create Cancel button
    btn_cancel = tk.Button(check_window, text="CANCEL", width=10, height=2, bg="#333333", fg="white", command=check_window.destroy)
    btn_cancel.pack(side=tk.RIGHT, padx=20, pady=20)

def open_run_cases_screen():
    # Create a new window for running load cases
    run_cases_window = tk.Toplevel(root)
    run_cases_window.title("Run Load Cases")
    run_cases_window.geometry("600x400")  # Adjust window size as needed

    # Add title label
    lbl_title = tk.Label(run_cases_window, text="RUN LOAD CASES ON EXISTING BS", font=("Arial", 14))
    lbl_title.pack(pady=10)

    # Add buttons in the new screen
    btn_project_info = tk.Button(run_cases_window, text="INPUT PROJECT INFORMATION", width=30, height=2, 
                                 bg="#333333", fg="white", command=open_project_info_screen)
    btn_project_info.pack(pady=10)

    btn_riser_info = tk.Button(run_cases_window, text="INPUT RISER INFORMATION", width=30, height=2, 
                               bg="#333333", fg="white", command=open_riser_info_screen)
    btn_riser_info.pack(pady=10)

    

    btn_riser_response = tk.Button(run_cases_window, text="INPUT RISER RESPONSE", width=30, height=2, 
                                   bg="#333333", fg="white",command=open_riser_RESPONSE_screen)
    btn_riser_response.pack(pady=10)

    btn_bs_info = tk.Button(run_cases_window, text="INPUT BS INFORMATION", width=30, height=2, 
                            bg="#333333", fg="white",command=open_bs_information_screen)
    btn_bs_info.pack(pady=10)

    # Add OK and CANCEL buttons
    frame = tk.Frame(run_cases_window)
    frame.pack(pady=20)

    btn_ok = tk.Button(frame, text="OK", width=10, height=2, bg="#333333", fg="white")
    btn_ok.pack(side=tk.LEFT, padx=10)

    # Cancel button to close the window
    btn_cancel = tk.Button(frame, text="CANCEL", width=10, height=2, bg="#333333", fg="white", 
                           command=run_cases_window.destroy)
    btn_cancel.pack(side=tk.LEFT, padx=10)
# Main application window
root = tk.Tk()
root.title("Tkinter Project")
root.geometry("600x600")  # Set window size

# Create a frame to organize the buttons on the main screen
frame = tk.Frame(root, padx=20, pady=20)
frame.pack(expand=True)

# Add buttons in a column with space between them
btn_design_bs = tk.Button(frame, text="DESIGN BS", command=open_design_screen, width=100, height=2, 
                          bg="#333333", fg="white")
btn_design_bs.pack(pady=10)  # Vertical spacing

# Create the button
btn_check_bs = tk.Button(frame, text="CHECK IF EXISTING BS IS SUITABLE", width=100, height=2, 
                         bg="#333333", fg="white", command=show_check_screen)
btn_check_bs.pack(pady=50)

btn_run_cases = tk.Button(frame, text="RUN LOAD CASES ON EXISTING BS", width=100, height=2, 
                          bg="#333333", fg="white", command=open_run_cases_screen)
btn_run_cases.pack(pady=30)  # Vertical spacing

# Run the application
root.mainloop()
