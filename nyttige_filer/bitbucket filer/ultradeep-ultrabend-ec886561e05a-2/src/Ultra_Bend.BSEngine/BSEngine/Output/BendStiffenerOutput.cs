using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ultra_Bend.BSEngine;

namespace UltraBend.Services.BSEngine.Output
{
    public class BendStiffenerOutput
    {
        public int NumberOfSegments { get; set; }

        public double InnerDiameter { get; set; }

        public double BendStiffenerLength { get; set; }

        public int NumberOfElements { get; set; }

        public double AverageElementLength { get; set; }

        public List<Segment> Segments { get; set; }

        public RiserUmbilical RiserUmbilical { get; set; } = new RiserUmbilical();

        public Load Load { get; set; } = new Load();

        public List<Node> Nodes { get; set; } = new List<Node>();

        public KeyResults KeyResults { get; set; } = new KeyResults();

        public List<ElementResult> CombinedElementResults { get; set; } = new List<ElementResult>();

        public List<NodeResult> CombinedNodeResults { get; set; } = new List<NodeResult>();

        public List<ElementResult> RiserElementResults { get; set; } = new List<ElementResult>();

        public List<BendStiffenerResult> BendStiffenerResults { get; set; } = new List<BendStiffenerResult>();

        public bool Error { get; set; } = false;

        public string RawOutput { get; set; }

        public async Task ReadLogAsync(TextReader reader, OutputsService output, string channel, bool writeOutput)
        {
            string line;

            var currentSection = OutputSection.Start;

            var currentSegment = -1;

            var counter = 0;

            // use a dictionary for fast element lookup (ref)
            var ElemNumberToElementMap = new Dictionary<int, Element>();


            var sbRaw = new StringBuilder();
            while ((line = reader.ReadLine()) != null)
            {
                counter++;

                line = line.Replace("\0", string.Empty);
                var lineTrimmed = Regex.Replace(line.Trim(), @"\s+", " ");

                sbRaw.AppendLine(line);

                if (lineTrimmed.ToLower().Contains("error"))
                    Error = true;

                if (writeOutput)
                    await output.WriteLineAsync($"{lineTrimmed}");
                //System.Console.WriteLine(line);

                //if (counter > 375)
                //    System.Console.Write(lineTrimmed);


                if (lineTrimmed.StartsWith("Bend stiffener properties", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.BendStiffenerProperties;
                    continue;
                }

                if (lineTrimmed.StartsWith("Riser/umbilical properties", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.RiserUmbilicalProperties;
                    continue;
                }

                if (lineTrimmed.StartsWith("Material data", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.MaterialData;
                    continue;
                }

                if (lineTrimmed.StartsWith("Load data", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.LoadData;
                    continue;
                }

                if (lineTrimmed.StartsWith("Beam element : lenght and stiffness data :",
                    StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.ElementDataLengthAndStiffness;
                    continue;
                }

                if (lineTrimmed.StartsWith("Beam element : Weight and buoyancy data (per unit lenght)",
                    StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.ElementDataWeightAndBuoyancy;
                    continue;
                }

                if (lineTrimmed.StartsWith("Initial (stress-free) coordinates", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.InitialCoordinates;
                    continue;
                }

                if (lineTrimmed.StartsWith("Key results", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.KeyResults;
                    continue;
                }

                if (lineTrimmed.StartsWith("Results for combined model (riser and bend stiffener)",
                    StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.CombinedResults;
                    continue;
                }

                if (lineTrimmed.StartsWith("Results for riser", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.RiserResults;
                    continue;
                }

                if (lineTrimmed.StartsWith("Results for bend stiffener", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = OutputSection.BendStiffenerResults;
                    continue;
                }


                int intTester;
                double doubleTester;

                if (currentSection == OutputSection.BendStiffenerProperties)
                {
                    if (currentSegment == -1)
                    {
                        if (lineTrimmed.StartsWith("Numer of Segments", StringComparison.OrdinalIgnoreCase))
                        {
                            NumberOfSegments = int.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments = new List<Segment>();
                            for (var i = 0; i < NumberOfSegments; i++)
                                Segments.Add(new Segment());
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Inner diameter", StringComparison.OrdinalIgnoreCase))
                        {
                            InnerDiameter = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Bend stiffener lenght", StringComparison.OrdinalIgnoreCase))
                        {
                            BendStiffenerLength = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Numer of elements", StringComparison.OrdinalIgnoreCase))
                        {
                            NumberOfElements = int.Parse(lineTrimmed.Split(':')[1].Trim());
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Average element lenght", StringComparison.OrdinalIgnoreCase))
                        {
                            AverageElementLength = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            continue;
                        }
                    }

                    // segments

                    if (lineTrimmed.StartsWith("Segment number", StringComparison.OrdinalIgnoreCase))
                    {
                        currentSegment = int.Parse(lineTrimmed.Split(':')[1].Trim());
                        Segments[currentSegment - 1].SegmentNumber = currentSegment;
                        continue;
                    }

                    if (currentSegment > -1)
                    {
                        if (lineTrimmed.StartsWith("Number of elements", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = int.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].NumberOfElements = value;
                            continue;
                        }


                        if (lineTrimmed.StartsWith("First element number", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = int.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].FirstElementNumber = value;
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Last element number", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = int.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].LastElementNumber = value;
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Segment length", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].SegmentLength = value;
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Element length", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].ElementLength = value;
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Outer diameter, end 1", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].OuterDiameterEnd1 = value;
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Outer diameter, end 2", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].OuterDiameterEnd2 = value;
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Density", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].Density = value;
                            continue;
                        }

                        if (lineTrimmed.StartsWith("Material model", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = lineTrimmed.Split(':')[1].Trim();
                            Segments[currentSegment - 1].MaterialModel = value;
                            continue;
                        }

                        if (lineTrimmed.StartsWith("E-modulus", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = double.Parse(lineTrimmed.Split(':')[1].Trim());
                            Segments[currentSegment - 1].EModulus = value;
                            continue;
                        }

                        if (lineTrimmed.Split(null).Length == 3
                            && int.TryParse(lineTrimmed.Split(null)[0], out intTester)
                            && double.TryParse(lineTrimmed.Split(null)[1], out doubleTester)
                            && double.TryParse(lineTrimmed.Split(null)[2], out doubleTester))
                        {
                            var split = lineTrimmed.Split(null);
                            Segments[currentSegment - 1].Elements.Add(new Element
                            {
                                ElementNumber = int.Parse(split[0]),
                                LengthCoordinate = double.Parse(split[1]),
                                OuterDiameter = double.Parse(split[2])
                            });
                        }
                    }
                }

                if (currentSection == OutputSection.RiserUmbilicalProperties)
                {
                    if (lineTrimmed.StartsWith("Number of elements", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.NumberOfElements = int.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("First element number", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.FirstElementNumber = int.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Last element number", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.LastElementNumber = int.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Length", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.Length = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Element length", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.ElementLength = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Bend stiffness", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.BendStiffness = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Axial stiffness", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.AxialStiffness = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Torsional stiffness", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.TorsionalStiffness = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Mass per unit length", StringComparison.OrdinalIgnoreCase))
                    {
                        RiserUmbilical.MassPerUnitLength = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }
                }

                if (currentSection == OutputSection.LoadData)
                {
                    if (lineTrimmed.StartsWith("Force", StringComparison.OrdinalIgnoreCase))
                    {
                        Load.Force = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Relative force angle (deg)", StringComparison.OrdinalIgnoreCase))
                    {
                        Load.RelativeForceAngle = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }
                }

                if (currentSection == OutputSection.ElementDataLengthAndStiffness)
                {
                    var split = lineTrimmed.Split(null);
                    if (split.Length == 6
                        && int.TryParse(split[0], out intTester)
                        && double.TryParse(split[1], out doubleTester)
                        && double.TryParse(split[2], out doubleTester)
                        && double.TryParse(split[3], out doubleTester)
                        && double.TryParse(split[4], out doubleTester)
                        && double.TryParse(split[5], out doubleTester))
                        foreach (var element in Segments.SelectMany(i => i.Elements)
                            .Where(i => i.ElementNumber == int.Parse(split[0])))
                        {
                            element.Length = double.Parse(split[1]);
                            element.AxialStiffness = double.Parse(split[2]);
                            element.BendingStiffnessY = double.Parse(split[3]);
                            element.BendingStiffnessZ = double.Parse(split[4]);
                            element.TorsionalStiffness = double.Parse(split[5]);
                        }
                }

                if (currentSection == OutputSection.ElementDataWeightAndBuoyancy)
                {
                    if (ElemNumberToElementMap.Count != Segments.Sum(s => s.Elements.Count))
                        foreach (var element in Segments.SelectMany(i => i.Elements).ToList())
                            if (!ElemNumberToElementMap.ContainsKey(element.ElementNumber))
                                ElemNumberToElementMap.Add(element.ElementNumber, element);

                    var split = lineTrimmed.Split(null);
                    if (split.Length == 3
                        && int.TryParse(split[0], out intTester)
                        && double.TryParse(split[1], out doubleTester)
                        && double.TryParse(split[2], out doubleTester))
                    {
                        var elemNumber = int.Parse(split[0]);
                        if (ElemNumberToElementMap.ContainsKey(elemNumber))
                        {
                            ElemNumberToElementMap[elemNumber].Weight = double.Parse(split[1]);
                            ElemNumberToElementMap[elemNumber].Buoyancy = double.Parse(split[2]);
                        }

                        //foreach (var element in Segments.SelectMany(i => i.Elements)
                        //    .Where(i => i.ElementNumber == int.Parse(split[0])))
                        //{
                        //    element.Weight = double.Parse(split[1]);
                        //    element.Buoyancy = double.Parse(split[2]);
                        //}
                    }
                }

                if (currentSection == OutputSection.InitialCoordinates)
                {
                    var split = lineTrimmed.Split(null);
                    if (split.Length == 4
                        && int.TryParse(split[0], out intTester)
                        && double.TryParse(split[1], out doubleTester)
                        && double.TryParse(split[2], out doubleTester)
                        && double.TryParse(split[3], out doubleTester))
                        Nodes.Add(new Node
                        {
                            NodeNumber = int.Parse(split[0]),
                            X = double.Parse(split[1]),
                            Y = double.Parse(split[2]),
                            Z = double.Parse(split[3])
                        });
                }

                if (currentSection == OutputSection.KeyResults)
                {
                    if (lineTrimmed.StartsWith("BS moment at root end", StringComparison.OrdinalIgnoreCase))
                    {
                        KeyResults.BSMomentAtRootEnd = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("BS shear force at root end", StringComparison.OrdinalIgnoreCase))
                    {
                        KeyResults.BSShearForceAtRootEnd = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Riser tension at root end", StringComparison.OrdinalIgnoreCase))
                    {
                        KeyResults.RiserTensionAtRootEnd = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Maximum BS curvature", StringComparison.OrdinalIgnoreCase))
                    {
                        KeyResults.MaximumBSCurvature = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Maximum BS strain at OD", StringComparison.OrdinalIgnoreCase))
                    {
                        KeyResults.MaximumBSStrainAtOuterDiameter = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Maximum curvature", StringComparison.OrdinalIgnoreCase))
                    {
                        KeyResults.MaximumCurvature = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }

                    if (lineTrimmed.StartsWith("Z-displacment of riser tip", StringComparison.OrdinalIgnoreCase))
                    {
                        KeyResults.ZDisplacementOfRiserTip = double.Parse(lineTrimmed.Split(':')[1].Trim());
                        continue;
                    }
                }

                if (currentSection == OutputSection.CombinedResults)
                {
                    var split = lineTrimmed.Split(null);
                    if (split.Length == 10
                        && int.TryParse(split[0], out intTester)
                        && double.TryParse(split[1], out doubleTester)
                        && double.TryParse(split[2], out doubleTester)
                        && double.TryParse(split[3], out doubleTester)
                        && double.TryParse(split[4], out doubleTester)
                        && double.TryParse(split[5], out doubleTester)
                        && double.TryParse(split[6], out doubleTester)
                        && double.TryParse(split[7], out doubleTester)
                        && double.TryParse(split[8], out doubleTester)
                        && double.TryParse(split[9], out doubleTester))
                        CombinedElementResults.Add(new ElementResult
                        {
                            ElementNumber = int.Parse(split[0]),
                            Length = double.Parse(split[1]),
                            Tension = double.Parse(split[2]),
                            Angle = double.Parse(split[3]),
                            CurvatureEnd1 = double.Parse(split[4]),
                            CurvatureEnd2 = double.Parse(split[5]),
                            MomentEnd1 = double.Parse(split[6]),
                            MomentEnd2 = double.Parse(split[7]),
                            ShearEnd1 = double.Parse(split[8]),
                            ShearEnd2 = double.Parse(split[9])
                        });

                    if (split.Length == 5
                        && int.TryParse(split[0], out intTester)
                        && double.TryParse(split[1], out doubleTester)
                        && double.TryParse(split[2], out doubleTester)
                        && double.TryParse(split[3], out doubleTester)
                        && double.TryParse(split[4], out doubleTester))
                        CombinedNodeResults.Add(new NodeResult
                        {
                            NodeNumber = int.Parse(split[0]),
                            LengthCoordinate = double.Parse(split[1]),
                            X = double.Parse(split[2]),
                            Z = double.Parse(split[3]),
                            Curvature = double.Parse(split[4])
                        });
                }


                if (currentSection == OutputSection.RiserResults)
                {
                    var split = lineTrimmed.Split(null);
                    if (split.Length == 10
                        && int.TryParse(split[0], out intTester)
                        && double.TryParse(split[1], out doubleTester)
                        && double.TryParse(split[2], out doubleTester)
                        && double.TryParse(split[3], out doubleTester)
                        && double.TryParse(split[4], out doubleTester)
                        && double.TryParse(split[5], out doubleTester)
                        && double.TryParse(split[6], out doubleTester)
                        && double.TryParse(split[7], out doubleTester)
                        && double.TryParse(split[8], out doubleTester)
                        && double.TryParse(split[9], out doubleTester))
                        RiserElementResults.Add(new ElementResult
                        {
                            ElementNumber = int.Parse(split[0]),
                            Length = double.Parse(split[1]),
                            Tension = double.Parse(split[2]),
                            Angle = double.Parse(split[3]),
                            CurvatureEnd1 = double.Parse(split[4]),
                            CurvatureEnd2 = double.Parse(split[5]),
                            MomentEnd1 = double.Parse(split[6]),
                            MomentEnd2 = double.Parse(split[7]),
                            ShearEnd1 = double.Parse(split[8]),
                            ShearEnd2 = double.Parse(split[9])
                        });

                    //if (split.Length == 5
                    //    && Int32.TryParse(split[0], out intTester)
                    //    && Double.TryParse(split[1], out doubleTester)
                    //    && Double.TryParse(split[2], out doubleTester)
                    //    && Double.TryParse(split[3], out doubleTester)
                    //    && Double.TryParse(split[4], out doubleTester))
                    //{
                    //    RiserNodeResults.Add(new NodeResult
                    //    {
                    //        NodeNumber = Int32.Parse(split[0]),
                    //        LengthCoordinate = Double.Parse(split[1]),
                    //        X = Double.Parse(split[2]),
                    //        Z = Double.Parse(split[3]),
                    //        Curvature = Double.Parse(split[4])
                    //    });
                    //}
                }

                if (currentSection == OutputSection.BendStiffenerResults)
                {
                    var split = lineTrimmed.Split(null);
                    if (split.Length == 7
                        && int.TryParse(split[0], out intTester)
                        && double.TryParse(split[1], out doubleTester)
                        && double.TryParse(split[2], out doubleTester)
                        && double.TryParse(split[3], out doubleTester)
                        && double.TryParse(split[4], out doubleTester)
                        && double.TryParse(split[5], out doubleTester)
                        && double.TryParse(split[6], out doubleTester))
                        BendStiffenerResults.Add(new BendStiffenerResult
                        {
                            NodeNumber = int.Parse(split[0]),
                            LengthCoordinate = double.Parse(split[1]),
                            OuterDiameter = double.Parse(split[2]),
                            Curvature = double.Parse(split[3]),
                            Moment = double.Parse(split[4]),
                            ShearForce = double.Parse(split[5]),
                            Strain = double.Parse(split[6])
                        });
                }
            }

            RawOutput = sbRaw.ToString();

        }

        private enum OutputSection
        {
            Start,
            BendStiffenerProperties,
            RiserUmbilicalProperties,
            MaterialData,
            LoadData,
            ElementDataLengthAndStiffness,
            ElementDataWeightAndBuoyancy,
            InitialCoordinates,
            KeyResults,
            CombinedResults,
            RiserResults,
            BendStiffenerResults
        }
    }
}