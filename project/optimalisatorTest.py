import os
import subprocess
from queue import Queue
import threading
import time
import re
import csv
import pandas as pd
from openpyxl.styles import PatternFill
from openpyxl import load_workbook, Workbook
import tkinter as tk
import logging
from maxcurve_module import DataPlotter


thresholds = {
    "case_files\\Case1": {"maximum_curvature":0.055835668},
    "case_files\\Case2": {"maximum_curvature": 0.051167134},
    "case_files\\Case3": {"maximum_curvature": 0.046473762},
    "case_files\\Case4": {"maximum_curvature": 0.041759232},
    "case_files\\Case5": {"maximum_curvature": 0.034737267},
    "case_files\\Case6": {"maximum_curvature": 0.032398017},
    "case_files\\Case7": {"maximum_curvature": 0.030050334},
}
min_width, max_width, min_length, max_length, width_increment, length_increment = 0.5, 8, 0.5, 8, 0.025, 0.25
min_width, max_width, min_length, max_length, width_increment, length_increment = 1.4, 1.5, 10, 11, 0.05, 0.5


def print_data(groups):
    for case_group, group in groups:
        print(f"Case Group: {case_group}")
        print(group)

def extract_key_results(case_file):
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

def save_search_log(case_group, log_data):
    """Save the search log to a CSV file, appending to it if it already exists."""
    file_name = f"optimized_search_log.csv"
    df = pd.DataFrame(log_data, columns=["Case Group", "Width", "Length", "Curvature", "Status"])
    
    # Append to the file if it exists, otherwise create it
    if os.path.isfile(file_name):
        df.to_csv(file_name, mode='a', header=False, index=False)
    else:
        df.to_csv(file_name, index=False)
    
    print(f"Search log saved to {file_name}")

def redefine_group_as_2d_array(group):
    """Redefine the group as a 2D dictionary where width is the first key and length is the second key."""
    width_length_dict = {}
    for index, row in group.iterrows():
        width = round(row['width'], 5)  # Round to 5 decimal places
        length = round(row['length'], 5)
        if width not in width_length_dict:
            width_length_dict[width] = {}
        width_length_dict[width][length] = row
    return width_length_dict

def runBSEngine(case):
        current_directory = os.getcwd()
        process = subprocess.Popen(f".\\bsengine -b {case}", shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, cwd=current_directory)
        stdout, stderr = process.communicate()
        print(f"stdout: {stdout}")
        print(f"stderr: {stderr}")
        if process.returncode != 0:
            print(f"Error encountered with case: {case}. FIKS AT DEN KJØRES PÅ NYTT PÅ EN ELLER ANNEN MÅTE DITT NEK.")
        
        case_file = case.strip().replace('.inp', '.log')
        result = extract_key_results(case_file)
        result = float(result['maximum_curvature'])
        return result

def check_min_length_column(group, case_group, groups):
    case_group_data = groups.get_group(case_group)
    case_2d_array = redefine_group_as_2d_array(case_group_data)
    threshold = thresholds[case_group]['maximum_curvature']

    current_width = min_width
    current_length = min_length

    while current_width <= max_width:
        rounded_length = round(current_length, 5)

        if min_width in case_2d_array and rounded_length in case_2d_array[min_width]:
            curvature = runBSEngine(case_2d_array[min_width][rounded_length]["case_name"])

            if curvature <= threshold:
                print("valid bs found in min length, case should be dismissed")
            else:
                current_width += width_increment

    # List to store checked values
    search_log = []




def find_shortest_valid_result(group, case_group, groups):
    """Find the shortest valid result for each case group and log the search path."""
    case_group_data = groups.get_group(case_group)
    case_2d_array = redefine_group_as_2d_array(case_group_data)
    threshold = thresholds[case_group]['maximum_curvature']

    best_result = None
    current_width = min_width
    current_length = max_length

    # List to store checked values
    search_log = []

    # Check top left corner
    search_log.append([case_group, min_width, min_length, case_2d_array[min_width][min_length]['case_name'], "Checked"])
    if runBSEngine(case_2d_array[min_width][min_length]['case_name']) <= threshold:
        print("first result is valid (top left). case should be dismissed")
        save_search_log(case_group, search_log)
        return case_2d_array[min_width][min_length]

    # Check bottom left corner
    search_log.append([case_group, max_width, min_length, case_2d_array[max_width][min_length]['case_name'], "Checked"])
    if runBSEngine(case_2d_array[max_width][min_length]['case_name']) <= threshold:
        print("max width min length valid (bottom left). case should be dismissed")
        save_search_log(case_group, search_log)
        return case_2d_array[max_width][min_length]

    # Check top right corner
    search_log.append([case_group, max_width, max_length, case_2d_array[max_width][max_length]['case_name'], "Checked"])
    if runBSEngine(case_2d_array[max_width][max_length]['case_name']) > threshold:
        print("max width max length valid (top right). case should be dismissed")
        save_search_log(case_group, search_log)
        return None
    

    while current_width <= max_width and current_length >= min_length:
        rounded_width = round(current_width, 5)
        rounded_length = round(current_length, 5)

        if rounded_width in case_2d_array and rounded_length in case_2d_array[rounded_width]:
            curvature = runBSEngine(case_2d_array[rounded_width][rounded_length]["case_name"])

            # Log each checked entry
            search_log.append([case_group, rounded_width, rounded_length, curvature, "Checked"])

            if curvature <= threshold:
                best_result = case_2d_array[rounded_width][rounded_length]
                current_length -= length_increment
            else:
                current_width += width_increment
        else:
            current_width += width_increment

    save_search_log(case_group, search_log)
    return best_result

def test_BS_against_all_cases(casegroupname,width,length, groups):
    for casegroup in groups:
        if casegroup == casegroupname:
            pass
        else:
            print(f"Testing casegroup {casegroup["case_group"]}-{width}-{length}")
            #runBSEngine(groups.get_group(casegroup).iloc[0]["case_name"])
        print(f"Testing casegroup {casegroup}")



def loadBSCases():
    """loads all the bsengine cases and splits tyhem into groups and extracts the width and length from the case name"""
    file_path = 'bsengine-cases.txt'
    cases = []
    with open(file_path, 'r') as file:
        cases = file.readlines()

    data = []
    for case in cases:
        case_name = case.strip()
        case_group = case_name.split('-')[0]
        width = float(case_name.split('-')[1])
        length = float(case_name.split('-')[2])
        data.append({"case_name": case_name, "case_group": case_group, "width": width, "length": length})

    df = pd.DataFrame(data)

    # Separate the data into groups based on the extracted case name
    groups = df.groupby('case_group')

    test_BS_against_all_cases("Case1",1.4,10,groups)
    #for case in thresholds.keys():
     #   shortest_valid_result = find_shortest_valid_result(groups.get_group(case), case, groups)
      #  print(f"Shortest valid result for {case}: {shortest_valid_result}\n")

loadBSCases()


"""


# Load the CSV file
file_path = 'bsengine-summary.csv'
data = pd.read_csv(file_path)

# Extract the case name, width, and length from the 'case_name' column
data['case_group'] = data['case_name'].apply(lambda x: x.split('-')[0])
data['width'] = data['case_name'].apply(lambda x: float(x.split('-')[1]))
data['length'] = data['case_name'].apply(lambda x: float(x.split('-')[2]))


# Separate the data into groups based on the extracted case name
groups = data.groupby('case_group')

# Run the search for each case
for case in thresholds.keys():
    shortest_valid_result = find_shortest_valid_result(groups.get_group(case), case)
    print(f"Shortest valid result for {case}: {shortest_valid_result}\n")

"""

