#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      kie
#
# Created:     27/03/2018
# Copyright:   (c) kie 2018
# Licence:     <your licence>
#-------------------------------------------------------------------------------

# CREATE THE RESULTS FILE
summary = file('40-results1.txt', 'w')
lines = []
keyres1 = []
keyres2 = []
keyres = []

#################################
# COUNT THE NUMBER OF CASES
count = 0
with open('bsengine-cases.txt', 'r') as f:
    for line in f:
        count += 1
#print(count)
#count = count - 1

#################################
# WRITE FILE HEADER
summary.write("Load Case     c_max BS     C_max overall" + '\n')

#################################
# READ THE FILE NAMES
for x in range(0, count):
    num_lines = 0
    keyres1 = []
    keyres2 = []
    with open('bsengine-cases.txt', 'r') as in_file:
        lines = in_file.readlines()
        casefile = lines[x].rstrip('\r\n')
        casefile = casefile.rstrip('\r.inp')
        casefile = casefile + ".log"
        #print x
        #print casefile
        with open(casefile, 'r') as res_file:
            for line in res_file:
                num_lines += 1
        with open(casefile, 'r') as res_file:
            resline = res_file.readlines()
            keyres1a = resline[2050].rstrip('\r\n')  # Use 834 with LIN and 2050 with NOLIN # restrip removes trailing 'CR', which is coded as "\n".
            keyres1 = keyres1a[44:]
            keyres2a = resline[2052]                  # Use 836 with LIN and 2052 with NOLIN
            keyres2 = keyres2a[44:]
            casename = casefile.rstrip('\r\.log')
            keyres = casename + keyres1 + keyres2
            summary.write(keyres)

summary.close










#for line in open('bsengine-cases.txt') .xreadlines(): count +=1

# READ THE "CASES" FILE
#g = open(bsengine-cases.txt)
#num_lines = sum(1 for line in open('bsengine-cases.txt'))
#summary.write(count)
summary.close()

#def filelength(g):
#    with open(g) as
#lines =

#case = []
