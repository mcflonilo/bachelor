using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ultra_Bend.BSEngine.BSEngine;
using Ultra_Bend.Common.Data;
using UltraBend.Common;

namespace Ultra_Bend.Common
{
    public class Solver
    {

        public async Task FindOptimalBS(
            Project project,
            BSEngine.OutputsService outputsService)
        {
            // clone through json
            var projectDefinition = JsonConvert.DeserializeObject<Project>(JsonConvert.SerializeObject(project));

            var maxLength = projectDefinition.BendStiffener.BendStiffenerConstraints.MaxOverallLength;
            var minLength = projectDefinition.BendStiffener.BendStiffenerConstraints.MinOverallLength;

            var maxRootOuterDiameter = projectDefinition.BendStiffener.BendStiffenerConstraints.MaxRootOuterDiameter;
            var minRootOuterDiameter = projectDefinition.BendStiffener.BendStiffenerConstraints.MinRootOuterDiameter;

            var rootLength = projectDefinition.BendStiffener.BendStiffenerConstraints.RootLength;
            var tiptLength = projectDefinition.BendStiffener.BendStiffenerConstraints.TipLength;

            double[,] bounds = new double[2, 2];
            bounds[0, 0] = minLength;
            bounds[0, 1] = maxLength;
            bounds[1, 0] = minRootOuterDiameter;
            bounds[1, 1] = maxRootOuterDiameter;


            var optimalResult = Task.Run(async () =>
            {
                return Ultra_Bend.Common.Math.Lipo.adaptive_lipo(
                (x) =>
                {
                    return Task.Run(async () =>
                    {
                        var length = x[0];
                        var diameter = x[1];
                        var proj = JsonConvert.DeserializeObject<Project>(JsonConvert.SerializeObject(project));

                        proj.BendStiffener.Sections[1].Length = length;
                        proj.BendStiffener.Sections[1].RootOuterDiameter = diameter;
                        proj.BendStiffener.Sections[0].TipOuterDiameter = diameter;
                        proj.BendStiffener.Sections[0].RootOuterDiameter = diameter;
                        var y = await CapacityCurveCostFunction(proj, outputsService);

                        // checks if points are within a polygon (in capacity curve)
                        bool passesNormal = await WithinPolygon(
                            y.normalOperation.Keys.ToArray(),
                            y.normalOperation.Values.ToArray(),
                            proj.RiserCapacities.NormalDeflectionCurvature.ToArray(),
                            proj.RiserCapacities.NormalUmbilicalTensions.ToArray());

                        bool passesAbnormal = await WithinPolygon(
                            y.abnormalOperation.Keys.ToArray(),
                            y.abnormalOperation.Values.ToArray(),
                            proj.RiserCapacities.AbnormalDeflectionCurvature.ToArray(),
                            proj.RiserCapacities.AbnormalUmbilicalTensions.ToArray());

                        await outputsService.WriteLineAsync($"Length: {length}\tDiameter: {diameter}\tPasses Normal: {passesNormal}\tPasses Abnormal: {passesAbnormal}");

                        return ((!passesNormal || !passesAbnormal) ? 100000 : 1) * (y.weight);

                    }).Result;

                }, bounds);
            });

        }

        public async Task<bool> WithinPolygon(double[] x1, double[] y1, double[] x2, double[] y2)
        {
            var v1 = new List<Accord.IntPoint>();
            for (var i = 0; i < x1.Length; i++) {
                v1.Add(new Accord.IntPoint(Convert.ToInt32(x1[i]), Convert.ToInt32(y1[i])));
            }

            var v2 = new List<Accord.IntPoint>();
            for (var i = 0; i < x2.Length; i++)
            {
                v2.Add(new Accord.IntPoint(Convert.ToInt32(x2[i]), Convert.ToInt32(y2[i])));
            }

            var checker = new Accord.Math.Geometry.SimpleShapeChecker();
            return checker.CheckIfPointsFitShape(v2, v1);
        }

        public async Task<(double weight, Project project, Dictionary<double,double> normalOperation, Dictionary<double, double> abnormalOperation)> CapacityCurveCostFunction(
            Project project,
            BSEngine.OutputsService outputsService)
        {
            // clone through json
            var projectDefinition = JsonConvert.DeserializeObject<Project>(JsonConvert.SerializeObject(project));

            double weight = 0;
            var normalOperation = new ConcurrentDictionary<double, double>();
            var abnormalOperation = new ConcurrentDictionary<double, double>();
            var indexes = Enumerable.Range(0, projectDefinition.RiserResponses.NormalDeflectionAngles.Count);

            await indexes.ParallelForEachAsync(projectDefinition.FiniteElementAnalysisParameters.Threads, i => {

                var input = Helpers.BuildConfiguration(
                    projectDefinition.ProjectInformation.Name,
                    projectDefinition.RiserResponses.NormalDeflectionAngles[i],
                    projectDefinition.RiserResponses.NormalUmbilicalTensions[i],
                    projectDefinition.RiserInformation,
                    projectDefinition.BendStiffener,
                    projectDefinition.FiniteElementAnalysisParameters);

                var engine = new BsEngine(outputsService);

                var result = Task.Run(async () => await engine.ExecuteAsync(input)).Result;
                if (result?.Segments is { })
                {
                    if (weight == 0)
                        weight = result.Segments.SelectMany(j => j.Elements.Select(i => i.Weight)).Sum();
                    normalOperation.TryAdd(result.KeyResults.MaximumBSCurvature, projectDefinition.RiserResponses.NormalDeflectionAngles[i]);
                }

                return Task.CompletedTask;
            });

            await indexes.ParallelForEachAsync(projectDefinition.FiniteElementAnalysisParameters.Threads, i =>
            {
                var input = Helpers.BuildConfiguration(
                    projectDefinition.ProjectInformation.Name,
                    projectDefinition.RiserResponses.AbnormalDeflectionAngles[i],
                    projectDefinition.RiserResponses.AbnormalUmbilicalTensions[i],
                    projectDefinition.RiserInformation,
                    projectDefinition.BendStiffener,
                    projectDefinition.FiniteElementAnalysisParameters);

                var engine = new BsEngine(outputsService);
                var result = Task.Run(async () => await engine.ExecuteAsync(input)).Result;
                //var result = await engine.ExecuteAsync(input);
                if (result?.Segments is { })
                {
                    abnormalOperation.TryAdd(result.KeyResults.MaximumBSCurvature, projectDefinition.RiserResponses.AbnormalDeflectionAngles[i]);
                }
                return Task.CompletedTask;
            });

                //for (var i = 0; i < projectDefinition.RiserResponses.NormalDeflectionAngles.Count; i++)
                //{
                //    var input = Helpers.BuildConfiguration(
                //        projectDefinition.ProjectInformation.Name,
                //        projectDefinition.RiserResponses.NormalDeflectionAngles[i],
                //        projectDefinition.RiserResponses.NormalUmbilicalTensions[i],
                //        projectDefinition.RiserInformation,
                //        projectDefinition.BendStiffener,
                //        projectDefinition.FiniteElementAnalysisParameters);

                //    var engine = new BsEngine(outputsService);

                //    var result = Task.Run(async () => await engine.ExecuteAsync(input)).Result;
                //    if (result?.Segments is { })
                //    {
                //        if (weight == 0)
                //            weight = result.Segments.SelectMany(j => j.Elements.Select(i => i.Weight)).Sum();
                //        normalOperation.TryAdd(result.KeyResults.MaximumBSCurvature, projectDefinition.RiserResponses.NormalDeflectionAngles[i]);
                //    }
                //}

            //    for (var i = 0; i < projectDefinition.RiserResponses.AbnormalDeflectionAngles.Count; i++)
            //{
            //    var input = Helpers.BuildConfiguration(
            //        projectDefinition.ProjectInformation.Name,
            //        projectDefinition.RiserResponses.AbnormalDeflectionAngles[i],
            //        projectDefinition.RiserResponses.AbnormalUmbilicalTensions[i],
            //        projectDefinition.RiserInformation,
            //        projectDefinition.BendStiffener,
            //        projectDefinition.FiniteElementAnalysisParameters);

            //    var engine = new BsEngine(outputsService);
            //        var result = await engine.ExecuteAsync(input);
            //    if (result?.Segments is { })
            //    {
            //        abnormalOperation.TryAdd(result.KeyResults.MaximumBSCurvature, projectDefinition.RiserResponses.AbnormalDeflectionAngles[i]);
            //    }
            //}

            return (weight, projectDefinition, normalOperation.ToDictionary(k => k.Key, k => k.Value), abnormalOperation.ToDictionary(k => k.Key, k => k.Value));
        }
    }
}
