import tkinter as tk
import os
import matplotlib.pyplot as plt
from matplotlib.figure import Figure
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import numpy as np
from scipy.interpolate import interp1d
import pandas as pd
import re
import subprocess
import os
from modules.reportScreen_module import ReportScreen
import sys

thresholds_normal = {}
thresholds_abnormal = {}


def switch_frame(frame):
    frame.tkraise()

class AnalysisScreen:
    def __init__(self, root, prev_frame, data, show_frame):
        self.root = root
        self.prev_frame = prev_frame
        self.data = data
        self.show_frame = show_frame
        project_info = data.get("project_info", {})
        riser_info = data.get("riser_info", {})
        riser_capacities = data.get("riser_capacities", {})
        riser_response = data.get("riser_response", {})
        bs_dimensions = data.get("bs_dimension", {})
        bs_materials = data.get("bs_material", {})

        self.normal_max_curve, self.abnormal_max_curve = self.interpolate_max_curve(riser_capacities, riser_response)
        self.length = round(float(riser_info.get("Riser Length")), 3)
        self.EA = round(float(riser_info.get("Axial Stiffness")), 3)
        self.EI = round(float(riser_info.get("Bending Stiffness")), 3)
        self.GT = round(float(riser_info.get("Torsial Stiffness")), 3)
        self.m = round(float(riser_info.get("Mass Per Unit Length")), 3)
        self.ID = round(float(riser_info.get("Outer Diameter")) + (2 * float(riser_info.get("Outer Diameter Tolerance"))) , 3)
        self.SL = round(0.700, 3)
        
        self.TOD = round(self.ID + 0.04, 3)  # DETTE MÅ DOBBELSJEKKES FORDI JEG VET IKKE
        self.MAT = ["NOLIN 60D_30"]
        self.MATID = ["60D_30"]
        self.normal_cases, self.abnormal_cases = self.make_case_arrays(riser_response)

        for i, value in enumerate(self.normal_max_curve):
            thresholds_normal[f"case_files\\Case-normal{i+1}"] = {"maximum_curvature": value}
        for i, value in enumerate(self.abnormal_max_curve):
            thresholds_abnormal[f"case_files\\Case-abnormal{i+1}"] = {"maximum_curvature": value}


        
        # Add buttons at the bottom
        btn_frame = tk.Frame(self.root)
        btn_frame.grid(row=0, column=3, columnspan=2, pady=10, padx=10, sticky="ew")
        
        btn_return = tk.Button(btn_frame, text="Return", width=15, bg="#333", fg="white",
                               command=lambda: show_frame(prev_frame))
        btn_return.grid(row=0, column=0, pady=10)

        btn_create_case_files = tk.Button(btn_frame, text="Generate Cases", width=15, bg="#333", fg="white",
                                          command=self.casesBtn)
        btn_create_case_files.grid(row=1, column=0, pady=10)

        btn_run_analysis = tk.Button(btn_frame, text="Run Analysis", width=15, bg="#333", fg="white",
                                     command=lambda: self.loadBSCases("bsengine-cases-normal.txt"))
        btn_run_analysis.grid(row=2, column=0, pady=10)


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
    def casesBtn(self):
        casetotal = 0
        casetotal += self.generate_case_files(self.length, self.EA, self.EI, self.GT, self.m, self.normal_cases, self.ID, self.SL, self.OD, self.CL, self.TL, self.TOD, self.MAT, self.MATID, "normal")
        casetotal += self.generate_case_files(self.length, self.EA, self.EI, self.GT, self.m, self.abnormal_cases, self.ID, self.SL, self.OD, self.CL, self.TL, self.TOD, self.MAT, self.MATID, "abnormal")
        tk.messagebox.showinfo("Case Files", f"generated {casetotal} cases.")
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
    
    def cl_od_to_array(self, min_cl, max_cl, min_od, max_od, incrementSize_fieldLength, incrementSize_fieldWidth):
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

        return cl_array, od_array

    def print_data(self, groups):
        for case_group, group in groups:
            print(f"Case Group: {case_group}")
            print(group)

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

            executable_path = resource_path('bsengine\\bsengine')
            def checkIfCaseIsAlreadyChecked(case):
                casename = case.replace('.inp', '_FEA.log')
                if os.path.exists(casename):
                    print(f"bs already checked for case: {casename}. skipping!")
                    return True
                return False
            current_directory = os.getcwd()
            
            print(f"Running case: {case}")

            if checkIfCaseIsAlreadyChecked(case):
                case_file = case.strip().replace('.inp', '.log')
                result = self.extract_key_results(case_file)
                if result is None:
                    print(f"Error extracting key results for case: {case}")
                else:
                    result = float(result['maximum_curvature'])
                    return result
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
        threshold = thresholds_normal[case_group]['maximum_curvature']

        best_result = None
        current_width = self.min_width
        current_length = self.max_length

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
        findShortestBSForCase = self.findShortestBSForCase(file_path)
        print("findShortestBSForCase: ", findShortestBSForCase)
        self.checkBSAgainsAllCases(findShortestBSForCase, "abnormal", 1)
        print("verifying result")
        self.checkBSAgainsAllCases(findShortestBSForCase, "normal", 1)
        # Create riserResponse frame
        report_frame = tk.Frame(self.root)
        report_frame.grid(row=0, column=0, sticky="nsew")
        self.report_app = ReportScreen(report_frame, self.prev_frame, self.show_frame, self.data['riser_capacities'], self.data['riser_response'],findShortestBSForCase["case_name"], self.data, thresholds_normal, thresholds_abnormal)
        self.show_frame(report_frame)
    
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

        self.print_data(groups)
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
                print(f"bs {f'case_files\\Case-normal{i}'} passed in all cases")
                return shortest

    def checkBSAgainsAllCases(self, shortest,tag, i):
        if tag == "normal":
            thresholds = thresholds_normal
        else:
            thresholds = thresholds_abnormal
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
    app = AnalysisScreen(root, prev_frame, data)
    root.mainloop()

if __name__ == "__main__":
    main()