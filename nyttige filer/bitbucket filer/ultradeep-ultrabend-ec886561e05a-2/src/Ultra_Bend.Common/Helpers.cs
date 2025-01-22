using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultra_Bend.Common.Data;
using UltraBend.Services.BSEngine.Input;

namespace Ultra_Bend.Common
{
    public static class Helpers
    {

        public static BendStiffenerConfiguration BuildConfiguration(
            string name,
            double forceDirection,
            double tension,
            RiserInformation riserInformation,
            BendStiffener bendStiffener,
            Data.FiniteElementAnalysisParameters feParameters
            )
        {

            var input = new BendStiffenerConfiguration
            {
                Name = name,
                BendStiffenerData = new BendStiffenerData
                {
                    InnerDiameter = riserInformation.OuterDiameter,
                    Segments = new List<Segment>()
                },
                UmbilicalData = new UmbilicalData
                {
                    Length = riserInformation.Length,
                    NumberOfElements = feParameters.ElementsInUmbilical,
                    BendingStiffness = riserInformation.BendingStiffness,
                    AxialStiffness = riserInformation.AxialStiffness,
                    TorsionalStiffness = riserInformation.TorsionalStiffness,
                    Mass = riserInformation.Mass
                },
                ElementPrint = new ElementPrint
                {
                    ElementRanges = new List<ElementRange>()
                },
                FiniteElementAnalysisParameters = new UltraBend.Services.BSEngine.Input.FiniteElementAnalysisParameters
                {
                    ToleranceNorm = feParameters.ToleranceNorm,
                    MaxIterations = feParameters.MaxIterations,
                    BasisIncrement = feParameters.BasisIncrement,
                    MinimumIncrement = feParameters.MinimumIncrement,
                    MaximumIncrement = feParameters.MaximumIncrement,
                },
                CurvatureRange = new CurvatureRange
                {
                    MaximumCurvature = feParameters.MaximumCurvature,
                    Number = feParameters.Number,
                },
                Force = new Force
                {
                    ForceDirection = forceDirection,
                    Tension = tension
                }
            };

            int indexCount = 1;
            foreach (var section in bendStiffener.Sections)
            {
                input.BendStiffenerData.Segments.Add(new Segment
                {
                    Length = section.Length,
                    NumberOfElements = feParameters.ElementsInSections,
                    OuterDiameter1 = section.RootOuterDiameter,
                    OuterDiameter2 = section.TipOuterDiameter,
                    Density = bendStiffener.Material.Density,
                    NonLinear = false,
                    EModulus = bendStiffener.Material.LinearElasticModulus
                });
                input.ElementPrint.ElementRanges.Add(new ElementRange
                {
                    StartElementIndex = indexCount,
                    EndElementIndex = indexCount + feParameters.ElementsInSections
                });
                indexCount += feParameters.ElementsInSections;
            }

            return input;
        }
    }
}
