using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UltraBend.Services.BSEngine.Input;
using UltraBend.Services.BSEngine.Output;
using Segment = UltraBend.Services.BSEngine.Input.Segment;

namespace Ultra_Bend.BSEngine.BSEngine
{
    public class BsEngine
    {
        private const string BsEngineFilePath = "BSEngine/bsengine.exe";
        private const string BsEngineLicenseFilePath = "BSEngine/bsengine.lic";

        protected OutputsService output { get; set; } = null;
        protected String channel { get; set; } = "Engine";

        public BsEngine(OutputsService outputService)
        {
            output = outputService;
        }


        public async Task RunTestAsync()
        {
            var input = GetKieEngine();
            await output.WriteLineAsync("Test Kie Engine Input Created");

            //var jsonString = JsonConvert.SerializeObject(input);

            var initialResult = await ExecuteAsync(input);

            if (initialResult != null)
            {
                var initialWeight = initialResult.Segments.SelectMany(j => j.Elements.Select(i => i.Weight)).Sum();

                Console.WriteLine($"Initial weight: {initialWeight}");
            }
        }

        public async Task<BendStiffenerOutput> ExecuteAsync(BendStiffenerConfiguration config, bool writeOutput = false)
        {
            // get the path to run in
            var path = GetPath(config);

            if (writeOutput)
                await output.WriteLineAsync($"Execution Path: {path}");

            // create it if needed
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var inputPath = Path.Combine(path, $"input.inp");

            if (File.Exists(inputPath))
            {
                File.Delete(inputPath);
            }

            using (TextWriter writer = File.CreateText(inputPath))
            {
                await config.WriteDataGroupAsync(writer);
            }

            var localBsEngineFilePath = Path.Combine(path, "bsengine.exe");
            var localBsEngineLicenseFilePath = Path.Combine(path, "bsengine.lic");

            File.Copy(GetBsEngineFilePath(), localBsEngineFilePath, true);
            File.Copy(GetBsEngineLicenseFilePath(), localBsEngineLicenseFilePath, true);

            var shortInputPath = Helpers.GetShortPath(inputPath);

            await Helpers.RunProcessAsync(
                localBsEngineFilePath,
                Path.GetDirectoryName(localBsEngineFilePath),
                $"-b {shortInputPath}",
                output,
                writeOutput);

            var outputPath = Path.Combine(Path.GetDirectoryName(inputPath), "input.log");
            var outputPathFea = Path.Combine(Path.GetDirectoryName(inputPath), "input_FEA.log");

            if (writeOutput)
                await output.WriteLineAsync($"Output Path: {outputPath}");

            if (File.Exists(outputPath))
            {
                return await ParseOutput(outputPath, outputPathFea);
            }
            else
            {
                if (writeOutput)
                    await output.WriteLineAsync($"Error: Output file, {outputPath}, did not exist.");

                return null;
            }
        }

        public async Task<BendStiffenerOutput> ParseOutput(string path, string pathFea, bool writeOutput = false)
        {
            var bendStiffenerOutput = new BendStiffenerOutput();

            using (TextReader reader = File.OpenText(path))
            {
                await bendStiffenerOutput.ReadLogAsync(reader, output, channel, writeOutput);
            }

            if (bendStiffenerOutput.Error)
            {
                using (TextReader reader = File.OpenText(pathFea))
                {
                    await bendStiffenerOutput.ReadLogAsync(reader, output, channel, writeOutput);
                }
            }

            return bendStiffenerOutput;
        }

        private string GetBsEngineFilePath()
        {
            return Path.Combine(Helpers.AssemblyDirectory, BsEngineFilePath);
        }
        private string GetBsEngineLicenseFilePath()
        {
            return Path.Combine(Helpers.AssemblyDirectory, BsEngineLicenseFilePath);
        }

        private string GetDataDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        private string GetPath(BendStiffenerConfiguration config)
        {
            var jsonConfig = JsonConvert.SerializeObject(config);

            var hash = Helpers.GetHashMd5(jsonConfig);

            var directory = GetDataDirectory();

            var path = Path.Combine(directory, "UltraBend", hash);
            return path;
        }

        public static BendStiffenerConfiguration GetKieEngine()
        {
            return new BendStiffenerConfiguration
            {
                Name = "Functional testing",
                BendStiffenerData = new BendStiffenerData
                {
                    InnerDiameter = 0.142,
                    Segments = new List<Segment>
                    {
                        new Segment
                        {
                            Length = 0.650,
                            NumberOfElements = 10,
                            OuterDiameter1 = 1.092,
                            OuterDiameter2 = 1.092,
                            Density = 2000,
                            NonLinear = false,
                            EModulus = 10000.0E06
                        },
                        new Segment
                        {
                            Length = 6.707,
                            NumberOfElements = 100,
                            OuterDiameter1 = 1.092,
                            OuterDiameter2 = 0.199,
                            Density = 1150,
                            NonLinear = false,
                            EModulus = 200000.0
                        },
                        new Segment
                        {
                            Length = 0.100,
                            NumberOfElements = 5,
                            OuterDiameter1 = 0.199,
                            OuterDiameter2 = 0.199,
                            Density = 1150,
                            NonLinear = false,
                            EModulus = 200000.0
                        }
                    }
                },
                UmbilicalData = new UmbilicalData
                {
                    Length = 3.0,
                    NumberOfElements = 30,
                    BendingStiffness = 28.0,
                    AxialStiffness = 448000,
                    TorsionalStiffness = 27.0,
                    Mass = 31.4
                },
                ElementPrint = new ElementPrint
                {
                    ElementRanges = new List<ElementRange>
                    {
                        new ElementRange
                        {
                            StartElementIndex = 1,
                            EndElementIndex = 10
                        },
                        new ElementRange
                        {
                            StartElementIndex = 11,
                            EndElementIndex = 110
                        },
                        new ElementRange
                        {
                            StartElementIndex = 111,
                            EndElementIndex = 115
                        }
                    }
                },
                FiniteElementAnalysisParameters = new FiniteElementAnalysisParameters(),
                CurvatureRange = new CurvatureRange
                {
                    MaximumCurvature = 0.5,
                    Number = 32
                },
                Force = new Force
                {
                    ForceDirection = 20.8,
                    Tension = 350.0
                }
            };
        }
    }
}
