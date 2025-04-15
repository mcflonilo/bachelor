import tkinter as tk
from dataclasses import dataclass, field
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
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
from tkinter import messagebox, Toplevel, ttk, Canvas, Frame, Scrollbar,filedialog
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
        grouped_data = {field: entry.get() for field, entry in self.entries.items()}  
        self.data_store.parameters[self.data_group_name] = grouped_data

        if hasattr(self.controller, "update_banner_info"):
            self.controller.update_banner_info()
        self.go_back()

    def update_banner_info(self):
        project_info = DataStore().get_parameter("project_info")

    # Map keys in project_info to their respective label prefixes
        key_to_prefix = {
            "project name": "Project Name: ",
            "client": "Client: ",
            "designer name": "Designer: "
        }

    # Loop through the mapping and update labels safely
        for key, prefix in key_to_prefix.items():
        # Normalize keys to lowercase when accessing banner_entries
            entry_key = key.lower()

            if entry_key in self.banner_entries:
                value = project_info.get(key, "")
                label_widget = self.banner_entries[entry_key]

            # Only update label if value is non-empty
                if value:
                    label_widget.config(text=f"{prefix}{value}")

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
        riser_info = self.data.get_parameter("riser_info")
        bs_dim = self.data.get_parameter("bs_dimension")
        material_text_block = self.data.get_parameter("material_text_block")
        material_identifier = self.data.get_parameter("material_identifier")

        length = round(float(riser_info["riser length"]), 3)
        EA = round(float(riser_info["axial stiffness"]), 3)
        EI = round(float(riser_info["bending stiffness"]), 3)
        GT = round(float(riser_info["torsial stiffness"]), 3)
        m = round(float(riser_info["mass per unit length"]), 3)

        ID = round(float(riser_info["outer diameter"]) + 
                (2 * (float(riser_info["outer diameter tolerance"]) + float(bs_dim["clearance"]))), 3)
        SL = round(0.700, 3)

        CL, OD = self.cl_od_to_array(
            round(float(bs_dim["min overall length"]), 3), 
            round(float(bs_dim["max overall length"]), 3), 
            round(float(bs_dim["min root outer diameter"]), 3), 
            round(float(bs_dim["max root outer diameter"]), 3), 
            round(float(bs_dim["increment length"]), 3), 
            round(float(bs_dim["increment width"]), 3)
        )

        TL = round(float(bs_dim["tip length"]), 3)
        TOD = round(ID + 0.04, 3)  # TEMPORARY VALUE

        normal_cases, abnormal_cases = self.make_case_arrays(self.data.get_parameter("riser_response"))

        casetotal = 0
        casetotal += self.generate_case_files(length, EA, EI, GT, m, normal_cases, ID, SL, OD, CL, TL, TOD, material_text_block, material_identifier, "normal")
        casetotal += self.generate_case_files(length, EA, EI, GT, m, abnormal_cases, ID, SL, OD, CL, TL, TOD, material_text_block, material_identifier, "abnormal")

        tk.messagebox.showinfo("Case Files", f"Generated {casetotal} cases.")


    def generate_case_files_multi_BS_btn(self):
        length = round(float(self.data.get_parameter("riser_info")["riser length"]), 3)
        EA = round(float(self.data.get_parameter("riser_info")["axial stiffness"]), 3)
        EI = round(float(self.data.get_parameter("riser_info")["bending stiffness"]), 3)
        GT = round(float(self.data.get_parameter("riser_info")["torsial stiffness"]), 3)
        m = round(float(self.data.get_parameter("riser_info")["mass per unit length"]), 3)
        normal_cases, abnormal_cases = self.make_case_arrays(self.data.get_parameter("riser_response"))
        MATID = self.data.get_parameter("material_identifier")
        bs_dimension_multi = self.data.get_parameter("bs_dimension_multi")
        bs_mat_text_block = self.data.get_parameter("material_text_block")

        casetotal = 0
        casetotal +=self.generate_case_files_multi_BS(length,EA,EI,GT,m,normal_cases,bs_dimension_multi,bs_mat_text_block,MATID,"normal", "case_files")
        casetotal +=self.generate_case_files_multi_BS(length,EA,EI,GT,m,abnormal_cases,bs_dimension_multi,bs_mat_text_block,MATID,"abnormal", "case_files")
        tk.messagebox.showinfo("Case Files", f"generated {casetotal} cases.")

    def generate_case_files_multi_BS(self, length, EA, EI, GT, m, cases, bs_dimension_multi, material_text_block, matID, label, output_dir):
        print(bs_dimension_multi)
        """Generates case files for each case in `cases` for all BS dimensions in `bs_dimension_multi`."""

        os.makedirs(output_dir, exist_ok=True)
        case_list_filename = os.path.join(os.path.dirname(output_dir), f'bsengine-cases-{label}.txt')

        with open(case_list_filename, 'w') as case_list_file:
            total_cases = 0

            for bs in bs_dimension_multi:
                ID = round(float(bs["ID"]), 3)
                OD = round(float(bs["Root Outer Diameter"]), 3)
                CL = round(float(bs["Cone Length"]), 3)
                TL = round(float(bs["Tip Length"]), 3)
                TOD = round(float(bs["Tip Outer Diameter"]), 3)

                for case_id, case_data in enumerate(cases):
                    case_filename = f"Case-{label}-{case_id+1}-ID{ID}-OD{OD}-CL{CL}-TL{TL}-TOD{TOD}.inp"
                    case_filepath = os.path.join(output_dir, case_filename)
                    case_list_file.write(case_filepath + '\n')

                    with open(case_filepath, 'w') as inp:
                        inp.write('BEND STIFFENER DATA\n')
                        inp.write("' ID   NSEG\n")
                        inp.write("' inner diameter      Number of linear segments\n")
                        inp.write(f"{ID}   3\n")
                        inp.write("' LENGTH   NEL   OD1    OD2  DENSITY LIN/NOLIN        EMOD/MAT-ID\n")
                        inp.write(f"0.7  50  {OD}  {OD}  2000  LIN  10000.E06\n")
                        inp.write(f"{CL}  100  {OD}  {TOD}  1150  NOLIN {matID}\n")
                        inp.write(f"{TL}  50  {TOD}  {TOD}  1150  NOLIN {matID}\n")
                        inp.write("'-----------------------------------------------------------------------\n\n")

                        inp.write("RISER DATA\n")
                        inp.write("'SRIS,  NEL   EI,    EA,      GT     Mass\n")
                        inp.write("' (m)         kNm^2  kN              kg/m\n")
                        inp.write(f"{length}  100  {EI}  {EA}  {GT}  {m}\n\n")

                        inp.write("ELEMENT PRINT\n'NSPEC\n3\n'IEL1    IEL2\n1         9\n10        30\n70        80\n")
                        inp.write("FE SYSTEM DATA TEST PRINT\n'IFSPRI 1/2\n1\n'2\n")
                        inp.write("FE ANALYSIS PARAMETERS\n'TOLNOR  MAXIT\n1.E-07  30\n")
                        inp.write("'DSINC,DSMIN,DSMAX,\n0.01  0.001  0.1\n'3.0  0.01 10.\n'5.  0.1 10.\n")
                        inp.write("CURVATURE RANGE\n'CURMAX  - Maximum curvature\n'NCURV   - Number of curvature levels\n")
                        inp.write("'CURMAX (1/m),NCURV\n'0.5       30\n0.2       100\n")
                        inp.write("FORCE\n'Relang  tension\n'(deg)   (kN)\n")
                        inp.write(f"{case_data[0]}  {case_data[1]}\n")
                        inp.write("'8.00   400.0\n'16.5   500.0\n'19.0   550.0\n")
                        inp.write("'19.1   600.0\n'18.6   650.0\n'17.5   700.0\n'14.0   775.0\n")

                        # Inject material block (already formatted)
                        inp.write(material_text_block + '\n')

                        inp.write("'EXPORT MATERIAL DATA\n'--------------------------------------------------\n")
                        inp.write("' IMEX   = 1 : tabular  =2 riflex format\n'  1\n")
                        inp.write("'---------------------------------------------\n")
                        inp.write("end\n")

                    total_cases += 1

        return total_cases


    def generate_case_files(self, length, EA, EI, GT, m, cases, ID, SL, OD, CL, TL, TOD, material_text_block, matID, label):

        inp1 = open(f'bsengine-cases-{label}.txt', 'w')
        y = 0  # Case counter

        for i in cases:
            y += 1
            for j in OD:
                for k in CL:
                    os.makedirs('case_files', exist_ok=True)

                    filename = f"Case-{label}{y}-{j}-{k}-{matID}.inp"
                    filepath = os.path.join('case_files', filename)

                    with open(filepath, 'w') as inp:
                        inp1.write(filepath + '\n')

                        # Write fixed BSENGINE inputs...
                        inp.write('BEND STIFFENER DATA\n')
                        inp.write("' ID   NSEG\n")
                        inp.write("' inner diameter      Number of linear segments\n")
                        inp.write(f"{ID}   3\n")
                        inp.write("' LENGTH   NEL   OD1    OD2  DENSITY LIN/NOLIN        EMOD/MAT-ID\n")
                        inp.write(f"{SL}  50  {j}  {j}  2000  LIN  10000.E06\n")
                        inp.write(f"{k}  100  {j}  {TOD}  1150  NOLIN {matID}\n")
                        inp.write(f"{TL}  50  {TOD}  {TOD}  1150  NOLIN {matID}\n")
                        inp.write("'-----------------------------------------------------------------------\n\n")

                        # Riser section...
                        inp.write("RISER DATA\n")
                        inp.write("'SRIS,  NEL   EI,    EA,      GT     Mass\n")
                        inp.write("' (m)         kNm^2  kN              kg/m\n")
                        inp.write(f"{length}  100  {EI}  {EA}  {GT}  {m}\n\n")

                        # Standard config sections...
                        inp.write("ELEMENT PRINT\n'NSPEC\n3\n'IEL1    IEL2\n1         9\n10        30\n70        80\n")
                        inp.write("FE SYSTEM DATA TEST PRINT\n'IFSPRI 1/2\n1\n'2\n")
                        inp.write("FE ANALYSIS PARAMETERS\n'TOLNOR  MAXIT\n1.E-07  30\n")
                        inp.write("'DSINC,DSMIN,DSMAX,\n0.01  0.001  0.1\n'3.0  0.01 10.\n'5.  0.1 10.\n")
                        inp.write("CURVATURE RANGE\n'CURMAX  - Maximum curvature\n'NCURV   - Number of curvature levels\n")
                        inp.write("'CURMAX (1/m),NCURV\n'0.5       30\n0.2       100\n")
                        inp.write("FORCE\n'Relang  tension\n'(deg)   (kN)\n")
                        inp.write(f"{i[0]}  {i[1]}\n'8.00   400.0\n'16.5   500.0\n'19.0   550.0\n")
                        inp.write("'19.1   600.0\n'18.6   650.0\n'17.5   700.0\n'14.0   775.0\n")

                        # Insert dynamic material block
                        inp.write(material_text_block + '\n')

                        inp.write("'EXPORT MATERIAL DATA\n'--------------------------------------------------\n")
                        inp.write("' IMEX   = 1 : tabular  =2 riflex format\n'  1\n")
                        inp.write("'---------------------------------------------\n")
                        inp.write("end\n")

        inp1.close()
        return sum(1 for _ in open(f'bsengine-cases-{label}.txt'))

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
            case = f"case_files\\Case-{tag}{i}-{shortest['width']}-{shortest['length']}-{self.data.get_parameter("material_identifier")}.inp"
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
            print (f"Extracted result: {result}")
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
            """Checks the results of the analysis and prints a status for all BS configurations (PASSED/FAILED)."""

            # Step 1: Interpolate max curvature
            self.interpolate_max_curve(
                self.data.get_parameter("riser_capacities"),
                self.data.get_parameter("riser_response")
            )

            # Step 2: Read case filenames
            normal_cases = open('bsengine-cases-normal.txt', 'r').readlines()
            abnormal_cases = open('bsengine-cases-abnormal.txt', 'r').readlines()

            # Step 3: Extract log paths
            normal_logs = [case.strip().replace('.inp', '.log') for case in normal_cases]
            abnormal_logs = [case.strip().replace('.inp', '.log') for case in abnormal_cases]

            # Step 4: Get results
            normal_results = {log: self.extract_key_results(log) for log in normal_logs}
            abnormal_results = {log: self.extract_key_results(log) for log in abnormal_logs}

            thresholds_normal = self.data.get_parameter("thresholds_normal")
            thresholds_abnormal = self.data.get_parameter("thresholds_abnormal")

            bs_cases = defaultdict(lambda: {"normal": [], "abnormal": []})

            # Group cases
            for case_name in normal_cases + abnormal_cases:
                case_name = case_name.strip()
                match = re.search(r'Case-(normal|abnormal)-(\d+)-ID([\d.]+)-OD([\d.]+)-CL([\d.]+)-TL([\d.]+)-TOD([\d.]+)', case_name)
                if not match:
                    logging.warning(f"⚠️ Could not parse case name: {case_name}")
                    continue

                case_type, case_num, bs_id, od, cl, tl, tod = match.groups()
                case_num = int(case_num)
                bs_key = f"ID{bs_id}-OD{od}-CL{cl}-TL{tl}-TOD{tod}"
                bs_cases[bs_key][case_type].append(case_num)

            all_bs_status = {}

            # Evaluate each BS
            for bs_key, case_dict in bs_cases.items():
                normal_passed = True
                abnormal_passed = True
                failure_reason = None

                for case_num in case_dict["normal"]:
                    log_file = f"case_files\\Case-normal-{case_num}-{bs_key}log"
                    result = normal_results.get(log_file)
                    threshold = thresholds_normal.get(f"case_files\\Case-normal{case_num}")

                    if result is None or threshold is None:
                        normal_passed = False
                        failure_reason = f"Missing result or threshold for normal case {case_num}"
                        break

                    curvature = float(result["maximum_curvature"])
                    if curvature > threshold["maximum_curvature"]:
                        normal_passed = False
                        failure_reason = f"Normal case {case_num} exceeded threshold ({curvature} > {threshold['maximum_curvature']})"
                        break

                if normal_passed:
                    for case_num in case_dict["abnormal"]:
                        log_file = f"case_files\\Case-abnormal-{case_num}-{bs_key}log"
                        result = abnormal_results.get(log_file)
                        threshold = thresholds_abnormal.get(f"case_files\\Case-abnormal{case_num}")

                        if result is None or threshold is None:
                            abnormal_passed = False
                            failure_reason = f"Missing result or threshold for abnormal case {case_num}"
                            break

                        curvature = float(result["maximum_curvature"])
                        if curvature > threshold["maximum_curvature"]:
                            abnormal_passed = False
                            failure_reason = f"Abnormal case {case_num} exceeded threshold ({curvature} > {threshold['maximum_curvature']})"
                            break

                if normal_passed and abnormal_passed:
                    all_bs_status[bs_key] = "✅ PASSED"
                else:
                    all_bs_status[bs_key] = f"❌ FAILED – {failure_reason}"

            # Print summary
            print("\n=== BS Configuration Results ===")
            for bs_key, status in all_bs_status.items():
                print(f"{bs_key}: {status}")

            return [bs for bs, status in all_bs_status.items() if status.startswith("✅")]
        run_threads()
        check_results()

# Base Frame for all screens
class BaseFrame(tk.Frame):
    def __init__(self, parent, controller):
        super().__init__(parent, bg="#E3DFCF")
        self.controller = controller
        self.data_store = DataStore()  # All frames share this instance
        self.button_style = {
            "bg": "#E3C376", "fg": "black",
            "width": 30, "height": 2,
            "bd": 1, "relief": "solid",
            "highlightbackground": "black",
            "font": ("Arial", 10, "bold"),
            "activebackground": "#d2b660"
        }
        self.sub_button_style = {
            "bg": "#E3C376", "fg": "black",
            "width": 20, "height": 2,
            "bd": 1, "relief": "solid",
            "highlightbackground": "black",
            "font": ("Arial", 10, "bold"),
            "activebackground": "#d2b660"
        }
        self.back_button_style = {
            "bg": "#000000", "fg": "white",
            "width": 30, "height": 2,
            "bd": 1, "relief": "solid",
            "highlightbackground": "black",
            "font": ("Arial", 10, "bold"),
            "activebackground": "#d2b660"
        }
        self.title_style = {
            "bg": "#E3DFCF",
            "fg": "black",
            "font": ("Arial", 20, "bold"),
            "anchor": "w",
            "padx": 5,
            "pady": 2
        }
        self.label_style = {
            "bg": "#E3DFCF",
            "fg": "black",
            "font": ("Arial", 15, "bold"),
            "anchor": "w",
            "padx": 5,
            "pady": 2
        }
        self.inputFrame_style = {
            "bg": "#E3DFCF",
            "fg": "black",
            "font": ("Arial", 10, "bold"),
            "anchor": "w",
            "padx": 5,
            "pady": 2
        }
        self.label_plot_style = {
            "bg": "#1D6F6E",
            "fg": "white",
            "font": ("Arial", 15, "bold"),
            "anchor": "w",
            "padx": 5,
            "pady": 2
        }

        self.center_frame = tk.Frame(self, bg=self["bg"])
        self.center_frame.place(relx=0.5, rely=0.30, anchor="center")

    def set_previous_frame(self, frame_name):
        self.previous_frame = frame_name

    def go_back(self):
        if self.previous_frame:
            self.controller.show_frame(self.previous_frame)

    def switch_to(self, frame_name):
        self.controller.show_frame(frame_name)

# Navigation Frame - design optimal BS frame
class FindOptimalBsNavFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        tk.Label(self.center_frame, text="Navigation", **self.title_style).pack(pady=(110, 10))
        tk.Button(self.center_frame, text="Project Info", command=lambda: self.switch_to("ProjectInfoFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Info", command=lambda: self.switch_to("RiserInfoFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="BS Dimensions", command=lambda: self.switch_to("BSDimensionsFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Response", command=lambda: self.switch_to("RiserResponseFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Capacities", command=lambda: self.switch_to("RiserCapacitiesFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="BS Material", command=lambda: self.switch_to("SelectMaterialFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Run Analysis", command=lambda: self.switch_to("RunAnalysisFrame"), **self.button_style).pack(pady=5)

class CheckExistingBSNavFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        tk.Label(self.center_frame, text="Navigation", **self.title_style).pack(pady=(110, 10))
        tk.Button(self.center_frame, text="Project Info", command=lambda: self.switch_to("ProjectInfoFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Info", command=lambda: self.switch_to("RiserInfoFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Response", command=lambda: self.switch_to("RiserResponseFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Capacities", command=lambda: self.switch_to("RiserCapacitiesFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="BS Material", command=lambda: self.switch_to("SelectMaterialFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="BS designs", command=lambda: self.switch_to("BSDimensionsFrameMulti"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Run Analysis", command=lambda: self.switch_to("RunAnalysisFrame"), **self.button_style).pack(pady=5)

class RunLoadCaseOnBSNavFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        tk.Label(self.center_frame, text="Navigation", **self.title_style).pack(pady=(110, 10))
        tk.Button(self.center_frame, text="Project Info", command=lambda: self.switch_to("ProjectInfoFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Info", command=lambda: self.switch_to("RiserInfoFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="BS Dimensions", command=lambda: self.switch_to("BSDimensionsFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Response", command=lambda: self.switch_to("RiserResponseFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Riser Capacities", command=lambda: self.switch_to("RiserCapacitiesFrame"), **self.button_style).pack(pady=5)
        tk.Button(self.center_frame, text="Run Analysis", command=lambda: self.switch_to("RunAnalysisFrame"), **self.button_style).pack(pady=5)

# start frame
class NavigationFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)
        center_frame = tk.Frame(self, bg=self["bg"])
        center_frame.place(relx=0.5, rely=0.30, anchor="center")
        tk.Label(center_frame, text="Navigation", **self.title_style).pack(pady=(10, 20))

        button_style = {
            "bg": "#E3C376", "fg": "black",
            "width": 30, "height": 2,
            "bd": 1, "relief": "solid",
            "highlightbackground": "black",
            "font": ("Arial", 10, "bold"),
            "activebackground": "#d2b660"
        }

        tk.Button(center_frame, text="Design Optimal BS", command=lambda: self.switch_to("FindOptimalBsNavFrame"), **button_style).pack(pady=5)
        tk.Button(center_frame, text="Check Existing BS Designs", command=lambda: self.switch_to("CheckExistingBSNavFrame"), **button_style).pack(pady=5)
        tk.Button(center_frame, text="Run Load Cases on Existing BS", command=lambda: self.switch_to("RunLoadCaseOnBSNavFrame"), **button_style).pack(pady=5)


# Generic Input Frame
class InputFrame(BaseFrame):
    def __init__(self, parent, controller, fields, frame_name, data_group_name):
        super().__init__(parent, controller)
        self.fields = fields
        self.frame_name = frame_name
        self.data_group_name = data_group_name  # Grouping name
        self.entries = {}
        
        for field in fields:
            tk.Label(self, text=f"Enter {field}:", **self.inputFrame_style).pack()
            entry = tk.Entry(self)
            entry.pack()
            self.entries[field] = entry

        tk.Button(self, text="Save data", command=self.save_data, **self.sub_button_style).pack(pady=5)

    def save_data(self):
        grouped_data = {field: entry.get() for field, entry in self.entries.items()}  
        self.data_store.parameters[self.data_group_name] = grouped_data  # Store under group
        self.go_back()  # Go back to previous frame
        if self.frame_name == "ProjectInfoFrame":
            banner_labels = self.controller.banner_entries
            for key in grouped_data:
                if key.lower() in banner_labels:
                    banner_labels[key.lower()].config(text=grouped_data[key])

# Updated PlotFrame to handle normal and abnormal data
class PlotFrame(BaseFrame):
    def __init__(self, parent, controller, title, data_key):
        super().__init__(parent, controller)
        self.data_key = data_key  # Unique key for storing data in DataStore
        
        self.input_frame = tk.Frame(self, bg="#1D6F6E", highlightbackground="black", highlightthickness=1)
        self.input_frame.pack(side="left", padx=10, pady=10)
        self.plot_frame = tk.Frame(self, bg="#1D6F6E", highlightbackground="black", highlightthickness=1)
        self.plot_frame.pack(side="right", padx=10, pady=10)

        self.normal_rows = []
        self.abnormal_rows = []

        # Normal Operation Section
        self.normal_section = tk.Frame(self.input_frame, bg="#1D6F6E")
        self.normal_section.pack(pady=10)
        tk.Label(self.normal_section, text="Normal Operation", **self.label_plot_style).pack()
        tk.Button(self.normal_section, text="Add Row (Normal)", command=self.add_normal_row, **self.sub_button_style).pack()

        # Abnormal Operation Section
        self.abnormal_section = tk.Frame(self.input_frame, bg="#1D6F6E")
        self.abnormal_section.pack(pady=10)
        tk.Label(self.abnormal_section, text="Abnormal Operation", **self.label_plot_style).pack()
        tk.Button(self.abnormal_section, text="Add Row (Abnormal)", command=self.add_abnormal_row, **self.sub_button_style).pack()
        tk.Button(self.input_frame, text="Plot Data", command=self.update_plot, **self.sub_button_style).pack(pady=10)
        self.figure, self.ax = plt.subplots(figsize=(6, 4))
        self.ax.set_title(title)
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)
        self.canvas = FigureCanvasTkAgg(self.figure, self.plot_frame)
        self.canvas.get_tk_widget().pack()

    def add_normal_row(self):
        frame = tk.Frame(self.normal_section)
        frame.pack()
        curvature_entry = tk.Entry(frame, width=10)
        tension_entry = tk.Entry(frame, width=10)
        curvature_entry.pack(side="left")
        tension_entry.pack(side="left")
        self.normal_rows.append((curvature_entry, tension_entry))

    def add_abnormal_row(self):
        frame = tk.Frame(self.abnormal_section)
        frame.pack()
        curvature_entry = tk.Entry(frame, width=10)
        tension_entry = tk.Entry(frame, width=10)
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
            self.ax.plot(normal_curvature, normal_tension, marker='o', label="Normal", color="#E3C376")
        if abnormal_curvature and abnormal_tension:
            self.ax.plot(abnormal_curvature, abnormal_tension, marker='s', label="Abnormal", color="#1D6F6E")

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
        fields = ["Project name", "client", "designer name"]
        super().__init__(parent, controller, fields, "ProjectInfoFrame", "project_info")
class RiserInfoFrame(InputFrame):
    def __init__(self, parent, controller):
        fields = ["riser identification", "outer diameter", "outer diameter tolerance", "mass per unit length", "axial stiffness", "bending stiffness", "torsial stiffness", "riser length"]
        super().__init__(parent, controller, fields, "RiserInfoFrame", "riser_info")
class BSDimensionsFrame(InputFrame):
    def __init__(self, parent, controller):
        fields = ["root length", "tip length", "min root outer diameter", "max root outer diameter", "min overall length", "max overall length", "clearance ", "increment width", "increment length"]
        super().__init__(parent, controller, fields, "BSDimensionsFrame", "bs_dimension")
class createNewMaterialFrame:
    def __init__(self, parent, on_save=None):
        self.top = Toplevel(parent)
        self.top.title("Create New Material")
        self.top.geometry("500x600")
        self.top.configure(bg="#E3DFCF")
        self.top.iconbitmap("ultrabend_proposed_logo.ico")

        self.on_save = on_save
        self.entries = {}
        self.dynamic_entries = []

        # === Reusable styles ===
        label_style = {
            "bg": "#E3DFCF",
            "fg": "black",
            "font": ("Arial", 10, "bold")
        }
        button_style = {
            "bg": "#E3C376",
            "fg": "black",
            "bd": 1,
            "relief": "solid",
            "highlightbackground": "black",
            "activebackground": "#d2b660",
            "font": ("Arial", 10, "bold"),
            "padx": 10,
            "pady": 5
        }

        # === Static Fields ===
        tk.Label(self.top, text="Material identifier", **label_style).pack(pady=(10, 3))
        self.entries["Material identifier "] = tk.Entry(self.top)
        self.entries["Material identifier "] .pack(pady=5)

        tk.Label(self.top, text="Number of datapoints in stress/strain table", **label_style).pack(pady=(10, 3))
        self.entries["number of datapoints in stress/strain table"] = tk.Entry(self.top)
        self.entries["number of datapoints in stress/strain table"].pack(pady=5)

        self.generate_button = tk.Button(self.top, text="Generate Fields", command=self.generate_datapoint_fields, **button_style)
        self.generate_button.pack(pady=10)

        # === Scrollable canvas ===
        self.canvas = Canvas(self.top, height=300, bg="#E3DFCF", highlightthickness=0, bd=0)
        self.scrollable_frame = Frame(self.canvas, bg="#E3DFCF")
        self.scrollbar = Scrollbar(self.top, orient="vertical", command=self.canvas.yview)
        self.canvas.configure(yscrollcommand=self.scrollbar.set)

        self.canvas.pack(side="left", fill="both", expand=True)
        self.scrollbar.pack(side="right", fill="y")
        self.canvas.create_window((0, 0), window=self.scrollable_frame, anchor="nw")

        self.scrollable_frame.bind(
            "<Configure>", lambda e: self.canvas.configure(scrollregion=self.canvas.bbox("all"))
        )

        # === Submit Button ===
        self.submit_button = tk.Button(self.top, text="Submit", command=self.collect_material_data, **button_style)
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
        if self.on_save:
            self.on_save()
        self.top.destroy()

class SelectMaterialFrame(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)

        tk.Label(self, text="Select Material", **self.title_style).pack(pady=10)
        self.materials = self.load_materials()
        self.selected_material = tk.StringVar()
        self.controller = controller

        self.dropdown = ttk.Combobox(self, textvariable=self.selected_material, values=list(self.materials.keys()))
        self.dropdown.pack(pady=5)

        tk.Button(self, text="Load Material", command=self.load_selected_material, **self.button_style).pack(pady=10)
        tk.Button(self, text="Create New Material", command=self.create_new_material, **self.button_style).pack(pady=5)

    def create_new_material(self):
        """Opens a new window to create a new material."""
        self.new_material_window = createNewMaterialFrame(self, on_save=self.refresh_dropdown)

    def load_materials(self):
        db_path = "materials/material_database.json"
        if os.path.exists(db_path):
            with open(db_path, "r") as file:
                return json.load(file)
        else:
            messagebox.showerror("Error", "Material database not found.")
            return {}
        return {}

    def refresh_dropdown(self):
        self.materials = self.load_materials()
        self.dropdown['values'] = list(self.materials.keys())

    def load_selected_material(self):
        self.generate_material_text_block()
        material_name = self.selected_material.get()
        if material_name in self.materials:
            self.data_store.set_parameter("selected_material", self.materials[material_name])
            self.data_store.set_parameter("material_identifier", material_name)
            messagebox.showinfo("Success", f"Material '{material_name}' loaded successfully!")
        else:
            messagebox.showerror("Error", "Please select a valid material.")

    def generate_material_text_block(self):
        material_name = self.selected_material.get()
        if material_name not in self.materials:
            messagebox.showerror("Error", "Please select a valid material.")
            return

        material = self.materials[material_name]
        nmat = material["NMAT"]
        data = material["data"]

        lines = []
        lines.append("MATERIAL DATA")
        lines.append("' Material identifier")
        lines.append(material_name)
        lines.append("'NMAT - Number of points in stress/strain table for BS material")
        lines.append(str(nmat))
        lines.append("' strain   stress (kPa)    - Nmat input lines")

        for pair in data:
            strain = f"{pair['strain']:.3f}"
            stress = f"{pair['stress']:.1f}"
            lines.append(f"{strain}   {stress}")

        material_text = "\n".join(lines)

        # Store the formatted text in the DataStore for input file generation
        self.data_store.set_parameter("material_text_block", material_text)
        messagebox.showinfo("Material Block Generated", "Formatted material block saved in DataStore.")


class BSDimensionsFrameMulti(BaseFrame):
    def __init__(self, parent, controller):
        super().__init__(parent, controller)

        self.controller = controller
        self.num_bs = tk.IntVar(value=1)  # Default 1 BS entry
        self.entries = []

        tk.Label(self, text="Enter number of BS:").pack(pady=5)
        tk.Entry(self, textvariable=self.num_bs, width=5).pack(pady=5)

        tk.Button(self, text="Generate Fields", command=self.generate_fields, **self.button_style).pack(pady=5)

        self.input_frame = tk.Frame(self)
        self.input_frame.pack(pady=10)

        tk.Button(self, text="Save Data", command=self.save_data, **self.button_style).pack(pady=5)
        tk.Button(self, text="Generate Cases", command=lambda:DataProcessor().generate_case_files_multi_BS_btn(),**self.button_style).pack(pady=5)
        tk.Button(self, text="Run Analysis", command=lambda:DataProcessor().multithreadedAnalysis(), **self.button_style).pack(pady=5)
        

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
        tk.Label(self, text="Run Analysis", **self.title_style).pack(pady=10)

        # Progress Bar
        self.progress = ttk.Progressbar(self, orient="horizontal", length=300, mode="determinate")
        self.progress.pack(pady=10)

        # Buttons
        tk.Button(self, text="Refresh", command=self.update_labels, **self.button_style).pack(pady=5)
        tk.Button(self, text="Generate Cases", command=self.data_processor.casesBtn, **self.button_style).pack(pady=5)
        tk.Button(self, text="Run Analysis", command=self.run_analysis, **self.button_style).pack(pady=5)
        tk.Button(self, text="Save Data", command=self.data_store.save_data, **self.button_style).pack(pady=5)
        tk.Button(self, text="Load Data", command=self.load_data, **self.button_style).pack(pady=5)
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
        self.table_frame = tk.Frame(self.main_frame)
        self.table_frame.pack(side="left", fill="both", expand=True, padx=10, pady=10)
        self.create_tkinter_table(self.table_frame)

        # Plot Frame
        self.plot_frame = tk.Frame(self.main_frame)
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
    back_btn = tk.Button(
        banner, text="BACK", bg="black", fg="white",
        bd=1, relief="solid", highlightbackground="black",
        command=(lambda: controller.go_back()) if controller else (lambda: print("Back")))
    back_btn.place(x=135, y=5, width=87, height=26)

    # Divider
    divider = tk.Frame(banner, bg="black", width=2, height=28)
    divider.place(x=220 + 10, y=5)

    # Entry fields with placeholders
    entries = {}
    labels = ["project name", "client", "designer name"]
    start_x = 230 + 10
    spacing = 222

    for i, key in enumerate(labels):
        x = start_x + i * spacing
        entry = tk.Label(banner, bg="black", fg="white",
                        text=f"{key.title()}: ", anchor="w", font=("Arial", 9))
        entry.place(x=x, y=5, width=212, height=26)
        entries[key] = entry

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
            go_back_callback=lambda: self.show_frame("go_back"),
            menu_callback=lambda: self.show_frame("NavigationFrame")
        )

        self.banner_prefixes = {
            "project name": "Project Name: ",
            "client": "Client: ",
            "designer": "Designer: "
        }

        # Container for the pages (goes below the banner)
        self.container = tk.Frame(self.main_frame, width=1080, height=720, bg="#E3DFCF")
        self.container.pack(fill="both", expand=True)

        self.frames = {}
        self.current_frame = None

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
            self.container.grid_rowconfigure(0, weight=1)
            self.container.grid_columnconfigure(0, weight=1)


        self.show_frame("NavigationFrame")
        def update_banner_info(self):
            project_info = DataStore().get_parameter("project_info")

            if "project name" in self.banner_entries:
                self.banner_entries["project name"].config(
                    text=f"Project Name: {project_info.get('Project Name', '')}"
                )
            if "client" in self.banner_entries:
                self.banner_entries["client"].config(
                    text=f"Client: {project_info.get('client', '')}"
                )
            if "designer" in self.banner_entries:
                self.banner_entries["designer"].config(
                    text=f"Designer: {project_info.get('designer name', '')}"
                )
    def go_back(self):
        if self.current_frame:
            current = self.frames[self.current_frame]
            if hasattr(current, "previous_frame"):
                self.show_frame(current.previous_frame)

    def show_frame(self, frame_name):
        if self.current_frame:
            self.frames[frame_name].set_previous_frame(self.current_frame)
        self.frames[frame_name].tkraise()
        self.current_frame = frame_name

if __name__ == "__main__":
    app = App()
    app.mainloop()
    app.destroy()  # Close the app properly
