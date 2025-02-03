import numpy as np
from scipy.interpolate import interp1d
import matplotlib.pyplot as plt

# Data for Normal Operation
curvature = np.array([0.08197, 0.07752, 0.07353, 0.06944, 0.06536, 0.05714, 0.04902, 0.04082,
                      0.03268, 0.02451, 0.02045, 0.01634, 0.01225, 0.00817, 0.00000])
tension = np.array([0, 87, 174, 261, 348, 522, 696, 870, 1044, 1218, 1305, 1392, 1479, 1566, 1566])

# Data for Abnormal Operation
curvatureAbnormal = np.array([0.11765, 0.11236, 0.10753, 0.10204, 0.09709, 0.08696, 0.07519,
                              0.06250, 0.05000, 0.03125, 0.00000])
tensionAbnormal = np.array([0, 133, 266, 399, 532, 798, 1064, 1330, 1596, 1676, 1676])

# Create the plot
plt.figure(figsize=(10, 6))

# Plot Normal Operation
plt.plot(curvature, tension, marker='o', label='Normal Operation', color='blue')

# Plot Abnormal Operation
plt.plot(curvatureAbnormal, tensionAbnormal, marker='s', label='Abnormal Operation', color='red')

# Add labels, title, and grid
plt.title("Tension vs. Curvature (Normal and Abnormal Operation)", fontsize=16)
plt.xlabel("Curvature [1/m]", fontsize=12)
plt.ylabel("Tension [kN]", fontsize=12)
plt.grid(True)
plt.legend()

plt.show()


# Create the interpolation function
interp_function = interp1d(tension, curvature, kind='linear', fill_value="extrapolate")

# Example: Find allowable curvature for a given tension
input_tension = 1100  # Replace with desired tension value
allowable_curvature = interp_function(input_tension)
print(f"Allowable curvature for tension {input_tension} kN: {allowable_curvature:.6f} [1/m]")


# script for å hente ut max kurvatur for en gitt spenning