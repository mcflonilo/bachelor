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


def print_data(groups):
    for case_group, group in groups:
        print(f"Case Group: {case_group}")
        print(group)


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


def find_shortest_valid_result(group, case_group):
    """Find the shortest valid result for each case group and log the search path."""
    case_group_data = groups.get_group(case_group)
    case_2d_array = redefine_group_as_2d_array(case_group_data)
    min_width, max_width, min_length, max_length, width_increment, length_increment = 0.5, 2, 8, 17, 0.025, 0.25
    threshold = thresholds[case_group]['maximum_curvature']

    best_result = None
    current_width = min_width
    current_length = max_length

    # List to store checked values
    search_log = []

    # Check top left corner
    search_log.append([case_group, min_width, min_length, case_2d_array[min_width][min_length]['maximum_curvature'], "Checked"])
    if case_2d_array[min_width][min_length]['maximum_curvature'] <= threshold:
        print("first result is valid (top left). case should be dismissed")
        save_search_log(case_group, search_log)
        return case_2d_array[min_width][min_length]

    # Check bottom left corner
    search_log.append([case_group, max_width, min_length, case_2d_array[max_width][min_length]['maximum_curvature'], "Checked"])
    if case_2d_array[max_width][min_length]['maximum_curvature'] <= threshold:
        print("max width min length valid (bottom left). case should be dismissed")
        save_search_log(case_group, search_log)
        return case_2d_array[max_width][min_length]

    # Check top right corner
    search_log.append([case_group, max_width, max_length, case_2d_array[max_width][max_length]['maximum_curvature'], "Checked"])
    if case_2d_array[max_width][max_length]['maximum_curvature'] > threshold:
        print("max width max length invalid (top right). case should be dismissed")
        save_search_log(case_group, search_log)
        return None
    

    while current_width <= max_width and current_length >= min_length:
        rounded_width = round(current_width, 5)
        rounded_length = round(current_length, 5)

        if rounded_width in case_2d_array and rounded_length in case_2d_array[rounded_width]:
            row = case_2d_array[rounded_width][rounded_length]
            curvature = row['maximum_curvature']

            # Log each checked entry
            search_log.append([case_group, rounded_width, rounded_length, curvature, "Checked"])

            if curvature <= threshold:
                best_result = row
                current_length -= length_increment
            else:
                current_width += width_increment
        else:
            current_width += width_increment

    save_search_log(case_group, search_log)
    return best_result

def save_results_to_excel(csv_file, excel_file, thresholds):
    """Save the results to an Excel file and apply conditional formatting."""
    data = pd.read_csv(csv_file)
    writer = pd.ExcelWriter(excel_file, engine='openpyxl')
    data.to_excel(writer, index=False, sheet_name='Results')

    workbook = writer.book
    sheet = writer.sheets['Results']

    green_fill = PatternFill(start_color='00FF00', end_color='00FF00', fill_type='solid')
    red_fill = PatternFill(start_color='FF0000', end_color='FF0000', fill_type='solid')

    for _, row in data.iterrows():
        row_idx = row.name + 2  # Adjust for header row
        cell = sheet.cell(row=row_idx, column=4, value=row['Curvature'])

        # Apply formatting
        case_group = row['Case Group']
        if case_group in thresholds:
            threshold = thresholds[case_group]
            if row['Curvature'] <= threshold['maximum_curvature']:
                cell.fill = green_fill
            else:
                cell.fill = red_fill

    # Ensure at least one sheet is visible
    if not workbook.sheetnames:
        workbook.create_sheet(title='Sheet1')

    # Save the Excel file
    writer.close()
    

# Load the CSV file
file_path = 'bsengine-summary.csv'
data = pd.read_csv(file_path)

# Extract the case name, width, and length from the 'case_name' column
data['case_group'] = data['case_name'].apply(lambda x: x.split('-')[0])
data['width'] = data['case_name'].apply(lambda x: float(x.split('-')[1]))
data['length'] = data['case_name'].apply(lambda x: float(x.split('-')[2]))


# Separate the data into groups based on the extracted case name
groups = data.groupby('case_group')

groups = data.groupby('case_group')

# Run the search for each case
for case in thresholds.keys():
    shortest_valid_result = find_shortest_valid_result(groups.get_group(case), case)
    print(f"Shortest valid result for {case}: {shortest_valid_result}\n")

save_results_to_excel('optimized_search_log.csv', 'optimized_search_log.xlsx', thresholds)