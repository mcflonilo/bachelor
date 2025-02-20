#-------------------------------------------------------------------------------
# Name:        make-bsengine-batch
# Purpose:     Generate a batch of bsengine.inp files for subsequemt
#              run using UberWacher Executor
# Author:      kie
#
# Created:     27/03/2018
# Copyright:   (c) Ultra Deep, LLC 2018
# Licence:     <your licence>
#-------------------------------------------------------------------------------

######################################################
# INPUT UMBILICAL DATA
length = 5.0  # length of the umbilical outside the BS
EA = 1040000  # Axial stiffness [kN]
EI = 185.0    # Bending stiffness [kNm2]
GT = 143.0    # Torsional stiffness [kNm2]
m = 88.4      # Mass per unit length [kg/m]
#####################################################

#####################################################
# INPUT LOAD CASES

cases = ['4.0  550.0', '18.0  650.0', '23.0  750.0', '23.0  850.0', '16.0  1000.0', '10.0  1050.0', '2.0  1100.0']
# cases = ['15.0  470.0', '20.0  490.0', '25.0  700.0', '26.0  800.0', '25.0  930.0', '20.0  1020.0', '5.0  1140.0']
# cases = ['23.0  850.0']
#####################################################

#####################################################
# INPUT BS DIMENSIONS TO CALCULATE RESPONSE FOR
ID = 0.243
SL = 0.700
OD = ['1.534'] # ['1.559'] # ['1.409', '1.434', '1.459', '1.484', '1.509', '1.534', '1.559', '1.584', '1.609', '1.634', '1.659']  # ['1.534'] #, '0.250', '0.300', '0.350', '0.400', '0.450', '0.500', '0.550', '0.600', '0.650', '0.700', '0.750', '0.800', '0.850', '0.900', '0.950', '1.000', '1.050', '1.100', '1.150', '1.200']
CL = ['12.549'] # ['12.799'] #['11.299', '11.549', '11.799', '12.049', '12.299', '12.549', '12.799', '13.049', '13.299', '13.549', '13.799']  # ['12.594'] #, '1.75', '2.00', '2.25', '2.50', '2.75', '3.00', '3.25', '3.50', '3.75', '4.00', '4.25', '4.50', '4.75', '5.00', '5.25', '5.50', '5.75', '6.00', '6.25', '6.50', '6.75', '7.00', '7.25', '7.50', '7.75', '8.00']
TL = 0.150
TOD = 0.283
MAT = ['NOLIN  60D_30'] #250000'] #, 'LIN  162300', 'NOLIN  60D-15deg', 'NOLIN  60D-30.8deg']
MATID = ['60D_30'] #, '162300', '60D-15deg', '60D-30.8deg']
#####################################################


inp1 = file('bsengine-cases.txt', 'w')

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
                inp = file("Case" + str(y) + '-' + str(j) + '-' + str(k) + '-' + str(q) + ".inp", 'w')
                inp1.write("Case" + str(y) + '-' + str(j) + '-' + str(k) + '-' + str(q) + ".inp" + '\n')

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
