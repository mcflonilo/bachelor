using System;
using System.Collections.Generic;
using System.Linq;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Dependencies;
using UltraBend.Common.Math;
using UltraBend.Databases.Material;
using UltraBend.Databases.Project;
using Material = UltraBend.Services.DomainObjects.Material;
using MaterialData = UltraBend.Services.DomainObjects.MaterialData;


namespace UltraBend.Services.Project
{
    public class MaterialsService : BaseFileService<ProjectDbContext, Material>
    {
        public MaterialsService(string fileName, bool createFile = false)
        {
            FileName = fileName;

            if (createFile)
                using (new ProjectDbContext(fileName, createFile)) ;
        }

        public void DeleteById(Guid id)
        {
            var hasUpdate = false;
            using (var context = new ProjectDbContext(FileName))
            {
                var entity = context.Materials.Where(m => m.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    context.Materials.Remove(entity);
                    context.SaveChanges();
                    hasUpdate = true;
                    Invalidate(id);
                }
            }
            if (hasUpdate)OnRepositoryUpdated(this);
        }

        public List<Material> GetMaterials()
        {
            using (var context = new ProjectDbContext(FileName))
            {
                var result = new List<Material>(context.Materials
                    .AsEnumerable()
                    .Select(m => new Material
                    {
                        Id = m.Id,
                        Description = m.Description,
                        Name = m.Name,
                        RegressionOrder = m.RegressionOrder,
                        Regression =
                            Common.Data.ByteSerialization.GetDataContractByBytes<ExpressionRegression>(m.Regression),
                        AllowTemperatureExtrapolation = m.AllowTemperatureExtrapolation,
                        ForceZeroZero = m.ForceZeroZero,
                        TemperatureExtrapolationMax = m.TemperatureExtrapolationMax,
                        TemperatureExtrapolationMin = m.TemperatureExtrapolationMin,
                        RSquared = m.RSquared,
                        Density = m.Density,
                        NonLinear = m.NonLinear,
                        LinearElasticModulus = m.LinearElasticModulus,
                        Data = new List<MaterialData>(m.MaterialData.Select(md => new MaterialData
                            {
                                Id = md.Id,
                                Index = md.Index,
                                Strain = md.Strain,
                                Stress = md.Stress,
                                Temperature = md.Temperature
                            })
                            .OrderBy(d => d.Index).ToList())
                    })
                    .ToList());
                return result;
            }
        }

        [Cache]
        public Material GetMaterialById(Guid id)
        {
            using (var context = new ProjectDbContext(FileName))
            {
                return context.Materials.Where(m => m.Id == id)
                    .AsEnumerable()
                    .Select(m => new Material
                    {
                        Id = m.Id,
                        Description = m.Description,
                        Name = m.Name,
                        RegressionOrder = m.RegressionOrder,
                        Regression =
                            Common.Data.ByteSerialization.GetDataContractByBytes<ExpressionRegression>(m.Regression),
                        AllowTemperatureExtrapolation = m.AllowTemperatureExtrapolation,
                        ForceZeroZero = m.ForceZeroZero,
                        TemperatureExtrapolationMax = m.TemperatureExtrapolationMax,
                        TemperatureExtrapolationMin = m.TemperatureExtrapolationMin,
                        RSquared = m.RSquared,
                        Density = m.Density,
                        NonLinear = m.NonLinear,
                        LinearElasticModulus = m.LinearElasticModulus,
                        Data = new List<MaterialData>(m.MaterialData.Select(md => new MaterialData
                        {
                            Id = md.Id,
                            Index = md.Index,
                            Strain = md.Strain,
                            Stress = md.Stress,
                            Temperature = md.Temperature
                        }).ToList())
                    })
                    .FirstOrDefault();
            }
        }

        [Cache]
        public Material GetMaterialByDesignSectionId(Guid caseId)
        {
            using (var context = new ProjectDbContext(FileName))
            {
                var result = context.Materials.Where(m => m.DesignSections.Any(c => c.Id == caseId))
                    .AsEnumerable()
                    .Select(m => new Material
                    {
                        Id = m.Id,
                        Description = m.Description,
                        Name = m.Name,
                        RegressionOrder = m.RegressionOrder,
                        Regression =
                            Common.Data.ByteSerialization.GetDataContractByBytes<ExpressionRegression>(m.Regression),
                        AllowTemperatureExtrapolation = m.AllowTemperatureExtrapolation,
                        ForceZeroZero = m.ForceZeroZero,
                        TemperatureExtrapolationMax = m.TemperatureExtrapolationMax,
                        TemperatureExtrapolationMin = m.TemperatureExtrapolationMin,
                        RSquared = m.RSquared,
                        Density = m.Density,
                        NonLinear = m.NonLinear,
                        LinearElasticModulus = m.LinearElasticModulus,
                        Data = new List<MaterialData>(m.MaterialData.Select(md => new MaterialData
                        {
                            Id = md.Id,
                            Index = md.Index,
                            Strain = md.Strain,
                            Stress = md.Stress,
                            Temperature = md.Temperature
                        }).ToList())
                    })
                    .FirstOrDefault();

                if (result != null)
                    foreach (var data in result.Data)
                    {
                        CachingServices.CurrentContext.AddDependency(data);
                    }

                return result;
            }
        }

        public string GetNewName()
        {
            using (var context = new ProjectDbContext(FileName))
            {
                return "New Material " + (context.Materials.Count(m => m.Name.StartsWith("New Material")) + 1);
            }
        }

        public void UpsertMaterial(Material material)
        {
            using (var context = new ProjectDbContext(FileName))
            {
                var materialEntity = context.Materials.Where(m => m.Id == material.Id).FirstOrDefault();

                if (materialEntity == null)
                {
                    materialEntity = new UltraBend.Databases.Project.Material()
                    {
                        Id = material.Id
                    };
                    context.Materials.Add(materialEntity);
                    Invalidate(materialEntity.Id);
                }

                materialEntity.Description = material.Description;
                materialEntity.Name = material.Name;
                materialEntity.Regression = Common.Data.ByteSerialization.GetBytesByDataContract(material.Regression);
                materialEntity.RegressionOrder = material.RegressionOrder;
                materialEntity.ForceZeroZero = material.ForceZeroZero;
                materialEntity.RSquared = material.RSquared;
                materialEntity.AllowTemperatureExtrapolation = material.AllowTemperatureExtrapolation;
                materialEntity.TemperatureExtrapolationMin = material.TemperatureExtrapolationMin;
                materialEntity.TemperatureExtrapolationMax = material.TemperatureExtrapolationMax;
                materialEntity.Density = material.Density;
                materialEntity.NonLinear = material.NonLinear;
                materialEntity.LinearElasticModulus = material.LinearElasticModulus;

                // drop data and re-add it
                var existingData = context.MaterialDatas.Where(md => md.MaterialId == material.Id).ToList();
                context.MaterialDatas.RemoveRange(existingData);
                foreach (var data in existingData)
                {
                    CachingServices.Invalidation.Invalidate(new MaterialData {Id = data.Id});
                }

                // add data back
                context.MaterialDatas.AddRange(material.Data.Select(md => new Databases.Project.MaterialData()
                {
                    Id = Guid.NewGuid(),
                    Index = md.Index,
                    Strain = md.Strain,
                    Stress = md.Stress,
                    Temperature = md.Temperature,
                    MaterialId = materialEntity.Id
                }));

                context.SaveChanges();

                Invalidate(materialEntity.Id);
            }
            OnRepositoryUpdated(this);
        }

        public static void PerformRegression(Material material)
        {
            if (material.Regression == null)
                material.Regression = new ExpressionRegression();
            material.Regression.Order = material.RegressionOrder;

            material.Regression.x = new[]
            {
                material.Data.Select(i => i.Strain).ToArray(),
                material.Data.Select(i => i.Temperature).ToArray()
            };
            material.Regression.y = material.Data.Select(i => i.Stress).ToArray();

            // when forcing to zero we also want to include 0,0 so that with one data point
            if (material.ForceZeroZero && (material.Regression.x[0].Length == 0 || material.Regression.x[1].Length == 0 || (!material.Regression.x.Any(i => Math.Abs(i[0]) < double.Epsilon) && !material.Regression.y.Any(i => Math.Abs(i) < double.Epsilon))))
            {
                material.Regression.x = new[]
                {
                    material.Data.Select(i => i.Strain).Concat(new []{0.0}).ToArray(),
                    material.Data.Select(i => i.Temperature).Concat(new []{ material.Data.Select(j => j.Temperature).FirstOrDefault()}).ToArray(),
                };
                material.Regression.y = material.Data.Select(i => i.Stress).Union(new[] { 0.0 }).ToArray();
            }

            if (material.Regression.x[0].Length > 0)
            {
                material.Regression.Compute(null, 1E-3, new List<string>() { "ε", "T" }, "σ", material.ForceZeroZero ? new[] { 0 } : null);
            }
        }
    }
}
