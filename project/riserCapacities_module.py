import tkinter as tk
from tkinter import ttk
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg

class riserCapacities:
    def __init__(self, frame, prev_frame):
        self.frame = frame

        # Create frames
        self.input_frame = ttk.Frame(self.frame)
        self.input_frame.grid(row=0, column=0, padx=10, pady=10, sticky="nsew")
        self.plot_frame = ttk.Frame(self.frame)
        self.plot_frame.grid(row=0, column=1, padx=10, pady=10, sticky="nsew")

        # Initialize rows for normal and abnormal operations
        self.normal_rows = []
        self.abnormal_rows = []
        self.interpolation_rows = []

        # Create headers for Normal Operation
        ttk.Label(self.input_frame, text="Normal Operation").grid(row=0, column=0, columnspan=3, pady=5)
        ttk.Label(self.input_frame, text="Curvature [1/m]").grid(row=1, column=0)
        ttk.Label(self.input_frame, text="Tension [kN]").grid(row=1, column=1)

        # Create headers for Abnormal Operation
        ttk.Label(self.input_frame, text="Abnormal Operation").grid(row=100, column=0, columnspan=3, pady=5)
        ttk.Label(self.input_frame, text="Curvature [1/m]").grid(row=101, column=0)
        ttk.Label(self.input_frame, text="Tension [kN]").grid(row=101, column=1)

        # Add "Add Row" buttons
        ttk.Button(self.input_frame, text="Add Row (Normal)", command=self.add_normal_row).grid(row=2, column=2, pady=5)
        ttk.Button(self.input_frame, text="Add Row (Abnormal)", command=self.add_abnormal_row).grid(row=102, column=2, pady=5)

        # Autofill button
        ttk.Button(self.input_frame, text="Autofill Data", command=self.autofill_data).grid(row=300, column=0, columnspan=3, pady=10)

        # Interpolation section
        ttk.Label(self.input_frame, text="Interpolation").grid(row=200, column=0, columnspan=3, pady=10)
        ttk.Label(self.input_frame, text="Input Tension [kN]").grid(row=201, column=0, padx=5)
        self.interpolation_rows_frame = ttk.Frame(self.input_frame)
        self.interpolation_rows_frame.grid(row=202, column=0, columnspan=3)

        # Add initial interpolation field
        self.add_interpolation_field()

        ttk.Button(self.input_frame, text="Add Interpolation Field", command=self.add_interpolation_field).grid(row=203, column=0, columnspan=3, pady=5)
        ttk.Button(self.input_frame, text="Interpolate & Plot", command=self.interpolate_and_plot).grid(row=204, column=0, columnspan=3, pady=5)

        # Plot button
        ttk.Button(self.input_frame, text="Plot Data", command=self.update_plot).grid(row=400, column=0, columnspan=3, pady=10)

        # back button
        ttk.Button(self.input_frame, text="Back", command=lambda: prev_frame.tkraise()).grid(row=500, column=0, columnspan=3, pady=10)

        # Matplotlib figure
        self.figure, self.ax = plt.subplots(figsize=(6, 4))
        self.ax.set_title("Tension vs. Curvature")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)
        self.canvas = FigureCanvasTkAgg(self.figure, self.plot_frame)
        self.canvas.get_tk_widget().grid(row=0, column=0)

        # Add initial rows
        for _ in range(10):
            self.add_normal_row()
            self.add_abnormal_row()

    def add_normal_row(self):
        """Add a new input row for Normal Operation."""
        row_index = len(self.normal_rows) + 3
        curvature_entry = ttk.Entry(self.input_frame, width=10)
        tension_entry = ttk.Entry(self.input_frame, width=10)
        curvature_entry.grid(row=row_index, column=0, padx=5, pady=2)
        tension_entry.grid(row=row_index, column=1, padx=5, pady=2)
        self.normal_rows.append((curvature_entry, tension_entry))

    def add_abnormal_row(self):
        """Add a new input row for Abnormal Operation."""
        row_index = len(self.abnormal_rows) + 103
        curvature_entry = ttk.Entry(self.input_frame, width=10)
        tension_entry = ttk.Entry(self.input_frame, width=10)
        curvature_entry.grid(row=row_index, column=0, padx=5, pady=2)
        tension_entry.grid(row=row_index, column=1, padx=5, pady=2)
        self.abnormal_rows.append((curvature_entry, tension_entry))

    def add_interpolation_field(self):
        """Add a new input field for interpolation."""
        frame = self.interpolation_rows_frame
        row_index = len(self.interpolation_rows)
        tension_entry = ttk.Entry(frame, width=10)
        tension_entry.grid(row=row_index, column=0, padx=5, pady=2)
        self.interpolation_rows.append(tension_entry)

    def get_data_from_entries(self, rows):
        """Extract data from entry rows."""
        curvature = []
        tension = []
        for curvature_entry, tension_entry in rows:
            try:
                curvature_value = float(curvature_entry.get())
                tension_value = float(tension_entry.get())
                curvature.append(curvature_value)
                tension.append(tension_value)
            except ValueError:
                continue
        return curvature, tension

    def get_tension_from_interpolation_rows(self):
        """Extract tension values from interpolation rows."""
        tensions = []
        for entry in self.interpolation_rows:
            try:
                tension_value = float(entry.get())
                tensions.append(tension_value)
            except ValueError:
                continue
        return tensions

    def interpolate_and_plot(self):
        """Interpolate curvature based on tension and plot the results."""
        tensions = self.get_tension_from_interpolation_rows()

        curvature_normal, tension_normal = self.get_data_from_entries(self.normal_rows)
        curvature_abnormal, tension_abnormal = self.get_data_from_entries(self.abnormal_rows)

        normal_sorted = sorted(zip(tension_normal, curvature_normal))
        abnormal_sorted = sorted(zip(tension_abnormal, curvature_abnormal))

        tension_normal, curvature_normal = zip(*normal_sorted) if normal_sorted else ([], [])
        tension_abnormal, curvature_abnormal = zip(*abnormal_sorted) if abnormal_sorted else ([], [])

        self.ax.scatter([], [], color='blue', label="Interpolated Normal")
        self.ax.scatter([], [], color='red', label="Interpolated Abnormal")

        for tension in tensions:
            if tension_normal:
                interpolated_normal = np.interp(tension, tension_normal, curvature_normal)
                self.ax.scatter(interpolated_normal, tension, color='blue', zorder=5)
            if tension_abnormal:
                interpolated_abnormal = np.interp(tension, tension_abnormal, curvature_abnormal)
                self.ax.scatter(interpolated_abnormal, tension, color='red', zorder=5)

        self.ax.legend()
        self.canvas.draw()

    def autofill_data(self):
        """Autofill the input fields with predefined data."""
        normal_data = [
            (0.08197, 0), (0.07752, 87), (0.07353, 174), (0.06944, 261), (0.06536, 348),
            (0.05714, 522), (0.04902, 696), (0.04082, 870), (0.03268, 1044), (0.02451, 1218),
            (0.02045, 1305), (0.01634, 1392), (0.01225, 1479), (0.00817, 1566), (0.00000, 1566)
        ]

        abnormal_data = [
            (0.11765, 0), (0.11236, 133), (0.10753, 266), (0.10204, 399), (0.09709, 532),
            (0.08696, 798), (0.07519, 1064), (0.06250, 1330), (0.05000, 1596), (0.03125, 1676), (0.00000, 1676)
        ]

        for row, (curvature, tension) in zip(self.normal_rows, normal_data):
            row[0].delete(0, tk.END)
            row[0].insert(0, curvature)
            row[1].delete(0, tk.END)
            row[1].insert(0, tension)

        for row, (curvature, tension) in zip(self.abnormal_rows, abnormal_data):
            row[0].delete(0, tk.END)
            row[0].insert(0, curvature)
            row[1].delete(0, tk.END)
            row[1].insert(0, tension)

    def update_plot(self):
        """Update the plot with the entered data."""
        curvature_normal, tension_normal = self.get_data_from_entries(self.normal_rows)
        curvature_abnormal, tension_abnormal = self.get_data_from_entries(self.abnormal_rows)

        self.ax.clear()
        self.ax.set_title("Tension vs. Curvature")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)

        if curvature_normal and tension_normal:
            self.ax.plot(curvature_normal, tension_normal, marker='o', label="Normal Operation", color='blue')
        if curvature_abnormal and tension_abnormal:
            self.ax.plot(curvature_abnormal, tension_abnormal, marker='s', label="Abnormal Operation", color='red')

        self.ax.legend()
        self.canvas.draw()
        
    def get_data_from_entries(self, rows):
        """Extract data from entry rows."""
        data = []
        for curvature_entry, tension_entry in rows:
            try:
                curvature_value = float(curvature_entry.get())
                tension_value = float(tension_entry.get())
                data.append((curvature_value, tension_value))
            except ValueError:
                continue
        return data

    def get_data(self):
        """Return all data from the input fields."""
        normal_data = self.get_data_from_entries(self.normal_rows)
        abnormal_data = self.get_data_from_entries(self.abnormal_rows)
        interpolation_data = [float(entry.get()) for entry in self.interpolation_rows if entry.get().strip() != ""]
        return {
            "normal": normal_data,
            "abnormal": abnormal_data,
            "interpolation": interpolation_data
        }
def main():
    root = tk.Tk()
    app = riserCapacities(root)
    root.mainloop()

if __name__ == "__main__":
    main()
