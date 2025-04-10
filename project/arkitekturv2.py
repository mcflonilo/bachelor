import tkinter as tk
from dataclasses import dataclass, field
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from tkinter import ttk
from tkinter import filedialog
import json
import os
import re
import subprocess
import sys
import pandas as pd
from scipy.interpolate import interp1d
import threading
import time
import numpy as np
from tkinter import messagebox, Toplevel, ttk, Canvas, Frame, Scrollbar
from queue import Queue
import logging
from collections import defaultdict


logging.basicConfig(filename="bs_analysis.log", level=logging.INFO, format="%(asctime)s - %(message)s")

# Singleton DataStore class
@dataclass
class DataStore:
    parameters: dict = field(default_factory=dict)
    results: dict = field(default_factory=dict)

    _instance = None
    
    def __new__(cls, *args, **kwargs):
        if not cls._instance:
            cls._instance = super(DataStore, cls).__new__(cls)
        return cls._instance
    def set_parameter(self, key, value):
        self.parameters[key] = value
    def get_parameter(self, key):
        return self.parameters.get(key, "Not set")
    def print_parameters(self):
        print(self.parameters)
    def save_data(self):
        """Save stored data to a JSON file."""
        project_name = self.parameters.get("project_info", {}).get("project_name", "default_project")
        default_filename = f"{project_name}_saved_data.json"

        filename = filedialog.asksaveasfilename(
            initialfile=default_filename,
            defaultextension=".json",
            filetypes=[("JSON files", "*.json"), ("All files", "*.*")],
            title="Save file"
        )

        
        with open(filename, "w") as file:
            json.dump(self.parameters, file, indent=4)
        print(f"Data saved to {filename}")

    def load_data(self):
        """Load data from a JSON file into the datastore."""
        filename = filedialog.askopenfilename(
            title="Select file",
            filetypes=(("JSON files", "*.json"), ("All files", "*.*"))
        )

        if filename:
            with open(filename, "r") as file:
                data = json.load(file)

            # Update the stored parameters with loaded data
            for key, value in data.items():
                self.set_parameter(key, value)

            print(f"Data loaded from {filename}")

@dataclass
class DataProcessor:
    _instance = None
    data = DataStore()
    def __new__(cls, *args, **kwargs):
        if not cls._instance:
            cls._instance = super(DataProcessor, cls).__new__(cls)
        return cls._instance
    def print_data(self, groups):
        for case_group, group in groups:
            print(f"Case Group: {case_group}")
            print(group)
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
        thresholds_normal = {}
        thresholds_abnormal = {}
        for i, value in enumerate(new_curvatures_linear_normal):
            thresholds_normal[f"case_files\\Case-normal{i+1}"] = {"maximum_curvature": value}
        for i, value in enumerate(new_curvatures_linear_abnormal):
            thresholds_abnormal[f"case_files\\Case-abnormal{i+1}"] = {"maximum_curvature": value}

        self.data.set_parameter("thresholds_normal", thresholds_normal)
        self.data.set_parameter("thresholds_abnormal", thresholds_abnormal)

        return new_curvatures_linear_normal, new_curvatures_linear_abnormal

    def cl_od_to_array(self, min_cl, max_cl, min_od, max_od, incrementSize_fieldLength, incrementSize_fieldWidth):
        cl = min_cl
        od = min_od
        cl_array = []
        od_array = []
        while cl <= max_cl:
            cl_array.append(round(cl, 3))
            cl += incrementSize_fieldLength
        if cl_array[-1] < max_cl:
            cl_array.append(round(max_cl, 3))
        while od <= max_od:
            od_array.append(round(od, 3))
            od += incrementSize_fieldWidth
        if od_array[-1] < max_od:
            od_array.append(round(max_od, 3))


        return cl_array, od_array
    
    def make_case_arrays(self, riser_response):
            normal_cases = []
            abnormal_cases = []
            for key, values in riser_response.items():
                for angle, tension in zip(values[0], values[1]):
                    if key == "normal":
                        normal_cases.append((angle, tension))
                    elif key == "abnormal":
                        abnormal_cases.append((angle, tension))
            return normal_cases, abnormal_cases
    
    def casesBtn(self):
        length = round(float(self.data.get_parameter("riser_info")["riser length"]), 3)
        EA = round(float(self.data.get_parameter("riser_info")["axial stiffness"]), 3)
        EI = round(float(self.data.get_parameter("riser_info")["bending stiffness"]), 3)
        GT = round(float(self.data.get_parameter("riser_info")["torsial stiffness"]), 3)
        m = round(float(self.data.get_parameter("riser_info")["mass per unit length"]), 3)
        ID = round(float(self.data.get_parameter("riser_info")["outer diameter"]) + (2 * (float(self.data.get_parameter("riser_info")["outer diameter tolerance"]) + float(self.data.get_parameter("bs_dimension")["clearance"]))), 3)
        SL = round(0.700, 3)
        CL, OD = self.cl_od_to_array(
            round(float(self.data.get_parameter("bs_dimension")["min overall length"]), 3), 
            round(float(self.data.get_parameter("bs_dimension")["max overall length"]), 3), 
            round(float(self.data.get_parameter("bs_dimension")["min root outer diameter"]), 3), 
            round(float(self.data.get_parameter("bs_dimension")["max root outer diameter"]), 3), 
            round(float(self.data.get_parameter("bs_dimension")["increment length"]), 3), 
            round(float(self.data.get_parameter("bs_dimension")["increment width"]), 3)
        )
        TL = round(float(self.data.get_parameter("bs_dimension")["tip length"]), 3)
        TOD = round(ID + 0.04, 3)  # DETTE ER IKKE SIKKERT OG MAN MÅ SPØRRE KNUT IVAR
        MAT = ["NOLIN 60D_30"]  # PLACEHOLDER TIL JEG VET BEDRE
        MATID = ["60D_30"]  # PLACEHOLDER TIL JEG VET BEDRE
        normal_cases, abnormal_cases = self.make_case_arrays(self.data.get_parameter("riser_response"))

        casetotal = 0
        casetotal += self.generate_case_files(length, EA, EI, GT, m, normal_cases, ID, SL, OD, CL, TL, TOD, MAT, MATID, "normal")
        casetotal += self.generate_case_files(length, EA, EI, GT, m, abnormal_cases, ID, SL, OD, CL, TL, TOD, MAT, MATID, "abnormal")
        tk.messagebox.showinfo("Case Files", f"generated {casetotal} cases.")

    def generate_case_files_multi_BS_btn(self):
        length = round(float(self.data.get_parameter("riser_info")["riser length"]), 3)
        EA = round(float(self.data.get_parameter("riser_info")["axial stiffness"]), 3)
        EI = round(float(self.data.get_parameter("riser_info")["bending stiffness"]), 3)
        GT = round(float(self.data.get_parameter("riser_info")["torsial stiffness"]), 3)
        m = round(float(self.data.get_parameter("riser_info")["mass per unit length"]), 3)
        normal_cases, abnormal_cases = self.make_case_arrays(self.data.get_parameter("riser_response"))
        MAT = ["NOLIN 60D_30"]  # PLACEHOLDER TIL JEG VET BEDRE
        MATID = ["60D_30"]  # PLACEHOLDER TIL JEG VET BEDRE
        bs_dimension_multi = self.data.get_parameter("bs_dimension_multi")

        casetotal = 0
        casetotal +=self.generate_case_files_multi_BS(length,EA,EI,GT,m,normal_cases,bs_dimension_multi,MAT,MATID,"normal", "case_files")
        casetotal +=self.generate_case_files_multi_BS(length,EA,EI,GT,m,abnormal_cases,bs_dimension_multi,MAT,MATID,"abnormal", "case_files")
        tk.messagebox.showinfo("Case Files", f"generated {casetotal} cases.")

    def generate_case_files_multi_BS(self, length, EA, EI, GT, m, cases, bs_dimension_multi, MAT, MATID, label, output_dir):
        """Generates case files for each case in `cases` for all BS dimensions in `bs_dimension_multi`."""

        # Create the case list file
        case_list_filename = os.path.join(output_dir, f'bsengine-cases-{label}.txt')
        with open(case_list_filename, 'w') as case_list_file:
            total_cases = 0  # Track case count

            for bs in bs_dimension_multi:  # Loop through each BS configuration
                ID = round(float(bs["ID"]), 3)
                OD = round(float(bs["Root Outer Diameter"]), 3)
                CL = round(float(bs["Cone Length"]), 3)
                TL = round(float(bs["Tip Length"]), 3)
                TOD = round(float(bs["Tip Outer Diameter"]), 3)

                for case_id, case_data in enumerate(cases):
                    case_filename = f"Case-{label}-{case_id+1}-ID{ID}-OD{OD}-CL{CL}-TL{TL}-TOD{TOD}.inp"
                    case_filepath = os.path.join(output_dir, case_filename)

                    # Write case file
                    with open(case_filepath, 'w') as inp:
                        inp.write('BEND STIFFENER DATA\n')
                        inp.write("' ID   NSEG\n")
                        inp.write("' inner diameter      Number of linear segments\n")
                        inp.write(f"{ID}   3\n")
                        inp.write("' LENGTH   NEL   OD1    OD2  DENSITY LIN/NOLIN        EMOD/MAT-ID\n")
                        inp.write(f"0.7  50  {OD}  {OD}  2000  LIN  10000.E06\n")
                        inp.write(f"{CL}  100  {OD}  {TOD}  1150  {MAT[0]}\n")
                        inp.write(f"{TL}  50  {TOD}  {TOD}  1150  {MAT[0]}\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("RISER DATA\n")
                        inp.write("'SRIS,  NEL   EI,    EA,      GT     Mass\n")
                        inp.write("' (m)         kNm^2  kN              kg/m\n")
                        inp.write(f"{length}  100  {EI}  {EA}  {GT}  {m}\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("ELEMENT PRINT\n")
                        inp.write("'NSPEC\n")
                        inp.write("3\n")
                        inp.write("'IEL1    IEL2\n")
                        inp.write("1         9\n")
                        inp.write("10        30\n")
                        inp.write("70        80\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("FE SYSTEM DATA TEST PRINT\n")
                        inp.write("'IFSPRI 1/2\n")
                        inp.write("1\n")
                        inp.write("'2\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("FE ANALYSIS PARAMETERS\n")
                        inp.write("'  finite element analysis parameters\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("'TOLNOR  MAXIT\n")
                        inp.write("1.E-07  30\n")
                        inp.write("'DSINC,DSMIN,DSMAX,\n")
                        inp.write("0.01  0.001  0.1\n")
                        inp.write("'3.0  0.01 10.\n")
                        inp.write("'5.  0.1 10.\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("CURVATURE RANGE\n")
                        inp.write("'CURMAX  - Maximum curvature\n")
                        inp.write("'NCURV   - Number of curvature levels\n")
                        inp.write("'CURMAX (1/m),NCURV\n")
                        inp.write("'0.5       30\n")
                        inp.write("0.2       100\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("FORCE\n")
                        inp.write("'Relang  tension\n")
                        inp.write("'(deg)   (kN)\n")
                        inp.write(f"{case_data[0]}   {case_data[1]}\n")
                        inp.write("'8.00   400.0\n")
                        inp.write("'16.5   500.0\n")
                        inp.write("'19.0   550.0\n")
                        inp.write("'19.1   600.0\n")
                        inp.write("'18.6   650.0\n")
                        inp.write("'17.5   700.0\n")
                        inp.write("'14.0   775.0\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("MATERIAL DATA\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("' Material identifier\n")
                        inp.write("60D_15\n")
                        inp.write("'NMAT - Number of points in stress/strain table for BS material\n")
                        inp.write("21\n")
                        inp.write("' strain   stress (kPa)    - Nmat input lines\n")
                        inp.write("0.0 0.0\n")
                        inp.write("0.005   1.40E+03\n")
                        inp.write("0.010   2.57E+03\n")
                        inp.write("0.015   3.61E+03\n")
                        inp.write("0.020   4.55E+03\n")
                        inp.write("0.025   5.36E+03\n")
                        inp.write("0.030   6.03E+03\n")
                        inp.write("0.035   6.59E+03\n")
                        inp.write("0.040   7.02E+03\n")
                        inp.write("0.045   7.37E+03\n")
                        inp.write("0.050   7.67E+03\n")
                        inp.write("0.055   7.92E+03\n")
                        inp.write("0.060   8.13E+03\n")
                        inp.write("0.065   8.31E+03\n")
                        inp.write("0.070   8.47E+03\n")
                        inp.write("0.075   8.61E+03\n")
                        inp.write("0.080   8.74E+03\n")
                        inp.write("0.085   8.86E+03\n")
                        inp.write("0.090   8.96E+03\n")
                        inp.write("0.095   9.06E+03\n")
                        inp.write("0.100   9.10E+03\n")
                        inp.write("'---------------------------------------------------\n")
                        inp.write("MATERIAL DATA\n")
                        inp.write("' Material identifier\n")
                        inp.write("60D_30\n")
                        inp.write("'NMAT - Number of points in stress/strain table for BS material\n")
                        inp.write("21\n")
                        inp.write("' strain   stress (kPa)    - Nmat input lines\n")
                        inp.write("0.000   0.0\n")
                        inp.write("0.005   1100.0\n")
                        inp.write("0.010   2060.0\n")
                        inp.write("0.015   2910.0\n")
                        inp.write("0.020   3690.0\n")
                        inp.write("0.025   4370.0\n")
                        inp.write("0.030   4950.0\n")
                        inp.write("0.035   5420.0\n")
                        inp.write("0.040   5810.0\n")
                        inp.write("0.045   6120.0\n")
                        inp.write("0.050   6400.0\n")
                        inp.write("0.055   6640.0\n")
                        inp.write("0.060   6840.0\n")
                        inp.write("0.065   7030.0\n")
                        inp.write("0.070   7180.0\n")
                        inp.write("0.075   7330.0\n")
                        inp.write("0.080   7470.0\n")
                        inp.write("0.085   7590.0\n")
                        inp.write("0.090   7710.0\n")
                        inp.write("0.095   7810.0\n")
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
                    # Append case filename to the case list file
                    case_list_file.write(case_filepath + '\n')
                    total_cases += 1

        return total_cases

    def generate_case_files(self, length, EA, EI, GT, m, cases, ID, SL, OD, CL, TL, TOD, MAT, MATID, label):
        # Create a text file to store the case filenames
        
        inp1 = open(f'bsengine-cases-{label}.txt', 'w')
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

                        inp = open(os.path.join(f'case_files', "Case-"+label + str(y) + '-' + str(j) + '-' + str(k) + '-' + str(q) + ".inp"), 'w')
                        inp1.write(os.path.join('case_files', "Case-"+label + str(y) + '-' + str(j) + '-' + str(k) + '-' + str(q) + ".inp") + '\n')

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
                        inp.write(str(i[0]) +"  "+ str(i[1]) + '\n')
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
        def count_lines(filename):
            with open(filename, 'r') as file:
                return sum(1 for line in file)

        inp.close()
        inp1.close()
        return count_lines(f'bsengine-cases-{label}.txt')

    def extract_key_results(self, case_file):
        try:
            with open(case_file, 'r') as res_file:
                res_lines = res_file.readlines()

            keyres1 = None
            keyres2 = None

            # Search for key results in the log file
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

        except FileNotFoundError:
            print(f"File not found: {case_file}")
            return None
        except Exception as e:
            print(f"Error processing {case_file}: {e}")
            return None

    def save_search_log(self, case_group, log_data):
        """Save the search log to a CSV file, ensuring correct formatting and headers."""
        file_name = "optimized_search_log.csv"

        # Create DataFrame with correct column names
        df = pd.DataFrame(
            log_data, 
            columns=["Case Group", "Width", "Length", "Case Name", "Curvature", "Status", "Reason"]
        )

        # Append to the file, ensuring the header is only written once
        df.to_csv(file_name, mode='a', header=not os.path.isfile(file_name), index=False)

        print(f"Search log saved to {file_name}")

    def redefine_group_as_2d_array(self, group):
        """Redefine the group as a 2D dictionary where width is the first key and length is the second key."""
        width_length_dict = {}
        for index, row in group.iterrows():
            width = round(row['width'], 5)  # Round to 5 decimal places
            length = round(row['length'], 5)
            if width not in width_length_dict:
                width_length_dict[width] = {}
            width_length_dict[width][length] = row
        return width_length_dict

    def runBSEngine(self, case):
            def resource_path(relative_path):
                """ Get the absolute path to the resource, works for dev and for PyInstaller """
                try:
                    # PyInstaller creates a temp folder and stores path in _MEIPASS
                    base_path = sys._MEIPASS
                except Exception:
                    base_path = os.path.abspath(".")

                return os.path.join(base_path, relative_path)

            def checkIfCaseIsAlreadyChecked(case):
                casename = case.replace('.inp', '_FEA.log')
                if os.path.exists(casename):
                    print(f"bs already checked for case: {casename}. skipping!")
                    return True
                return False
            
            executable_path = resource_path('bsengine\\bsengine')
            
            current_directory = os.getcwd()
            
            if checkIfCaseIsAlreadyChecked(case):
                case_file = case.strip().replace('.inp', '.log')
                result = self.extract_key_results(case_file)
                if result is None:
                    print(f"Error extracting key results for case: {case}")
                else:
                    result = float(result['maximum_curvature'])
                    return result
            print(f"Running case: {case}")
            process = subprocess.Popen(f"{executable_path} -b {case}", shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, cwd=current_directory)
            stdout, stderr = process.communicate()
            print(f"stdout: {stdout}")
            print(f"stderr: {stderr}")
            if process.returncode != 0:
                print(f"Error encountered with case: {case}. FIKS AT DEN KJØRES PÅ NYTT PÅ EN ELLER ANNEN MÅTE DITT NEK.")
            
            case_file = case.strip().replace('.inp', '.log')
            result = self.extract_key_results(case_file)
            result = float(result['maximum_curvature'])
            return result

    def find_shortest_valid_result(self, group, case_group, groups):
        """Find the shortest valid result for each case group and log the search path with detailed information."""
        case_group_data = groups.get_group(case_group)
        case_2d_array = self.redefine_group_as_2d_array(case_group_data)
        threshold = self.data.get_parameter("thresholds_normal")[case_group]['maximum_curvature']
        self.min_width = float(self.data.get_parameter("bs_dimension")["min root outer diameter"])
        self.max_width = float(self.data.get_parameter("bs_dimension")["max root outer diameter"])
        self.min_length = float(self.data.get_parameter("bs_dimension")["min overall length"])
        self.max_length = float(self.data.get_parameter("bs_dimension")["max overall length"])
        best_result = None
        current_width = self.min_width
        current_length = self.max_length
        self.width_increment = float(self.data.get_parameter("bs_dimension")["increment width"])
        self.length_increment = float(self.data.get_parameter("bs_dimension")["increment length"])

        # List to store checked values
        search_log = []

        def log_entry(width, length, case_name, curvature, status, reason):
            """Helper function to log an entry with full details."""
            search_log.append([case_group, width, length, case_name, curvature, status, reason])

        # Check top left corner
        case_name = case_2d_array[self.min_width][self.min_length]['case_name']
        curvature = self.runBSEngine(case_name)
        log_entry(self.min_width, self.min_length, case_name, curvature, 
                "Passed" if curvature <= threshold else "Failed", 
                "Top-left corner check")
        
        if curvature <= threshold:
            print("✅ First result is valid (top left). Case should be dismissed.")
            self.save_search_log(case_group, search_log)
            return pd.DataFrame()
        print("curvature: ", curvature, "threshold: ", threshold)

        # Check bottom left corner
        case_name = case_2d_array[self.max_width][self.min_length]['case_name']
        curvature = self.runBSEngine(case_name)
        log_entry(self.max_width, self.min_length, case_name, curvature, 
                "Passed" if curvature <= threshold else "Failed", 
                "Bottom-left corner check")

        if curvature <= threshold:
            print("✅ Max width, min length valid (bottom left). Case should be dismissed.")
            self.save_search_log(case_group, search_log)
            return pd.DataFrame()
        print("curvature: ", curvature, "threshold: ", threshold)

        # Check top right corner
        case_name = case_2d_array[self.min_width][self.max_length]['case_name']
        curvature = self.runBSEngine(case_name)
        log_entry(self.min_width, self.max_length, case_name, curvature, 
                "Passed" if curvature <= threshold else "Failed", 
                "Top-right corner check")

        if curvature <= threshold:
            print("✅ Min width, max length valid. (top right) Case should be dismissed.")
            self.save_search_log(case_group, search_log)
            return pd.DataFrame()
        
        print("curvature: ", curvature, "threshold: ", threshold)

        # Iterative search for best result
        while current_width <= self.max_width and current_length >= self.min_length:
            rounded_width = round(current_width, 5)
            rounded_length = round(current_length, 5)

            if rounded_width in case_2d_array and rounded_length in case_2d_array[rounded_width]:
                case_name = case_2d_array[rounded_width][rounded_length]["case_name"]
                curvature = self.runBSEngine(case_name)
                print("curvature: ", curvature, "threshold: ", threshold)
                
                status = "Passed" if curvature <= threshold else "Failed"
                reason = "Curvature within threshold" if status == "Passed" else "Curvature exceeded threshold"
                log_entry(rounded_width, rounded_length, case_name, curvature, status, reason)

                if curvature <= threshold:
                    best_result = case_2d_array[rounded_width][rounded_length]
                    current_length -= self.length_increment
                else:
                    current_width += self.width_increment
            else:
                log_entry(rounded_width, rounded_length, "N/A", "N/A", "Skipped", "No valid case at this width/length")
                current_width += self.width_increment

        self.save_search_log(case_group, search_log)
        return best_result

    def loadBSCases(self, file_path):
        self.interpolate_max_curve(self.data.get_parameter("riser_capacities"), self.data.get_parameter("riser_response"))
        findShortestBSForCase = self.findShortestBSForCase(file_path)
        self.checkBSAgainsAllCases(findShortestBSForCase, "abnormal", 1)
        print("verifying result")
        self.checkBSAgainsAllCases(findShortestBSForCase, "normal", 1)
        self.data.set_parameter("shortest_valid_result", findShortestBSForCase)
    
    def findShortestBSForCase(self,file_path):
        """Loads all the bsengine cases, extracts case group, width, and length from the case name."""
        cases = []
        with open(file_path, 'r') as file:
            cases = file.readlines()
        data = []
        for case in cases:
            case_name = case.strip()
            # Extract the base filename without path
            case_filename = os.path.basename(case_name)
            # Removing the file extension and splitting by '-'
            parts = case_filename.replace('.inp', '').split('-')
            if len(parts) < 4:
                print(f"Skipping invalid case format: {case_name}")
                continue
            # Extracting case group (e.g., "case_files\Case-normal1")
            case_group = os.path.join(os.path.dirname(case_name), f"{parts[0]}-{parts[1]}")
            try:
                width = float(parts[2])
                length = float(parts[3])
            except ValueError:
                print(f"Skipping case due to invalid width/length: {case_name}")
                continue
            data.append({"case_name": case_name, "case_group": case_group, "width": width, "length": length})
        # Convert to DataFrame
        df = pd.DataFrame(data)
        # Separate the data into groups based on the extracted case name
        groups = df.groupby('case_group')

        shortest_valid_result_array = []
        check = False
        i = 1
        while not check:
            shortest = self.find_shortest_valid_result(groups.get_group(f"case_files\\Case-normal{i}"), f"case_files\\Case-normal{i}", groups)
            if shortest.empty:
                i += 1
                continue

            shortest_valid_result_array.append(shortest)
            print(f"Shortest valid result for {f'case_files\\Case-normal{i}'}: {shortest}\n")
            check_result, i = self.checkBSAgainsAllCases(shortest, "normal",i)
            print("checking bs against all cases")
            if check_result:
                return shortest

    def checkBSAgainsAllCases(self, shortest,tag, i):
        if tag == "normal":
            thresholds = self.data.get_parameter("thresholds_normal")
        else:
            thresholds = self.data.get_parameter("thresholds_abnormal")
        for case in thresholds.keys():
            case = f"case_files\\Case-{tag}{i}-{shortest['width']}-{shortest['length']}-60D_30.inp"
            result = self.runBSEngine(case)
            if result <= thresholds[f"case_files\\Case-{tag}{i}"]['maximum_curvature']:
                print(f"bs valid for {tag} case{i}")
            else:
                print(f"bs invalid for {tag} case{i}")
                return False, i
            if i >= thresholds.__len__():
                return True, i
            i += 1
        return True, None

    def multithreadedAnalysis(self):
        def runBSEngineMultiThread(case, case_queue):
            def resource_path(relative_path):
                """ Get the absolute path to the resource, works for dev and for PyInstaller """
                try:
                    # PyInstaller creates a temp folder and stores path in _MEIPASS
                    base_path = sys._MEIPASS
                except Exception:
                    base_path = os.path.abspath(".")

                return os.path.join(base_path, relative_path)
            def checkIfCaseIsAlreadyChecked(case):
                casename = case.replace('.inp', '_FEA.log')
                if os.path.exists(casename):
                    print(f"bs already checked for case: {casename}. skipping!")
                    return True
                return False
            
            executable_path = resource_path('bsengine\\bsengine')
            current_directory = os.getcwd()

            if checkIfCaseIsAlreadyChecked(case):
                case_file = case.strip().replace('.inp', '.log')
                result = self.extract_key_results(case_file)
                if result is None:
                    print(f"Error extracting key results for case: {case}")
                else:
                    result = float(result['maximum_curvature'])
                    return result
            
            print(f"Running case: {case}")
            process = subprocess.Popen(f"{executable_path} -b {case}", shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, cwd=current_directory)
            stdout, stderr = process.communicate()
            print(f"stdout: {stdout}")
            print(f"stderr: {stderr}")
            if process.returncode != 0:
                print(f"Error encountered with case: {case}. Re-queuing the case.")
                case_queue.put(case)
            case_file = case.strip().replace('.inp', '.log')
            result = self.extract_key_results(case_file)
            result = float(result['maximum_curvature'])
            return result

        def createCaseQueue():
            cases = open('bsengine-cases-normal.txt', 'r').readlines()
            cases += open('bsengine-cases-abnormal.txt', 'r').readlines()
            cases = [case.strip() for case in cases]
            case_queue = Queue()
            for case in cases:
                case_queue.put(case)
            return case_queue

        def run_threads():
            def checkIfCaseIsAlreadyChecked(case):
                casename = case.replace('.inp', '_FEA.log')
                if os.path.exists(casename):
                    print(f"bs already checked for case: {casename}. skipping!")
                    return True
                return False
            case_queue = Queue()
            case_queue = createCaseQueue()
            print(f"Number of cases: {case_queue.qsize()}.")
            max_threads = os.cpu_count() - 1
            print(f"Max threads: {max_threads}.")
            threads = []

            while not case_queue.empty():
                while threading.active_count() > max_threads:
                    time.sleep(1)
                case = case_queue.get()

                if checkIfCaseIsAlreadyChecked(case):
                    continue

                print(f"Running case: {case}.")
                thread = threading.Thread(target=runBSEngineMultiThread, args=(case,case_queue))
                time.sleep(1)
                thread.start()
                threads.append(thread)
                print(f"Active threads: {threading.active_count()}.")

            for thread in threads:
                thread.join()
            print("All cases have been processed.")

        def check_results():
            """Checks the results of the analysis and determines which BS configurations pass all cases."""

            # Step 1: Load Data
            self.interpolate_max_curve(self.data.get_parameter("riser_capacities"), self.data.get_parameter("riser_response"))

            # Step 2: Read Normal & Abnormal Case Filenames
            normal_cases = open('bsengine-cases-normal.txt', 'r').readlines()
            abnormal_cases = open('bsengine-cases-abnormal.txt', 'r').readlines()

            # Convert filenames to corresponding log files (ONLY `.log`, not `_FEA.log`)
            normal_logs = [case.strip().replace('.inp', '.log') for case in normal_cases]
            abnormal_logs = [case.strip().replace('.inp', '.log') for case in abnormal_cases]

            # Step 3: Extract Results from Log Files
            normal_results = {log: self.extract_key_results(log) for log in normal_logs}
            abnormal_results = {log: self.extract_key_results(log) for log in abnormal_logs}

            # Step 4: Load Thresholds from DataStore
            thresholds_normal = self.data.get_parameter("thresholds_normal")
            thresholds_abnormal = self.data.get_parameter("thresholds_abnormal")

            bs_cases = defaultdict(lambda: {"normal": [], "abnormal": []})  # Store results grouped by BS

            # Step 5: Process Results - Group Cases by BS Configuration
            for case_name in normal_cases + abnormal_cases:
                case_name = case_name.strip()

                # **Extract BS Parameters from Case Name using Regex**
                match = re.search(r'Case-(normal|abnormal)-(\d+)-ID([\d.]+)-OD([\d.]+)-CL([\d.]+)-TL([\d.]+)-TOD([\d.]+)', case_name)
                if not match:
                    logging.warning(f"⚠️ Unable to extract BS parameters from: {case_name}. Skipping...")
                    continue

                case_type, case_number, bs_id, od, cl, tl, tod = match.groups()
                case_number = int(case_number)  # Convert to integer
                bs_key = f"ID{bs_id}-OD{od}-CL{cl}-TL{tl}-TOD{tod}"  # Unique BS Identifier

                if case_type == "normal":
                    bs_cases[bs_key]["normal"].append(case_number)
                else:
                    bs_cases[bs_key]["abnormal"].append(case_number)

            successful_bs = []

            # Step 6: Check Each BS Against All Cases
            for bs_key, cases in bs_cases.items():
                normal_passed = True
                abnormal_passed = True

                # **Check Normal Cases**
                for case_num in cases["normal"]:
                    log_file = f"case_files\\Case-normal-{case_num}-{bs_key}log"
                    norm_result = normal_results.get(log_file, None)

                    if norm_result is None:
                        logging.warning(f"Skipping {bs_key} due to missing normal case result: {log_file}")
                        normal_passed = False
                        break


                    print(thresholds_normal)

                    norm_curvature = float(norm_result["maximum_curvature"])
                    threshold_norm = thresholds_normal.get(f"case_files\\Case-normal{case_num}")
                    print(threshold_norm)
                    print(threshold_norm["maximum_curvature"])

                    if threshold_norm is None:
                        logging.warning(f"Missing normal threshold for case {case_num}. Skipping BS: {bs_key}.")
                        normal_passed = False
                        break

                    if norm_curvature > threshold_norm["maximum_curvature"]:
                        logging.info(f"❌ BS {bs_key} FAILED normal case {case_num}. Threshold: {threshold_norm["maximum_curvature"]}, Achieved: {norm_curvature}")
                        normal_passed = False
                        break  # Stop checking if one case fails

                # **Check Abnormal Cases (only if normal passed)**
                if normal_passed:
                    for case_num in cases["abnormal"]:
                        log_file = f"case_files\\Case-abnormal-{case_num}-{bs_key}log"
                        abnorm_result = abnormal_results.get(log_file, None)

                        if abnorm_result is None:
                            logging.warning(f"Skipping {bs_key} due to missing abnormal case result: {log_file}")
                            abnormal_passed = False
                            break

                        abnorm_curvature = float(abnorm_result["maximum_curvature"])
                        threshold_abnorm = thresholds_abnormal.get(f"case_files\\Case-abnormal{case_num}")

                        if threshold_abnorm is None:
                            logging.warning(f"Missing abnormal threshold for case {case_num}. Skipping BS: {bs_key}.")
                            abnormal_passed = False
                            break

                        if abnorm_curvature > threshold_abnorm["maximum_curvature"]:
                            logging.info(f"❌ BS {bs_key} FAILED abnormal case {case_num}. Threshold: {threshold_abnorm["maximum_curvature"]}, Achieved: {abnorm_curvature}")
                            abnormal_passed = False
                            break  # Stop checking if one case fails

                # If both normal & abnormal cases pass, BS is successful
                if normal_passed and abnormal_passed:
                    successful_bs.append(bs_key)
                    logging.info(f"✅ BS {bs_key} PASSED all cases.")

            # Step 7: Display Results
            if successful_bs:
                logging.info(f"✅ Successful BS Configurations: {', '.join(successful_bs)}")
                print(f"✅ Successful BS Configurations: {', '.join(successful_bs)}")
            else:
                logging.info("❌ No BS configurations met the required criteria.")
                print("❌ No BS configurations met the required criteria.")

            return successful_bs




            

        run_threads()
        check_results()


    

# Base Frame for all screens
class BaseFrame(tk.Frame):
    def __init__(self, parent, controller):
        super().__init__(parent, bg="#E3DFCF")
        self.controller = controller
        self.data_store = DataStore()  # All frames share this instance

    def switch_to(self, frame_name):
        self.controller.show_frame(frame_name)

# Navigation Frame - design optimal BS frame
class FindOptimalBsNavFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        
        tk.Label(self, text="Navigation").pack()
        
        tk.Button(self, text="Project Info", command=lambda: self.switch_to("ProjectInfoFrame")).pack()
        tk.Button(self, text="Riser Info", command=lambda: self.switch_to("RiserInfoFrame")).pack()
        tk.Button(self, text="BS Dimensions", command=lambda: self.switch_to("BSDimensionsFrame")).pack()
        tk.Button(self, text="Riser Response", command=lambda: self.switch_to("RiserResponseFrame")).pack()
        tk.Button(self, text="Riser Capacities", command=lambda: self.switch_to("RiserCapacitiesFrame")).pack()
        tk.Button(self, text="Run Analysis", command=lambda: self.switch_to("RunAnalysisFrame")).pack()
        tk.Button(self, text="BS Material", command=lambda: self.switch_to("SelectMaterialFrame")).pack()

        tk.Button(self, text="Back", command=lambda: self.switch_to("NavigationFrame")).pack()

class CheckExistingBSNavFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        
        tk.Label(self, text="Navigation").pack()
        
        tk.Button(self, text="Project Info", command=lambda: self.switch_to("ProjectInfoFrame")).pack()
        tk.Button(self, text="Riser Info", command=lambda: self.switch_to("RiserInfoFrame")).pack()
        tk.Button(self, text="Riser Response", command=lambda: self.switch_to("RiserResponseFrame")).pack()
        tk.Button(self, text="Riser Capacities", command=lambda: self.switch_to("RiserCapacitiesFrame")).pack()
        tk.Button(self, text="BS designs", command=lambda: self.switch_to("BSDimensionsFrameMulti")).pack()
        tk.Button(self, text="Run Analysis", command=lambda: self.switch_to("RunAnalysisFrame")).pack()

        tk.Button(self, text="Back", command=lambda: self.switch_to("NavigationFrame")).pack() 

class RunLoadCaseOnBSNavFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        
        tk.Label(self, text="Navigation").pack()
        
        tk.Button(self, text="Project Info", command=lambda: self.switch_to("ProjectInfoFrame")).pack()
        tk.Button(self, text="Riser Info", command=lambda: self.switch_to("RiserInfoFrame")).pack()
        tk.Button(self, text="BS Dimensions", command=lambda: self.switch_to("BSDimensionsFrame")).pack()
        tk.Button(self, text="Riser Response", command=lambda: self.switch_to("RiserResponseFrame")).pack()
        tk.Button(self, text="Riser Capacities", command=lambda: self.switch_to("RiserCapacitiesFrame")).pack()
        tk.Button(self, text="Run Analysis", command=lambda: self.switch_to("RunAnalysisFrame")).pack()

        # back button
        tk.Button(self, text="Back", command=lambda: self.switch_to("NavigationFrame")).pack()

# start frame
class NavigationFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        
        tk.Label(self, text="Navigation", bg="#ECECEC", font=("Arial", 14, "bold")).pack(pady=(10, 20))

        button_style = {
            "bg": "#E3C376",         # soft gold
            "fg": "black",           # text color
            "width": 30,
            "height": 2,
            "bd": 1,
            "relief": "solid",
            "highlightbackground": "black",
            "font": ("Arial", 10, "bold"),
            "activebackground": "#d2b660"
        }

        tk.Button(self, text="Design Optimal BS", command=lambda: self.switch_to("FindOptimalBsNavFrame"), **button_style).pack(pady=5)
        tk.Button(self, text="Check Existing BS Designs", command=lambda: self.switch_to("CheckExistingBSNavFrame"), **button_style).pack(pady=5)
        tk.Button(self, text="Run Load Cases on Existing BS", command=lambda: self.switch_to("RunLoadCaseOnBSNavFrame"), **button_style).pack(pady=5)


# Generic Input Frame
class InputFrame(BaseFrame):
    def __init__(self, parent, controller, fields, frame_name, back_frame, data_group_name):
        super().__init__(parent, controller)
        self.fields = fields
        self.frame_name = frame_name
        self.data_group_name = data_group_name  # Grouping name
        self.entries = {}
        
        for field in fields:
            tk.Label(self, text=f"Enter {field}:").pack()
            entry = tk.Entry(self)
            entry.pack()
            self.entries[field] = entry

        tk.Button(self, text="Save data", command=self.save_data).pack()
        tk.Button(self, text="Back", command=lambda: self.switch_to(back_frame)).pack()

    def save_data(self):
        grouped_data = {field: entry.get() for field, entry in self.entries.items()}  
        self.data_store.parameters[self.data_group_name] = grouped_data  # Store under group
        self.switch_to("FindOptimalBsNavFrame")

# Updated PlotFrame to handle normal and abnormal data
class PlotFrame(BaseFrame):
    def __init__(self, parent, controller, title, data_key):
        super().__init__(parent, controller)
        self.data_key = data_key  # Unique key for storing data in DataStore
        
        self.input_frame = ttk.Frame(self)
        self.input_frame.pack(side="left", padx=10, pady=10)
        self.plot_frame = ttk.Frame(self)
        self.plot_frame.pack(side="right", padx=10, pady=10)

        self.normal_rows = []
        self.abnormal_rows = []

        # Normal Operation Section
        self.normal_section = ttk.Frame(self.input_frame)
        self.normal_section.pack(pady=10)
        ttk.Label(self.normal_section, text="Normal Operation").pack()
        ttk.Button(self.normal_section, text="Add Row (Normal)", command=self.add_normal_row).pack()

        # Abnormal Operation Section
        self.abnormal_section = ttk.Frame(self.input_frame)
        self.abnormal_section.pack(pady=10)
        ttk.Label(self.abnormal_section, text="Abnormal Operation").pack()
        ttk.Button(self.abnormal_section, text="Add Row (Abnormal)", command=self.add_abnormal_row).pack()

        ttk.Button(self.input_frame, text="Plot Data", command=self.update_plot).pack(pady=10)
        ttk.Button(self.input_frame, text="Back", command=lambda: self.switch_to("FindOptimalBsNavFrame")).pack(pady=10)

        self.figure, self.ax = plt.subplots(figsize=(6, 4))
        self.ax.set_title(title)
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)
        self.canvas = FigureCanvasTkAgg(self.figure, self.plot_frame)
        self.canvas.get_tk_widget().pack()

    def add_normal_row(self):
        frame = ttk.Frame(self.normal_section)
        frame.pack()
        curvature_entry = ttk.Entry(frame, width=10)
        tension_entry = ttk.Entry(frame, width=10)
        curvature_entry.pack(side="left")
        tension_entry.pack(side="left")
        self.normal_rows.append((curvature_entry, tension_entry))

    def add_abnormal_row(self):
        frame = ttk.Frame(self.abnormal_section)
        frame.pack()
        curvature_entry = ttk.Entry(frame, width=10)
        tension_entry = ttk.Entry(frame, width=10)
        curvature_entry.pack(side="left")
        tension_entry.pack(side="left")
        self.abnormal_rows.append((curvature_entry, tension_entry))

    def update_plot(self):
        self.ax.clear()
        self.ax.set_title("Tension vs. Curvature")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)
        
        normal_curvature, normal_tension = self.get_data(self.normal_rows)
        abnormal_curvature, abnormal_tension = self.get_data(self.abnormal_rows)

        if normal_curvature and normal_tension:
            self.ax.plot(normal_curvature, normal_tension, marker='o', label="Normal", color='blue')
        if abnormal_curvature and abnormal_tension:
            self.ax.plot(abnormal_curvature, abnormal_tension, marker='s', label="Abnormal", color='red')

        self.ax.legend()
        self.canvas.draw()

        # Store data in DataStore
        self.data_store.set_parameter(self.data_key, {
            "normal": [normal_curvature, normal_tension],
            "abnormal": [abnormal_curvature, abnormal_tension]
        })

    def get_data(self, rows):
        """Extract data from input fields."""
        curvature = []
        tension = []
        for curvature_entry, tension_entry in rows:
            try:
                curvature.append(float(curvature_entry.get()))
                tension.append(float(tension_entry.get()))
            except ValueError:
                continue
        return curvature, tension
# Riser Response Frame using PlotFrame
class RiserResponseFrame(PlotFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller, "Riser Response", "riser_response_data")
# Riser Capacities Frame using PlotFrame
class RiserCapacitiesFrame(PlotFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller, "Riser Capacities", "riser_capacities_data")
class ProjectInfoFrame(InputFrame):
    def __init__(self, parent, controller):
        fields = ["Project Name", "client", "designer name"]
        super().__init__(parent, controller, fields, "ProjectInfoFrame", "FindOptimalBsNavFrame", "project_info")
class RiserInfoFrame(InputFrame):
    def __init__(self, parent, controller):
        fields = ["riser identification", "outer diameter", "outer diameter tolerance", "mass per unit length", "axial stiffness", "bending stiffness", "torsial stiffness", "riser length"]
        super().__init__(parent, controller, fields, "RiserInfoFrame", "FindOptimalBsNavFrame", "riser_info")
class BSDimensionsFrame(InputFrame):
    def __init__(self, parent, controller):
        fields = ["root length", "tip length", "min root outer diameter", "max root outer diameter", "min overall length", "max overall length", "clearance ", "increment width", "increment length"]
        super().__init__(parent, controller, fields, "BSDimensionsFrame", "FindOptimalBsNavFrame", "bs_dimension")
class SelectMaterialFrame(tk.Frame):
    def __init__(self, parent, data_store):
        super().__init__(parent)
        self.data_store = data_store

        tk.Label(self, text="Select Material").pack(pady=10)
        self.materials = self.load_materials()
        self.selected_material = tk.StringVar()

        self.dropdown = ttk.Combobox(self, textvariable=self.selected_material, values=list(self.materials.keys()))
        self.dropdown.pack(pady=5)

        tk.Button(self, text="Load Material", command=self.load_selected_material).pack(pady=10)
        tk.Button(self, text="Create New Material", command=self.create_new_material).pack(pady=10)

    def create_new_material(self):
        new_material_window = createNewMaterialFrame(self)
    def load_materials(self):
        db_path = "material_database.json"
        if os.path.exists(db_path):
            with open(db_path, "r") as file:
                return json.load(file)
        return {}

    def load_selected_material(self):
        material_name = self.selected_material.get()
        if material_name in self.materials:
            self.data_store.set_parameter("selected_material", self.materials[material_name])
            messagebox.showinfo("Material Loaded", f"Material '{material_name}' loaded into analysis.")
        else:
            messagebox.showerror("Error", "Please select a valid material.")

class createNewMaterialFrame:
    def __init__(self, parent):
        self.top = Toplevel(parent)
        self.top.title("Create New Material")
        self.top.geometry("500x800")

        self.entries = {}
        self.dynamic_entries = []

        tk.Label(self.top, text="Material identifier").pack(pady=5)
        self.entries["Material identifier "] = tk.Entry(self.top)
        self.entries["Material identifier "] .pack(pady=5)

        tk.Label(self.top, text="Number of datapoints in stress/strain table").pack(pady=5)
        self.entries["number of datapoints in stress/strain table"] = tk.Entry(self.top)
        self.entries["number of datapoints in stress/strain table"].pack(pady=5)

        self.generate_button = tk.Button(self.top, text="Generate Fields", command=self.generate_datapoint_fields)
        self.generate_button.pack(pady=10)

        # Scrollable canvas for dynamic entry fields
        self.canvas = Canvas(self.top, height=300)
        self.scrollable_frame = Frame(self.canvas)
        self.scrollbar = Scrollbar(self.top, orient="vertical", command=self.canvas.yview)
        self.canvas.configure(yscrollcommand=self.scrollbar.set)

        self.canvas.pack(side="left", fill="both", expand=True)
        self.scrollbar.pack(side="right", fill="y")
        self.canvas.create_window((0, 0), window=self.scrollable_frame, anchor="nw")

        self.scrollable_frame.bind(
            "<Configure>", lambda e: self.canvas.configure(scrollregion=self.canvas.bbox("all"))
        )

        self.submit_button = tk.Button(self.top, text="Submit", command=self.collect_material_data)
        self.submit_button.pack(pady=10)

    def generate_datapoint_fields(self):
        for entry in self.dynamic_entries:
            entry[0].destroy()
            entry[1].destroy()
        self.dynamic_entries.clear()

        try:
            num_points = int(self.entries["number of datapoints in stress/strain table"].get())
        except ValueError:
            messagebox.showerror("Invalid input", "Please enter a valid number of datapoints")
            return

        for i in range(num_points):
            row = Frame(self.scrollable_frame)
            row.pack(pady=2, padx=5, fill="x")

            tk.Label(row, text=f"Strain {i+1}", width=10, anchor="w").pack(side="left")
            strain_entry = tk.Entry(row, width=10)
            strain_entry.pack(side="left", padx=(0, 10))

            tk.Label(row, text=f"Stress {i+1}", width=10, anchor="w").pack(side="left")
            stress_entry = tk.Entry(row, width=10)
            stress_entry.pack(side="left")

            self.dynamic_entries.append((strain_entry, stress_entry))

    def collect_material_data(self):
        material_id = self.entries["Material identifier "] .get()
        num_points = self.entries["number of datapoints in stress/strain table"].get()

        try:
            num_points = int(num_points)
        except ValueError:
            messagebox.showerror("Error", "Invalid number of datapoints")
            return

        datapoints = []
        for strain_entry, stress_entry in self.dynamic_entries:
            try:
                strain = float(strain_entry.get())
                stress = float(stress_entry.get())
                datapoints.append({"strain": strain, "stress": stress})
            except ValueError:
                messagebox.showerror("Error", "Please enter valid numbers for all datapoints")
                return

        db_path = "material_database.json"
        if os.path.exists(db_path):
            with open(db_path, "r") as file:
                material_db = json.load(file)
        else:
            material_db = {}

        material_db[material_id] = {
            "NMAT": num_points,
            "data": datapoints
        }

        with open(db_path, "w") as file:
            json.dump(material_db, file, indent=4)

        messagebox.showinfo("Success", f"Material '{material_id}' saved to database!")
        self.top.destroy()

class BSDimensionsFrameMulti(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)

        self.controller = controller
        self.num_bs = tk.IntVar(value=1)  # Default 1 BS entry
        self.entries = []

        tk.Label(self, text="Enter number of BS:").pack(pady=5)
        tk.Entry(self, textvariable=self.num_bs, width=5).pack(pady=5)

        tk.Button(self, text="Generate Fields", command=self.generate_fields).pack(pady=5)

        self.input_frame = tk.Frame(self)
        self.input_frame.pack(pady=10)

        tk.Button(self, text="Save Data", command=self.save_data).pack(pady=5)
        tk.Button(self, text="Back", command=lambda: self.switch_to("FindOptimalBsNavFrame")).pack(pady=5)

        tk.Button(self, text="Generate Cases", command=lambda:DataProcessor().generate_case_files_multi_BS_btn()).pack(pady=5)
        tk.Button(self, text="Run Analysis", command=lambda:DataProcessor().multithreadedAnalysis()).pack(pady=5)
        

    def generate_fields(self):
        """Generates multiple BS input sections based on user input."""
        for widget in self.input_frame.winfo_children():
            widget.destroy()  # Clear old input fields

        self.entries = []  # Reset stored entries

        num_bs = self.num_bs.get()
        fields = ["Root Length", "Cone Length", "Tip Length", "Root Outer Diameter",
                  "Tip Outer Diameter", "ID"]

        for i in range(num_bs):
            tk.Label(self.input_frame, text=f"BS {i+1}", font=("Arial", 12, "bold")).pack(pady=5)

            entry_group = {}
            for field in fields:
                frame = tk.Frame(self.input_frame)
                frame.pack(pady=2, fill="x")

                tk.Label(frame, text=field, width=25, anchor="w").pack(side="left")
                entry = tk.Entry(frame, width=15)
                entry.pack(side="right")

                entry_group[field] = entry

            self.entries.append(entry_group)  # Store the group

    def save_data(self):
        """Saves all entered BS dimensions to DataStore."""
        bs_data = []

        for i, entry_group in enumerate(self.entries):
            bs_entry = {}
            for field, entry in entry_group.items():
                bs_entry[field] = entry.get()
            bs_data.append(bs_entry)

        self.data_store.set_parameter("bs_dimension_multi", bs_data)
        print("BS Dimensions Saved:", bs_data)  # Debugging

class RunAnalysisFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        self.controller = controller
        self.labels = {}  # Store labels dynamically
        self.data_processor = DataProcessor()  # Get the singleton instance
        self.data_store = DataStore()  # Singleton instance for data storage

        # Title
        tk.Label(self, text="Run Analysis", font=("Arial", 14, "bold")).pack(pady=10)

        # Progress Bar
        self.progress = ttk.Progressbar(self, orient="horizontal", length=300, mode="determinate")
        self.progress.pack(pady=10)

        # Buttons
        tk.Button(self, text="Go Back", command=lambda: self.switch_to("NavigationFrame")).pack(pady=5)
        tk.Button(self, text="Refresh", command=self.update_labels).pack(pady=5)
        tk.Button(self, text="Generate Cases", command=self.data_processor.casesBtn).pack(pady=5)
        tk.Button(self, text="Run Analysis", command=self.run_analysis).pack(pady=5)
        tk.Button(self, text="Save Data", command=self.data_store.save_data).pack(pady=5)
        tk.Button(self, text="Load Data", command=self.load_data).pack(pady=5)

        self.update_labels()

    def run_analysis(self):
        """Runs analysis in a separate thread and updates the progress bar."""
        self.progress["value"] = 0  # Reset progress bar
        thread = threading.Thread(target=self.analysis_task, daemon=True)
        thread.start()

    def analysis_task(self):
        """Performs analysis and updates progress safely."""
        total_steps = 10  # Simulated steps for progress (increase if needed)

        for i in range(total_steps):
            time.sleep(0.2)  # Simulate processing
            self.controller.after(0, self.update_progress, i + 1, total_steps)  # Update progress in main thread

        # Run actual analysis **in the main thread**
        self.controller.after(0, self.run_analysis_main_thread)

    def run_analysis_main_thread(self):
        """Runs the analysis on the main thread and ensures ReportFrame is shown."""
        self.data_processor.loadBSCases("bsengine-cases-normal.txt")  # Now safely runs in the main thread

        # Retrieve updated analysis data
        analysis_data = self.data_store.parameters

        # **Always create a new ReportFrame to ensure fresh data**
        if "ReportFrame" in self.controller.frames:
            del self.controller.frames["ReportFrame"]  # Remove old frame

        # Create new ReportFrame with updated data
        report_frame = ReportFrame(self.controller.container, self.controller, analysis_data)
        self.controller.frames["ReportFrame"] = report_frame

        # **Add it to the Tkinter grid properly**
        report_frame.grid(row=0, column=0, sticky="nsew")

        # **Switch to ReportFrame**
        self.controller.show_frame("ReportFrame")

    def update_progress(self, value, max_value):
        """Updates the progress bar safely in the main thread."""
        self.progress["value"] = (value / max_value) * 100
        self.progress.update_idletasks()

    def load_data(self):
        """Loads data and refreshes labels."""
        self.data_store.load_data()
        self.update_labels()  # Refresh UI after loading

    def update_labels(self):
        """Updates displayed labels with stored parameters."""
        for label in self.labels.values():
            label.destroy()  # Remove old labels
        self.labels.clear()

        for key, value in self.data_store.parameters.items():
            label = tk.Label(self, text=f"{key}: {value}")
            label.pack()
            self.labels[key] = label

class ReportFrame(BaseFrame):
    def __init__(self, parent, controller, analysis_data):
        super().__init__(parent, controller)

        self.controller = controller
        self.data_store = analysis_data  # Store received data
        self.analysis_data = analysis_data

        self.case = analysis_data.get("shortest_valid_result")
        self.capacities = analysis_data.get("riser_capacities")
        self.response = analysis_data.get("riser_response")
        self.threshold_normal = analysis_data.get("thresholds_normal")
        self.threshold_abnormal = analysis_data.get("thresholds_abnormal")

        self.normal_curves, self.abnormal_curves = self.getCurves(self.case)

        # Main container frame
        self.main_frame = tk.Frame(self)
        self.main_frame.pack(fill="both", expand=True)

        # Table Frame
        self.table_frame = ttk.Frame(self.main_frame)
        self.table_frame.pack(side="left", fill="both", expand=True, padx=10, pady=10)
        self.create_tkinter_table(self.table_frame)

        # Plot Frame
        self.plot_frame = ttk.Frame(self.main_frame)
        self.plot_frame.pack(side="right", fill="both", expand=True, padx=10, pady=10)

        # Matplotlib Figure
        self.figure, self.ax = plt.subplots(figsize=(6, 6))
        self.ax.set_title("Tension vs. Curvature")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)

        self.canvas = FigureCanvasTkAgg(self.figure, self.plot_frame)
        self.canvas.get_tk_widget().pack(fill="both", expand=True)

        self.update_plot()

        # Back Button
        self.back_button = tk.Button(self.main_frame, text="Back", command=lambda: self.switch_to("FindOptimalBsNavFrame"), width=10, height=2, bg="#333333", fg="white")
        self.back_button.pack(side="bottom", pady=10)

        if(self.analysis_data.get("bs_dimension_multi")):
            self.generateCasesForMultipleBs()

    def update_plot(self):
        """Update the plot with the data from DataStore."""
        self.ax.clear()
        self.ax.set_title(f"BS Response at NO and AO Loads for case:\n {self.case}")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)

        if self.capacities and self.response:
            # Capacities
            self.ax.plot(self.capacities["normal"][0], self.capacities["normal"][1], marker='o', label="Normal Capacity", color='blue', linewidth=0.5, markersize=5)
            self.ax.plot(self.capacities["abnormal"][0], self.capacities["abnormal"][1], marker='s', label="Abnormal Capacity", color='red', linewidth=0.5, markersize=5)

            # Response
            normal_curvature_resp = [float(curve["maximum_bs_curvature"]) for curve in self.normal_curves]
            abnormal_curvature_resp = [float(curve["maximum_bs_curvature"]) for curve in self.abnormal_curves]

            self.ax.plot(normal_curvature_resp, self.response["normal"][1], marker='o', label="Normal Response", color='blue', linewidth=0.5, markersize=5, markerfacecolor='yellow')
            self.ax.plot(abnormal_curvature_resp, self.response["abnormal"][1], marker='s', label="Abnormal Response", color='red', linewidth=0.5, markersize=5, markerfacecolor='yellow')

        self.ax.legend()
        self.canvas.draw()

    def create_tkinter_table(self, frame):
        """Creates tables inside a Tkinter frame using Treeview to display stored data."""
        
        def create_table(parent, title, columns, values):
            """Helper function to create and pack a table."""
            label = tk.Label(parent, text=title, font=("Arial", 12, "bold"), anchor="w")
            label.pack(anchor="w", pady=5)

            tree = ttk.Treeview(parent, columns=columns, show="headings", height=min(10, len(values)))
                
            for col in columns:
                tree.heading(col, text=col)
                tree.column(col, width=150, anchor="center")

            for val in values:
                tree.insert("", "end", values=val)

            tree.pack(fill="both", expand=True, pady=5)

        # Retrieve data from DataStore
        data = self.analysis_data

        # Create tables
        create_table(frame, "Project Information", ["Field", "Value"], data["project_info"].items())
        create_table(frame, "Riser Information", ["Field", "Value"], data["riser_info"].items())
        create_table(frame, "BS Dimensions", ["Field", "Value"], data["bs_dimension"].items())

        material_data = [(section, material["material_characteristics"], material["elastic_modules"]) for section, material in data["bs_material"].items()]
        create_table(frame, "BS Material", ["Section", "Material", "Elastic Modules"], material_data)

        # Threshold Normal
        threshold_normal_data = [
            (i+1, round(self.response["normal"][1][i], 5), round(self.response["normal"][0][i], 5), round(threshold[1]["maximum_curvature"], 5), round(float(self.normal_curves[i]["maximum_curvature"]), 5))
            for i, threshold in enumerate(self.threshold_normal.items())
        ]
        create_table(frame, "Threshold Normal", ["Case", "Tension", "Angle", "Threshold", "Max Curvature"], threshold_normal_data)

        # Threshold Abnormal
        threshold_abnormal_data = [
            (i+1, round(self.response["abnormal"][1][i], 5), round(self.response["abnormal"][0][i], 5), round(threshold[1]["maximum_curvature"], 5), round(float(self.abnormal_curves[i]["maximum_curvature"]), 5))
            for i, threshold in enumerate(self.threshold_abnormal.items())
        ]
        create_table(frame, "Threshold Abnormal", ["Case", "Tension", "Angle", "Threshold", "Max Curvature"], threshold_abnormal_data)

    def getCurves(self, case):
        """Extract curves from log files based on case name."""
        if isinstance(case, pd.Series):  
            case = case["case_name"]  # Extract the string from the 'case_name' column

        case = str(case)  # Ensure it is a string
        noOfCases = len(self.response['normal'][0])

        try:
            length, width = map(float, case.split('-')[2:4])  # Extract numeric values
        except (IndexError, ValueError):
            raise ValueError(f"Invalid case format: {case}")  # Handle errors gracefully

        normal_curves = [
            self.extract_data(f"case_files/Case-normal{i}-{length}-{width}-60D_30.log")
            for i in range(1, noOfCases + 1)
        ]
        abnormal_curves = [
            self.extract_data(f"case_files/Case-abnormal{i}-{length}-{width}-60D_30.log")
            for i in range(1, noOfCases + 1)
        ]

        return normal_curves, abnormal_curves


    def extract_data(self, case_file):
        """Extracts key values from log files."""
        try:
            with open(case_file, 'r') as res_file:
                res_lines = res_file.readlines()
            
            keyres1 = keyres2 = None
            for line in res_lines:
                if "Maximum BS curvature" in line:
                    keyres1 = line.strip().split(':')[-1].strip()
                if "Maximum curvature" in line:
                    keyres2 = line.strip().split(':')[-1].strip()

            if keyres1 and keyres2:
                return {"case_name": case_file.rstrip('.log'), "maximum_bs_curvature": keyres1, "maximum_curvature": keyres2}
        except FileNotFoundError:
            print(f"File not found: {case_file}")
        return None
    

def create_banner(parent, controller=None, go_back_callback=None, menu_callback=None):
    banner_color = "#1D6F6E"
    banner = tk.Frame(parent, height=37, bg=banner_color)
    banner.pack(side="top", fill="x")
    banner.pack_propagate(False)

    # Bottom border only
    bottom_border = tk.Frame(banner, bg="black", height=2)
    bottom_border.pack(side="bottom", fill="x")

    # Logo as image (PNG)
    try:
        logo_image = tk.PhotoImage(file="logo.png")  # <-- update path if needed
        logo = tk.Label(banner, image=logo_image, bg=banner_color)
        logo.image = logo_image  # prevent garbage collection
    except Exception as e:
        print(f"Failed to load logo: {e}")
        logo = tk.Label(banner, text="LOGO", fg="white", bg=banner_color)

    logo.place(x=5, y=5)

    # MENU button
    menu_btn = tk.Button(banner, text="MENU", bg="#E3C376", fg="black",
                         bd=1, relief="solid", highlightbackground="black",
                         command=menu_callback or (lambda: print("Menu")))
    menu_btn.place(x=40, y=5, width=87, height=26)

    # BACK button
    back_btn = tk.Button(banner, text="BACK", bg="black", fg="white",
                         bd=1, relief="solid", highlightbackground="black",
                         command=go_back_callback or (lambda: print("Back")))
    back_btn.place(x=135, y=5, width=87, height=26)

    # OK button
    ok_btn = tk.Button(banner, text="OK", bg="white", fg="black",
                       bd=1, relief="solid", highlightbackground="black",
                       command=lambda: print("OK"))
    ok_btn.place(x=230, y=5, width=87, height=26)

    # Divider
    divider = tk.Frame(banner, bg="black", width=2, height=28)
    divider.place(x=327 + 10, y=5)

    # Entry fields with placeholders
    entries = {}
    labels = ["Project Name", "Client", "Designer"]
    start_x = 337 + 10
    spacing = 222

    for i, label_text in enumerate(labels):
        x = start_x + i * spacing

        entry = tk.Entry(banner, bg="black", insertbackground="white",
                         relief="flat", highlightthickness=1, highlightbackground="white")
        entry.place(x=x, y=5, width=212, height=26)
        add_placeholder(entry, label_text)

        entries[label_text.lower()] = entry

    return banner, entries

def add_placeholder(entry_widget, placeholder_text, color="gray"):
    def on_focus_in(event):
        if entry_widget.get() == placeholder_text:
            entry_widget.delete(0, "end")
            entry_widget.config(fg="white")

    def on_focus_out(event):
        if not entry_widget.get():
            entry_widget.insert(0, placeholder_text)
            entry_widget.config(fg=color)

    entry_widget.insert(0, placeholder_text)
    entry_widget.config(fg=color)

    entry_widget.bind("<FocusIn>", on_focus_in)
    entry_widget.bind("<FocusOut>", on_focus_out)


# Main App Controller
class App(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("BS Ultradeep")
        self.geometry("1080x720")
        self.iconbitmap("ultrabend_proposed_logo.ico")
        self.protocol("WM_DELETE_WINDOW", self.quit)

        # NEW master layout frame
        self.main_frame = tk.Frame(self)
        self.main_frame.pack(fill="both", expand=True)

        # Banner goes at the top of main_frame
        self.banner, self.banner_entries = create_banner(
            self.main_frame,
            self,
            go_back_callback=lambda: self.show_frame("PreviousFrameName"),
            menu_callback=lambda: self.show_frame("NavigationFrame")
        )

        # Container for the pages (goes below the banner)
        self.container = tk.Frame(self.main_frame, width=1080, height=720, bg="#E3DFCF")
        self.container.pack(fill="both", expand=True)

        self.frames = {}

        for frame_name in [
            "FindOptimalBsNavFrame", "ProjectInfoFrame", 
            "RiserInfoFrame", "BSDimensionsFrame", 
            "RiserResponseFrame", "RiserCapacitiesFrame", 
            "RunAnalysisFrame", "NavigationFrame", 
            "CheckExistingBSNavFrame", "RunLoadCaseOnBSNavFrame",
            "BSDimensionsFrameMulti", "SelectMaterialFrame"
        ]:
            frame = globals()[frame_name](self.container, self)
            self.frames[frame_name] = frame
            frame.grid(row=0, column=0, sticky="nsew")

        self.show_frame("NavigationFrame")

    def show_frame(self, frame_name):
        self.frames[frame_name].tkraise()

if __name__ == "__main__":
    app = App()
    app.mainloop()
    app.destroy()  # Close the app properly
