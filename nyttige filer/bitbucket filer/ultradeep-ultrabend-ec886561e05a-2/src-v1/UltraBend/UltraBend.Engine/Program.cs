using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UltraBend.Engine.BSEngine;
using UltraBend.Services.BSEngine.Input;
using Segment = UltraBend.Services.BSEngine.Input.Segment;

namespace UltraBend
{

    internal class Program
    {
        private object lockSync = new Object();
        internal static void Main(string[] args)
        {
            try
            {
                new Program().Run(args).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                    Console.WriteLine("ERROR: " + e.Message + Environment.NewLine + e.StackTrace);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private async Task Run(string[] args)
        {
            var input = GetKieEngine();

            //await input.WriteDataGroupAsync(Console.Out);

            var jsonString = JsonConvert.SerializeObject(input);

            //Console.WriteLine(jsonString);

            // length
            double[] initial = new double[] { 0 };
            double[] xLower = new double[] { -1 };
            double[] xUpper = new double[] { 1 };


            var initialResult = await new BsEngine(null).ExecuteAsync(input);
            var initialWeight = initialResult.Segments.SelectMany(j => j.Elements.Select(i => i.Weight)).Sum();

            Console.WriteLine($"Initial weight: {initialWeight}");

            //var cache = new ConcurrentDictionary<double, BendStiffenerOutput>();

            /*var solution = Microsoft.SolverFoundation.Solvers.NelderMeadSolver.Solve(
                x =>
                {
                    lock (lockSync)
                    {
                        var length = 6.707 + x[0] * .15;
                        var xInput = GetKieEngine();
                        xInput.BendStiffenerData.Segments[1].Length = length;

                        double costFunction = 0;

                        var result = cache.GetOrAdd(length, j => new BsEngine().ExecuteAsync(xInput).Result);

                        //var result = ;

                        costFunction = result.KeyResults.MaximumBSCurvature;
                        //if (result.KeyResults.MaximumBSCurvature > 0.8)
                        //{
                        //    costFunction += result.KeyResults.MaximumBSCurvature * 1E9;
                        //}

                        //var totalWeight = result.Segments.SelectMany(j => j.Elements.Select(i => i.Weight)).Sum();

                        //costFunction += totalWeight / initialWeight;

                        //Console.WriteLine(
                        //    $"Length: {x[0]}\tWeight: {totalWeight}\tMax BS Curv: {result.KeyResults.MaximumBSCurvature.ToString("E")}\tCost: {costFunction}");

                        Console.WriteLine($"X[0]: {x[0]}\tLength: {length}\tMax BS Curv: {costFunction}");

                        return costFunction;
                    }
                }, initial, xLower, xUpper);*/


            //input.BendStiffenerData.Segments[1].Length = 5;
            //await new BsEngine().ExecuteAsync(input);
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
                FiniteElementAnalysisParameters = new FiniteElementAnalysisParameters
                {
                    ToleranceNorm = 1.0E-07,
                    MaxIterations = 30,
                    BasisIncrement = .1,
                    MinimumIncrement = 0.1,
                    MaximumIncrement = 1.0
                },
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
