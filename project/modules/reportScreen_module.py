import tkinter as tk
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from tkinter import ttk
import re


class ReportScreen:
    def __init__(self, root, prev_frame, show_frame, capacities, response, case, data, threshold_normal, threshold_abnormal):
        self.root = root
        self.prev_frame = prev_frame
        self.show_frame = show_frame
        self.capacities = capacities
        self.response = response
        self.frame = self.root
        self.case = case
        self.normal_curves, self.abnormal_curves = self.getCurves(self.case)
        self.threshold_normal, self.threshold_abnormal = threshold_normal, threshold_abnormal

        # Main container frame
        self.main_frame = tk.Frame(self.frame)
        self.main_frame.pack(fill="both", expand=True)

        # Create a frame for the table (left side)
        self.table_frame = ttk.Frame(self.main_frame)
        self.table_frame.pack(side="left", fill="both", expand=True, padx=10, pady=10)

        self.create_tkinter_table(self.table_frame, data)

        # Create a frame for the plot (right side)
        self.plot_frame = ttk.Frame(self.main_frame)
        self.plot_frame.pack(side="right", fill="both", expand=True, padx=10, pady=10)

        # Matplotlib figure
        self.figure, self.ax = plt.subplots(figsize=(6, 6))
        self.ax.set_title("Tension vs. Curvature")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)

        self.canvas = FigureCanvasTkAgg(self.figure, self.plot_frame)
        self.canvas.get_tk_widget().pack(fill="both", expand=True)

        self.update_plot()

    def update_plot(self):
        """Update the plot with the entered data."""
        curvature_normal_cap, tension_normal_cap = self.capacities["normal"]
        curvature_abnormal_cap, tension_abnormal_cap = self.capacities["abnormal"]

        curvature_normal_resp, tension_normal_resp = self.response["normal"]
        curvature_abnormal_resp, tension_abnormal_resp = self.response["abnormal"]

        curvature_normal_resp = [float(curve["maximum_bs_curvature"]) for curve in self.normal_curves]
        curvature_abnormal_resp = [float(curve["maximum_bs_curvature"]) for curve in self.abnormal_curves]

        self.ax.clear()
        self.ax.set_title(f"BS Response at NO and AO Loads for case: {self.case}")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)

        if curvature_normal_cap and tension_normal_cap:
            self.ax.plot(curvature_normal_cap, tension_normal_cap, marker='o', label="Normal Operation", color='blue', linewidth=0.5, markersize=5)
        if curvature_abnormal_cap and tension_abnormal_cap:
            self.ax.plot(curvature_abnormal_cap, tension_abnormal_cap, marker='s', label="Abnormal Operation", color='red', linewidth=0.5, markersize=5)
        if curvature_normal_resp and tension_normal_resp:
            self.ax.plot(curvature_normal_resp, tension_normal_resp, marker='o', label="Normal Operation response", color='blue', linewidth=0.5, markersize=5, markerfacecolor='yellow')
        if curvature_abnormal_resp and tension_abnormal_resp:
            self.ax.plot(curvature_abnormal_resp, tension_abnormal_resp, marker='s', label="Abnormal Operation response", color='red', linewidth=0.5, markersize=5, markerfacecolor='yellow')

        self.ax.legend()
        self.canvas.draw()

    def create_tkinter_table(self, frame, data):
        """Creates tables inside a Tkinter frame using Treeview to display dictionary data."""
            
        def create_table(parent, title, columns, values):
            """Helper function to create and pack a table with title, columns, and values."""
            label = tk.Label(parent, text=title, font=("Arial", 12, "bold"), anchor="w")
            label.pack(anchor="w", pady=5)

            tree = ttk.Treeview(parent, columns=columns, show="headings", height=min(10, len(values)))
                
            # Set column headings
            for col in columns:
                tree.heading(col, text=col)
                tree.column(col, width=150, anchor="center")

            # Insert data
            for val in values:
                tree.insert("", "end", values=val)

            tree.pack(fill="both", expand=True, pady=5)

        # Project Information Table
        project_info = [(k, v) for k, v in data["project_info"].items()]
        create_table(frame, "Project Information", ["Field", "Value"], project_info)

        # Riser Information Table
        riser_info = [(k, v) for k, v in data["riser_info"].items()]
        create_table(frame, "Riser Information", ["Field", "Value"], riser_info)

        # BS Dimensions Table
        bs_dimensions = [(k, v) for k, v in data["bs_dimension"].items()]
        create_table(frame, "BS Dimensions", ["Field", "Value"], bs_dimensions)

        # BS Material Table
        material_data = []
        for section, material in data["bs_material"].items():
            material_data.append((section, material["material_characteristics"], material["elastic_modules"]))
        create_table(frame, "BS Material", ["Section", "Material", "Elastic Modules"], material_data)

        case_data = []
        i = 0
        for threshold in self.threshold_normal.items():
            case_data.append((i+1, round(self.response["normal"][1][i], 5), round(self.response["normal"][0][i], 5), round(threshold[1]["maximum_curvature"], 5), round(float(self.normal_curves[i]["maximum_curvature"]), 5)))
            i += 1
        create_table(frame, "Threshold Normal", ["Case", "Tension", "Angle", "Threshold", "Max Curvature"], case_data)

        case_data_abnormal = []
        i = 0
        for threshold in self.threshold_abnormal.items():
            case_data_abnormal.append((i+1, round(self.response["abnormal"][1][i], 5), round(self.response["abnormal"][0][i], 5), round(threshold[1]["maximum_curvature"], 5), round(float(self.abnormal_curves[i]["maximum_curvature"]), 5)))
            i += 1
        create_table(frame, "Threshold Abnormal", ["Case", "Tension", "Angle", "Threshold", "Max Curvature"], case_data_abnormal)

    def getCurves(self, case):
        noOfCases = len(self.response['normal'][0])
        length = float(case.split('-')[2])
        width = float(case.split('-')[3].replace('D_30.inp', ''))
        normal_curves = []
        for i in range(1, noOfCases + 1):
            case_file = f"case_files\\Case-normal{i}-{length}-{width}-60D_30.log"
            data = self.extract_data(case_file)
            normal_curves.append(data)

        abnormal_curves = []
        for i in range(1, noOfCases + 1):
            case_file = f"case_files\\Case-abnormal{i}-{length}-{width}-60D_30.log"
            data = self.extract_data(case_file)
            abnormal_curves.append(data)

        return normal_curves, abnormal_curves

    def extract_data(self, case_file):
        with open(case_file, 'r') as res_file:
            res_lines = res_file.readlines()

        keyres1 = None
        keyres2 = None

        for line in res_lines:
            if re.search(r'Maximum BS curvature', line):
                keyres1 = line.strip().split(':')[-1].strip()
            if re.search(r'Maximum curvature', line):
                keyres2 = line.strip().split(':')[-1].strip()

        if keyres1 and keyres2:
            return {
                "case_name": case_file.rstrip('.log'),
                "maximum_bs_curvature": keyres1,
                "maximum_curvature": keyres2
            }
        else:
            print(f"Key results not found in {case_file}")
            return None
def main():
    root = tk.Tk()
    prev_frame = tk.Frame(root)
    prev_frame.pack(fill="both", expand=True)
    data = {
    'project_info': {
        'project_name': 'episkbøystiver',
        'client': 'knut ivar',
        'designer_name': 'lars opheim'
    },
    'riser_info': {
        'Riser Identification': 'riser1',
        'Outer Diameter': '0.22',
        'Outer Diameter Tolerance': '0.003',
        'Mass Per Unit Length': '88.4',
        'Axial Stiffness': '1040000',
        'Bending Stiffness': '185',
        'Torsial Stiffness': '143',
        'Riser Length': '5'
    },
    'riser_capacities': {
        'normal': [
            [0.08197, 0.07752, 0.07353, 0.06944, 0.06536, 0.05714, 0.04902, 0.04082, 0.03268, 0.02451], [0.0, 87.0, 174.0, 261.0, 348.0, 522.0, 696.0, 870.0, 1044.0, 1218.0]
        ],
        'abnormal': [
            [0.11765, 0.11236, 0.10753, 0.10204, 0.09709, 0.08696, 0.07519, 0.0625, 0.05, 0.03125], [0.0, 133.0, 266.0, 399.0, 532.0, 798.0, 1064.0, 1330.0, 1596.0, 1676.0]
        ],
    },
    'riser_response': {
        'normal': [
            [4.0, 18.0, 23.0, 23.0, 16.0, 10.0, 2.0], [550.0, 650.0, 750.0, 850.0, 1000.0, 1050.0, 1100.0]
        ],
        'abnormal': [
            [15.0, 20.0, 25.0, 26.0, 25.0, 20.0, 5.0], [470.0, 490.0, 700.0, 800.0, 930.0, 1020.0, 1140.0]
        ],
    },
    'bs_dimension': {
        'Input root length:': '0.650',
        'Input tip length:': '0.150',
        'Input MIN root OD:': '1',
        'Input MAX root OD:': '2',
        'Input MIN overall length:': '10',
        'Input MAX overall length:': '13',
        'Input clearance:': '0.010',
        'Input thidm(??):': 'fse',
        'BS ID:': '101',
        'Increment Width:': '0.10',
        'Increment Length:': '0.250'
    },
    'bs_material': {
        'Section 1': {
            'material_characteristics': 'Steel',
            'elastic_modules': '23'
        },
        'Section 2': {
            'material_characteristics': 'Titanium',
            'elastic_modules': '3'
        }
    }
}
    root.protocol("WM_DELETE_WINDOW", root.quit)
    case = 'Case-normal4-1.6-12.75-60D_30.inp'
    threshold_normal = {'case_files\\Case-normal1': {'maximum_curvature': np.float64(0.05583333333333334)}, 'case_files\\Case-normal2': {'maximum_curvature': np.float64(0.051166666666666666)}, 'case_files\\Case-normal3': {'maximum_curvature': np.float64(0.046475172413793106)}, 'case_files\\Case-normal4': {'maximum_curvature': np.float64(0.04176252873563219)}, 'case_files\\Case-normal5': {'maximum_curvature': np.float64(0.0347383908045977)}, 'case_files\\Case-normal6': {'maximum_curvature': np.float64(0.03239827586206897)}, 'case_files\\Case-normal7': {'maximum_curvature': np.float64(0.030050574712643677)}}
    threshold_abnormal = {'case_files\\Case-abnormal1': {'maximum_curvature': np.float64(0.09939751879699248)}, 'case_files\\Case-abnormal2': {'maximum_curvature': np.float64(0.09865315789473685)}, 'case_files\\Case-abnormal3': {'maximum_curvature': np.float64(0.0906921052631579)}, 'case_files\\Case-abnormal4': {'maximum_curvature': np.float64(0.08687150375939849)}, 'case_files\\Case-abnormal5': {'maximum_curvature': np.float64(0.08111924812030075)}, 'case_files\\Case-abnormal6': {'maximum_curvature': np.float64(0.07713691729323309)}, 'case_files\\Case-abnormal7': {'maximum_curvature': np.float64(0.07156428571428572)}}
    app = ReportScreen(root, prev_frame, lambda frame: frame.tkraise(), data['riser_capacities'], data['riser_response'],case, data, threshold_normal, threshold_abnormal)
    root.mainloop()
    root.destroy()

if __name__ == "__main__":
    main()