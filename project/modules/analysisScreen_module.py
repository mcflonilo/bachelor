import tkinter as tk
import os
import matplotlib.pyplot as plt
from matplotlib.figure import Figure
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import numpy as np
from scipy.interpolate import interp1d
def switch_frame(frame):
    frame.tkraise()

def generate_case_files(length, EA, EI, GT, m, cases, ID, SL, OD, CL, TL, TOD, MAT, MATID):
    inp1 = open('bsengine-cases.txt', 'w')

    x = 0 # Counter
    y = 0
    for i in cases:
        x = 0
        y = y + 1
        for j in OD:
            x = 0
            for k in CL:
                x = 0
                for l in MAT:
                    x = 0
                    q = MATID[x]
                    x = x + 1
                    # Create a directory for the case files if it doesn't exist
                    if not os.path.exists('case_files'):
                        os.makedirs('case_files')

                    inp = open(os.path.join('case_files', "Case" + str(y) + '-' + str(j) + '-' + str(k) + '-' + str(q) + ".inp"), 'w')
                    inp1.write(os.path.join('case_files', "Case" + str(y) + '-' + str(j) + '-' + str(k) + '-' + str(q) + ".inp") + '\n')

                    inp.write('BEND STIFFENER DATA'+'\n')
                    inp.write("' ID   NSEG"+'\n')
                    inp.write("' inner diameter      Number of linear segments"+'\n')
                    inp.write(str(ID) + "   3"+'\n')
                    inp.write("' LENGTH   NEL   OD1    OD2  DENSITY LIN/NOLIN        EMOD/MAT-ID"+'\n')
                    inp.write(str(SL) + "  50  " + str(j) + "  " + str(j) + "  2000  LIN  10000.E06"+'\n')
                    inp.write(str(k) + "  100  " + str(j) + "  " + str(TOD) + "  1150  " + str(l)+'\n')
                    inp.write(str(TL) + "  50  " + str(TOD) + "  " + str(TOD) + "  1150  " + str(l)+'\n')
                    inp.write("'-----------------------------------------------------------------------"+'\n')
                    inp.write("'"+'\n')
                    inp.write("RISER DATA"+'\n')
                    inp.write("'SRIS,  NEL   EI,    EA,      GT     Mass"+'\n')
                    inp.write("' (m)         kNm^2  kN              kg/m"+'\n')
                    inp.write(str(length) + "  100  " + str(EI) + "  " + str(EA) + "  " + str(GT) + "  " + str(m) +'\n')
                    inp.write("'"+'\n')
                    inp.write("' mandatory data group"+'\n')
                    inp.write("' -------------------------------------------------"+'\n')
                    inp.write("ELEMENT PRINT"+'\n')
                    inp.write("'NSPEC"+'\n')
                    inp.write("3"+'\n')
                    inp.write("'IEL1    IEL2"+'\n')
                    inp.write("1         9"+'\n')
                    inp.write("10        30"+'\n')
                    inp.write("70        80"+'\n')
                    inp.write("' -------------------------------------------------"+'\n')
                    inp.write("FE SYSTEM DATA TEST PRINT"+'\n')
                    inp.write("'IFSPRI 1/2"+'\n')
                    inp.write("1"+'\n')
                    inp.write("'2"+'\n')
                    inp.write("' -------------------------------------------------"+'\n')
                    inp.write("FE ANALYSIS PARAMETERS"+'\n')
                    inp.write("'"+'\n')
                    inp.write("'  finite element analysis parameters"+'\n')
                    inp.write("'"+'\n')
                    inp.write("'TOLNOR  MAXIT"+'\n')
                    inp.write("1.E-07  30"+'\n')
                    inp.write("'DSINC,DSMIN,DSMAX,"+'\n')
                    inp.write("0.01  0.001  0.1"+'\n')
                    inp.write("'3.0  0.01 10."+'\n')
                    inp.write("'5.  0.1 10."+'\n')
                    inp.write("'"+'\n')
                    inp.write("'----------------------------------------------"+'\n')
                    inp.write("CURVATURE RANGE"+'\n')
                    inp.write("'----------------------------------------------"+'\n')
                    inp.write("'CURMAX  - Maximum curvature"+'\n')
                    inp.write("'NCURV   - Number of curvature levels"+'\n')
                    inp.write("'"+'\n')
                    inp.write("'CURMAX (1/m),NCURV"+'\n')
                    inp.write("'0.5       30"+'\n')
                    inp.write("0.2       100"+'\n')
                    inp.write("'---------------------------------------------------"+'\n')
                    inp.write("FORCE"+'\n')
                    inp.write("'Relang  tension"+'\n')
                    inp.write("'(deg)   (kN)"+'\n')
                    inp.write(str(i) + '\n')
                    inp.write("'8.00   400.0"+'\n')
                    inp.write("'16.5   500.0"+'\n')
                    inp.write("'19.0   550.0"+'\n')
                    inp.write("'19.1   600.0"+'\n')
                    inp.write("'18.6   650.0"+'\n')
                    inp.write("'17.5   700.0"+'\n')
                    inp.write("'14.0   775.0"+'\n')
                    inp.write("'"+'\n')
                    inp.write("'----------------------------------------------------"+'\n')
                    inp.write("MATERIAL DATA"+'\n')
                    inp.write("'----------------------------------------------------"+'\n')
                    inp.write("' Material identifier"+'\n')
                    inp.write("60D_15"+'\n')
                    inp.write("'NMAT - Number of points in stress/strain table for BS material"+'\n')
                    inp.write("21"+'\n')
                    inp.write("' strain   stress (kPa)    - Nmat input lines"+'\n')
                    inp.write("0.0 0.0"+'\n')
                    inp.write("0.005   1.40E+03"+'\n')
                    inp.write("0.010   2.57E+03"+'\n')
                    inp.write("0.015   3.61E+03"+'\n')
                    inp.write("0.020   4.55E+03"+'\n')
                    inp.write("0.025   5.36E+03"+'\n')
                    inp.write("0.030   6.03E+03"+'\n')
                    inp.write("0.035   6.59E+03"+'\n')
                    inp.write("0.040   7.02E+03"+'\n')
                    inp.write("0.045   7.37E+03"+'\n')
                    inp.write("0.050   7.67E+03"+'\n')
                    inp.write("0.055   7.92E+03"+'\n')
                    inp.write("0.060   8.13E+03"+'\n')
                    inp.write("0.065   8.31E+03"+'\n')
                    inp.write("0.070   8.47E+03"+'\n')
                    inp.write("0.075   8.61E+03"+'\n')
                    inp.write("0.080   8.74E+03"+'\n')
                    inp.write("0.085   8.86E+03"+'\n')
                    inp.write("0.090   8.96E+03"+'\n')
                    inp.write("0.095   9.06E+03"+'\n')
                    inp.write("0.100   9.10E+03"+'\n')
                    inp.write("'"+'\n')
                    inp.write("MATERIAL DATA"+'\n')
                    inp.write("' Material identifier"+'\n')
                    inp.write("60D_30"+'\n')
                    inp.write("'NMAT - Number of points in stress/strain table for BS material"+'\n')
                    inp.write("21"+'\n')
                    inp.write("' strain   stress (kPa)    - Nmat input lines"+'\n')
                    inp.write("0.000   0.0"+'\n')
                    inp.write("0.005   1100.0"+'\n')
                    inp.write("0.010   2060.0"+'\n')
                    inp.write("0.015   2910.0"+'\n')
                    inp.write("0.020   3690.0"+'\n')
                    inp.write("0.025   4370.0"+'\n')
                    inp.write("0.030   4950.0"+'\n')
                    inp.write("0.035   5420.0"+'\n')
                    inp.write("0.040   5810.0"+'\n')
                    inp.write("0.045   6120.0"+'\n')
                    inp.write("0.050   6400.0"+'\n')
                    inp.write("0.055   6640.0"+'\n')
                    inp.write("0.060   6840.0"+'\n')
                    inp.write("0.065   7030.0"+'\n')
                    inp.write("0.070   7180.0"+'\n')
                    inp.write("0.075   7330.0"+'\n')
                    inp.write("0.080   7470.0"+'\n')
                    inp.write("0.085   7590.0"+'\n')
                    inp.write("0.090   7710.0"+'\n')
                    inp.write("0.095   7810.0"+'\n')
                    inp.write("0.100   7920.0"+'\n')
                    inp.write("'"+'\n')
                    inp.write("'EXPORT MATERIAL DATA"+'\n')
                    inp.write("'--------------------------------------------------"+'\n')
                    inp.write("' IMEX   = 1 : tabular  =2 riflex format"+'\n')
                    inp.write("'  1"+'\n')
                    inp.write("'---------------------------------------------"+'\n')
                    inp.write("end"+'\n')
                    inp.write("'mandatory data group"+'\n')
                    inp.write("'---------------------------------------------"+'\n')

    inp.close()
    inp1.close()

def cl_od_to_array(min_cl, max_cl, min_od, max_od, incrementSize_fieldLength, incrementSize_fieldWidth):
    cl = min_cl
    od = min_od
    cl_array = []
    od_array = []
    while cl <= max_cl:
        cl_array.append(round(cl, 3))
        cl += incrementSize_fieldLength
    while od <= max_od:
        od_array.append(round(od, 3))
        od += incrementSize_fieldWidth

    print(cl_array)
    print(od_array)

    return cl_array, od_array
    

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

        # Display riser response
        riser_response = data.get("riser_response", {})
        self.display_riser_response(scrollable_frame, riser_response)

        # Display BS dimensions
        bs_dimensions = data.get("bs_dimension", {})
        self.display_section(scrollable_frame, "BS Dimensions", bs_dimensions)

        # Display BS materials
        bs_materials = data.get("bs_material", {})
        self.display_bs_materials(scrollable_frame, bs_materials)

        # Add a return button
        btn_return = tk.Button(scrollable_frame, text="Return", width=10, height=2, bg="#333333", fg="white", command=lambda: switch_frame(prev_frame))
        btn_return.pack(pady=20)

        btn_run_analysis = tk.Button(scrollable_frame, text="Run Analysis", width=10, height=2, bg="#333333", fg="white", command=lambda: generate_case_files())
        self.abnormal_max_curve, self.normal_max_curve = self.interpolate_max_curve(riser_capacities, riser_response)

        print(bs_dimensions.get("Input MIN overall length"))

        self.length = riser_info.get("Riser Length")
        self.EA = riser_info.get("Axial Stiffness")
        self.EI = riser_info.get("Bending Stiffness")
        self.GT = riser_info.get("Torsial Stiffness")
        self.m = riser_info.get("Mass Per Unit Length")
        self.cases = [0, 8, 16.5, 19, 19.1, 18.6, 17.5, 14]
        self.ID = 1
        self.SL = 10
        self.CL, self.OD = cl_od_to_array(bs_dimensions.get("Input MIN overall length"), 
                                          bs_dimensions.get("Input MAX overall length"), 
                                          bs_dimensions.get("Input MIN root OD"), 
                                          bs_dimensions.get("Input MAX root OD"), 
                                          bs_dimensions.get("Increment Length"), 
                                          bs_dimensions.get("Increment Width"))
        self.TL = bs_dimensions.get("Input tip length")
        self.TOD = 0.15
        self.MAT = ['Steel', 'Titanium']
        self.MATID = [101, 102]

        print("Length: ", self.length)
        print("EA: ", self.EA)
        print("EI: ", self.EI)
        print("GT: ", self.GT)
        print("m: ", self.m)
        print("Cases: ", self.cases)
        print("ID: ", self.ID)
        print("SL: ", self.SL)
        print("CL: ", self.CL)
        print("OD: ", self.OD)
        print("TL: ", self.TL)
        print("TOD: ", self.TOD)
        print("MAT: ", self.MAT)
        print("MATID: ", self.MATID)

    def display_section(self, parent, title, section_data):
        lbl_section_title = tk.Label(parent, text=title, font=("Arial", 12, "bold"), anchor="w")
        lbl_section_title.pack(fill="x", padx=20, pady=5)
        for key, value in section_data.items():
            lbl = tk.Label(parent, text=f"{key}: {value}", anchor="w")
            lbl.pack(fill="x", padx=40, pady=5)

    def display_riser_capacities(self, parent, capacities):
        lbl_section_title = tk.Label(parent, text="Riser capacities", font=("Arial", 12, "bold"), anchor="w")
        lbl_section_title.pack(fill="x", padx=20, pady=5)
        for key, values in capacities.items():
            lbl_key = tk.Label(parent, text=f"{key.capitalize()} Operation:", font=("Arial", 10, "bold"), anchor="w")
            lbl_key.pack(fill="x", padx=40, pady=5)
            for angle, tension in zip(values[0], values[1]):
                lbl_value = tk.Label(parent, text=f"Angle: {angle}, Tension: {tension}", anchor="w")
                lbl_value.pack(fill="x", padx=60, pady=2)

    def display_riser_response(self, parent, response):
        lbl_section_title = tk.Label(parent, text="Riser Response", font=("Arial", 12, "bold"), anchor="w")
        lbl_section_title.pack(fill="x", padx=20, pady=5)
        for key, values in response.items():
            lbl_key = tk.Label(parent, text=f"{key.capitalize()} Operation:", font=("Arial", 10, "bold"), anchor="w")
            lbl_key.pack(fill="x", padx=40, pady=5)
            for angle, tension in zip(values[0], values[1]):
                lbl_value = tk.Label(parent, text=f"Angle: {angle}, Tension: {tension}", anchor="w")
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

    def interpolate_max_curve(self, capacities, response):
        normal_capacities = capacities.get("normal", [[], []])
        abnormal_capacities = capacities.get("abnormal", [[], []])

        normal_response_tensions = response.get("normal", [[], []])
        abnormal_response_tensions = response.get("abnormal", [[], []])

        normal_response_tensions = normal_response_tensions[1]
        abnormal_response_tensions = abnormal_response_tensions[1]

        tension_normal = normal_capacities[1]
        tension_abnormal = abnormal_capacities[1]
        curvature_normal = normal_capacities[0] 
        curvature_abnormal = abnormal_capacities[0]

        f_linear_normal = interp1d(tension_normal, curvature_normal, kind='linear')
        f_linear_abnormal = interp1d(tension_abnormal, curvature_abnormal, kind='linear')

        # Using linear interpolation
        new_curvatures_linear_normal = f_linear_normal(normal_response_tensions)
        new_curvatures_linear_abnormal = f_linear_abnormal(abnormal_response_tensions)

        return new_curvatures_linear_normal, new_curvatures_linear_abnormal


def main():
    root = tk.Tk()
    prev_frame = tk.Frame(root)
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
        'Input clearance:': '10',
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
    app = AnalysisScreen(root, prev_frame, data)
    root.mainloop()

if __name__ == "__main__":
    main()