using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultra_Bend.BSEngine;
using Ultra_Bend.BSEngine.BSEngine;
using Ultra_Bend.Common.Data.TestCases;

namespace Ultra_Bend.TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var outputService = new OutputsService();
            outputService.AddTextWriter(Console.Out);

            var project = NexansBacalhau.GetProject();

            var length = 5.0;
            var diameter = 1.0;

            project.BendStiffener.Sections[1].Length = length;
            project.BendStiffener.Sections[1].RootOuterDiameter = diameter;
            project.BendStiffener.Sections[0].TipOuterDiameter = diameter;
            project.BendStiffener.Sections[0].RootOuterDiameter = diameter;

            // pick the case
            var i = 5;
            var input = Common.Helpers.BuildConfiguration(
                project.ProjectInformation.Name,
                project.RiserResponses.NormalDeflectionAngles[i],
                project.RiserResponses.NormalUmbilicalTensions[i],
                project.RiserInformation,
                project.BendStiffener,
                project.FiniteElementAnalysisParameters);

            var engine = new BsEngine(outputService);

            var result = await engine.ExecuteAsync(input, true);
        }
    }
}
