import tkinter as tk
from tkinter import ttk
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg

class RiserResponseWindow:
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
        ttk.Label(self.input_frame, text="Abnormal Operation").grid(row=20, column=0, columnspan=3, pady=5)
        ttk.Label(self.input_frame, text="Curvature [1/m]").grid(row=21, column=0)
        ttk.Label(self.input_frame, text="Tension [kN]").grid(row=21, column=1)

        # Add "Add Row" buttons
        ttk.Button(self.input_frame, text="Add Row (Normal)", command=self.add_normal_row).grid(row=2, column=2, pady=5)
        ttk.Button(self.input_frame, text="Add Row (Abnormal)", command=self.add_abnormal_row).grid(row=22, column=2, pady=5)

        # Autofill button
        ttk.Button(self.input_frame, text="Autofill Data", command=self.autofill_data).grid(row=40, column=0, columnspan=3, pady=10)

        # Interpolation section
        ttk.Label(self.input_frame, text="Interpolation").grid(row=50, column=0, columnspan=3, pady=10)
        ttk.Label(self.input_frame, text="Input Tension [kN]").grid(row=51, column=0, padx=5)
        self.interpolation_rows_frame = ttk.Frame(self.input_frame)
        self.interpolation_rows_frame.grid(row=52, column=0, columnspan=3)

        # Add initial interpolation field
        self.add_interpolation_field()

        ttk.Button(self.input_frame, text="Add Interpolation Field", command=self.add_interpolation_field).grid(row=53, column=0, columnspan=3, pady=5)
        ttk.Button(self.input_frame, text="Interpolate & Plot", command=self.interpolate_and_plot).grid(row=54, column=0, columnspan=3, pady=5)

        # Plot button
        ttk.Button(self.input_frame, text="Plot Data", command=self.update_plot).grid(row=60, column=0, columnspan=3, pady=10)

        # back button
        ttk.Button(self.input_frame, text="Back", command=lambda: prev_frame.tkraise()).grid(row=70, column=0, columnspan=3, pady=10)

        # Matplotlib figure
        self.figure, self.ax = plt.subplots(figsize=(6, 4))
        self.ax.set_title("Tension vs. Curvature")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)
        self.canvas = FigureCanvasTkAgg(self.figure, self.plot_frame)
        self.canvas.get_tk_widget().grid(row=0, column=0)

        # Add initial rows
        for _ in range(7):
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
        row_index = len(self.abnormal_rows) + 23
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

    def interpolate_and_plot(self):
        """Interpolate curvature based on tension and plot the results."""
        tensions = []
        for entry in self.interpolation_rows:
            try:
                tensions.append(float(entry.get()))
            except ValueError:
                continue

        curvature_normal, tension_normal = self.get_data_from_entries(self.normal_rows)
        curvature_abnormal, tension_abnormal = self.get_data_from_entries(self.abnormal_rows)

        self.ax.clear()
        self.ax.set_title("Tension vs. Curvature")
        self.ax.set_xlabel("Curvature [1/m]")
        self.ax.set_ylabel("Tension [kN]")
        self.ax.grid(True)

        normal_sorted = sorted(zip(tension_normal, curvature_normal))
        abnormal_sorted = sorted(zip(tension_abnormal, curvature_abnormal))

        tension_normal, curvature_normal = zip(*normal_sorted) if normal_sorted else ([], [])
        tension_abnormal, curvature_abnormal = zip(*abnormal_sorted) if abnormal_sorted else ([], [])

        self.ax.plot(curvature_normal, tension_normal, label="Normal Operation", color='blue')
        self.ax.plot(curvature_abnormal, tension_abnormal, label="Abnormal Operation", color='red')

        for tension in tensions:
            if tension_normal:
                interpolated_normal = np.interp(tension, tension_normal, curvature_normal)
                self.ax.scatter(interpolated_normal, tension, color='blue', zorder=5)
            if tension_abnormal:
                interpolated_abnormal = np.interp(tension, tension_abnormal, curvature_abnormal)
                self.ax.scatter(interpolated_abnormal, tension, color='red', zorder=5)

        self.ax.legend()
        self.canvas.draw()
    def get_data(self):
        """Return all data from the input fields."""
        normal_data = self.get_data_from_entries(self.normal_rows)
        abnormal_data = self.get_data_from_entries(self.abnormal_rows)
        return {
            "normal": normal_data,
            "abnormal": abnormal_data
        }

    def set_data(self, data):
        """Set data to the input fields."""
        normal_angles = data.get("normal", [[], []])[0]
        normal_tensions = data.get("normal", [[], []])[1]
        abnormal_angles = data.get("abnormal", [[], []])[0]
        abnormal_tensions = data.get("abnormal", [[], []])[1]

        for row, angle, tension in zip(self.normal_rows, normal_angles, normal_tensions):
            row[0].delete(0, tk.END)
            row[0].insert(0, angle)
            row[1].delete(0, tk.END)
            row[1].insert(0, tension)

        for row, angle, tension in zip(self.abnormal_rows, abnormal_angles, abnormal_tensions):
            row[0].delete(0, tk.END)
            row[0].insert(0, angle)
            row[1].delete(0, tk.END)
            row[1].insert(0, tension)

    def autofill_data(self):
        """Autofill the input fields with predefined data."""
        normal_data = [
            (4.0, 550), (18.0, 650), (23.0, 750), (23.0, 850), (16.0, 1000),
            (10.0, 1050), (2.0, 1100)
        ]
        abnormal_data = [
            (15.0, 470), (20.0, 490), (25.0, 700), (26.0, 800), (25.0, 930),
            (20.0, 1020), (5.0, 1140)
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


def main():
    root = tk.Tk()
    root.geometry("1200x700")
    main_frame = tk.Frame(root)
    main_frame.pack(fill="both", expand=True)
    app = RiserResponseWindow(main_frame, None)
    root.mainloop()


if __name__ == "__main__":
    main()
