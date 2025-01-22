using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra_Bend.Common.Data.TestCases
{
    public static class NexansBacalhau
    {

        public static Project GetProject()
        {

            var sampleMaterial = new Material()
            {
                Name = "Sample",
                Density = 1150,
                NonLinear = true,
                AllowTemperatureExtrapolation = false,
                Data = new List<MaterialData>()
                {
new MaterialData(){ Strain = 0, Stress = 0, Temperature = 297.15},
new MaterialData(){ Strain = 0.5, Stress = 1207.77992879395, Temperature = 297.15},
new MaterialData(){ Strain = 1, Stress = 2271.47405429462, Temperature = 297.15},
new MaterialData(){ Strain = 1.5, Stress = 3209.19256786827, Temperature = 297.15},
new MaterialData(){ Strain = 2, Stress = 4045.67445482866, Temperature = 297.15},
new MaterialData(){ Strain = 2.5, Stress = 4759.45482866044, Temperature = 297.15},
new MaterialData(){ Strain = 3, Stress = 5347.5896306186, Temperature = 297.15},
new MaterialData(){ Strain = 3.5, Stress = 5820.04899866489, Temperature = 297.15},
new MaterialData(){ Strain = 4, Stress = 6204.95304850912, Temperature = 297.15},
new MaterialData(){ Strain = 4.5, Stress = 6539.51486426346, Temperature = 297.15},
new MaterialData(){ Strain = 5, Stress = 6798.2214953271, Temperature = 297.15},
new MaterialData(){ Strain = 5.5, Stress = 7018.49666221629, Temperature = 297.15},
new MaterialData(){ Strain = 6, Stress = 7221.4867823765, Temperature = 297.15},
new MaterialData(){ Strain = 6.5, Stress = 7390.96635514019, Temperature = 297.15},
new MaterialData(){ Strain = 7, Stress = 7540.67899421451, Temperature = 297.15},
new MaterialData(){ Strain = 7.5, Stress = 7674.26119270138, Temperature = 297.15},
new MaterialData(){ Strain = 8, Stress = 7794.5709835336, Temperature = 297.15},
new MaterialData(){ Strain = 8.5, Stress = 7902.81392968402, Temperature = 297.15},
new MaterialData(){ Strain = 9, Stress = 8011.13457943925, Temperature = 297.15},
new MaterialData(){ Strain = 9.5, Stress = 8114.23925233645, Temperature = 297.15},
new MaterialData(){ Strain = 10, Stress = 8198.26844681798, Temperature = 297.15},
new MaterialData(){ Strain = 10.5, Stress = 8289.10703159769, Temperature = 297.15},
new MaterialData(){ Strain = 11, Stress = 8386.43631508678, Temperature = 297.15},
new MaterialData(){ Strain = 11.5, Stress = 8461.06444147753, Temperature = 297.15},
new MaterialData(){ Strain = 12, Stress = 8536.2082777036, Temperature = 297.15},
new MaterialData(){ Strain = 12.5, Stress = 8617.12327547841, Temperature = 297.15},
new MaterialData(){ Strain = 13, Stress = 8690.64557187361, Temperature = 297.15},
new MaterialData(){ Strain = 13.5, Stress = 8765.45669781931, Temperature = 297.15},
new MaterialData(){ Strain = 14, Stress = 8837.17507788162, Temperature = 297.15},
new MaterialData(){ Strain = 14.5, Stress = 8907.57961726747, Temperature = 297.15},
new MaterialData(){ Strain = 15, Stress = 8965.77080551847, Temperature = 297.15},
                }
            };

            return
                new Project
                {
                    Materials = new List<Material>()
                    {
                        sampleMaterial
                    },
                    ProjectInformation = new ProjectInformation
                    {
                        Client = "Sample Client",
                        DesignerName = "Sample Designer",
                        Name = "Sample Project"
                    },
                    BendStiffener = new BendStiffener
                    {
                        Sections = new List<BendStiffenerSection>
                        {
                            new BendStiffenerSection()
                            {
                                Length = 0.6473,
                                Name = "Stiff Section",
                                RootOuterDiameter = 1,
                                TipOuterDiameter = 1
                            },
                            new BendStiffenerSection()
                            {
                                Length = 6,
                                Name = "Conical Section",
                                RootOuterDiameter = 1,
                                TipOuterDiameter = 0.325
                            },
                            new BendStiffenerSection()
                            {
                                Length = .2,
                                Name = "Tip Section",
                                RootOuterDiameter = 0.325,
                                TipOuterDiameter = 0.325
                            }
                        },
                        Material = sampleMaterial,
                        BendStiffenerConstraints = new BendStiffenerConstraints()
                        {
                            Clearance = .1,
                            MaxOverallLength = 10,
                            MaxRootOuterDiameter = 2,
                            MinOverallLength = 1,
                            MinRootOuterDiameter = .5,
                            MinThickness = .01,
                            RootLength = .7,
                            TipLength = .2
                        }
                    },
                    RiserCapacities = new RiserCapacities
                    {
                        NormalDeflectionCurvature = new List<double>()
                        {
                            0.08641,
                            0.08952,
                            0.08711,
                            0.08397,
                            0.08083,
                            0.07767,
                            0.07453,
                            0.07138,
                            0.06824,
                            0.0651,
                            0.06194,
                            0.0588,
                            0.05565,
                            0.05252,
                            0.04936,
                            0.04621,
                            0.04307,
                            0.03993,
                            0.03678,
                            0.03363,
                            0.03006,
                            0.02649,
                            0.02292,
                            0.01936,
                            0.01507,
                            0.01064,
                            0.00621,
                            0.003456,
                            0.002356,
                            0.001318,
                            0
                        },
                        NormalUmbilicalTensions = new List<double>
                        {
                            0,
                            40.82,
                            81.63,
                            122.4,
                            163.3,
                            204.1,
                            244.9,
                            285.7,
                            326.5,
                            367.3,
                            408.2,
                            449,
                            489.8,
                            530.6,
                            571.4,
                            612.2,
                            653.1,
                            693.9,
                            734.7,
                            775.5,
                            816.3,
                            857.1,
                            898,
                            938.8,
                            979.6,
                            1020,
                            1061,
                            1102,
                            1143,
                            1184,
                            1224

                        },
                        AbnormalDeflectionCurvature = new List<double>
                        {
                            0.127,
                            0.1246,
                            0.1222,
                            0.1198,
                            0.1174,
                            0.115,
                            0.1126,
                            0.1102,
                            0.1078,
                            0.1054,
                            0.103,
                            0.1006,
                            0.09817,
                            0.09577,
                            0.09337,
                            0.09097,
                            0.08857,
                            0.08617,
                            0.08377,
                            0.08137,
                            0.07884,
                            0.07648,
                            0.07335,
                            0.0703,
                            0.06694,
                            0.06337,
                            0.0598,
                            0.05624,
                            0.05267,
                            0.0491,
                            0.04553,
                            0.04196,
                            0.03839,
                            0.03482,
                            0.03126,
                            0.02769,
                            0.02412,
                            0.02055,
                            0.01698,
                            0.01341,
                            0.009846,
                            0.006238,
                            0.003595,
                            0.002734,
                            0.001905,
                            0.0005424,
                            0

                        },
                        AbnormalUmbilicalTensions = new List<double>
                        {
                            0,
                            40.82,
                            81.63,
                            122.4,
                            163.3,
                            204.1,
                            244.9,
                            285.7,
                            326.5,
                            367.3,
                            408.2,
                            449,
                            489.8,
                            530.6,
                            571.4,
                            612.2,
                            653.1,
                            693.9,
                            734.7,
                            775.5,
                            816.3,
                            857.1,
                            898,
                            938.8,
                            979.6,
                            1020,
                            1061,
                            1102,
                            1143,
                            1184,
                            1224,
                            1265,
                            1306,
                            1347,
                            1388,
                            1429,
                            1469,
                            1510,
                            1551,
                            1592,
                            1633,
                            1673,
                            1714,
                            1755,
                            1796,
                            1837,
                            1878
                        }
                    },
                    RiserResponses = new RiserResponses
                    {
                        NormalDeflectionAngles = new List<double>
                        {
                            3,
                            6,
                            11,
                            12,
                            13,
                            12,
                            10,
                            8,
                            4,
                            2,
                        },
                        NormalUmbilicalTensions = new List<double>()
                        {
                            350,
                            400,
                            470,
                            520,
                            570,
                            700,
                            770,
                            790,
                            800,
                            850,
                        },
                        AbnormalDeflectionAngles = new List<double>
                        {
                            3,
                            6,
                            11,
                            12,
                            13,
                            12,
                            10,
                            8,
                            4,
                            2,
                        },
                        AbnormalUmbilicalTensions = new List<double>()
                        {
                            350,
                            400,
                            470,
                            520,
                            570,
                            700,
                            770,
                            790,
                            800,
                            850,
                        }
                    },
                    RiserInformation = new RiserInformation()
                    {

                    },
                    FiniteElementAnalysisParameters = new FiniteElementAnalysisParameters()
                    {
                        ToleranceNorm = 1E-7,
                        MaxIterations = 30,
                        BasisIncrement = .1,
                        MinimumIncrement = .1,
                        MaximumIncrement = 1,
                        Threads = 16,
                        MaximumCurvature = 0.5,
                        ElementsInUmbilical = 30,
                        ElementsInSections = 100
                    }                    
                };

        }
    }
}
