using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Dependencies;
using UltraBend.Databases.Project;
using UltraBend.Services.DomainObjects;
using Design = UltraBend.Databases.Project.Design;
using Umbilical = UltraBend.Databases.Project.Umbilical;

namespace UltraBend.Services.Project
{
    public class DesignsService : BaseFileService<ProjectDbContext, UltraBend.Services.DomainObjects.Design>
    {
        public DesignsService(string fileName, bool createFile = false)
        {
            FileName = fileName;

            if (createFile)
                using (new ProjectDbContext(fileName, createFile)) ;
        }

        public void DeleteById(Guid id)
        {
            bool hasUpdate = false;
            using (var context = new ProjectDbContext(FileName))
            {
                var entity = context.Designs.Where(m => m.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    context.Designs.Remove(entity);
                    context.SaveChanges();
                    hasUpdate = true;
                    Invalidate(id);
                }
            }
            if (hasUpdate)OnRepositoryUpdated(this);
        }
        public string GetNewName()
        {
            using (var context = new ProjectDbContext(FileName))
            {
                return "New Bend Stiffener " + (context.Designs.Count(m => m.Name.StartsWith("New Bend Stiffener")) + 1);
            }
        }

        public List<UltraBend.Services.DomainObjects.Design> GetDesigns()
        {
            var materialsService = new MaterialsService(FileName);
            var umbilicalsService = new UmbilicalsService(FileName);
            List<UltraBend.Services.DomainObjects.Design> results;

            using (var context = new ProjectDbContext(FileName))
            {
                results = new List<UltraBend.Services.DomainObjects.Design>(context.Designs
                    .AsEnumerable()
                    .Select(d => new UltraBend.Services.DomainObjects.Design
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Sections = new List<UltraBend.Services.DomainObjects.DesignSection>(d.Sections.Select(ds => new UltraBend.Services.DomainObjects.DesignSection()
                        {
                            Id = ds.Id,
                            Index = ds.Index,
                            Name = ds.Name,
                            Length = ds.Length,
                            RootOuterDiameter = ds.RootOuterDiameter,
                            TipOuterDiameter = ds.TipOuterDiameter,
                            MaterialId = ds.MaterialId
                        }).OrderBy(ds => ds.Index).ToList()),
                        UmbilicalId = d.UmbilicalId
                    }));
            }


            foreach (var design in results)
            {
                if (design.UmbilicalId.HasValue)
                {
                    design.Umbilical = umbilicalsService.GetUmbilicalById(design.UmbilicalId.Value);
                    CachingServices.CurrentContext.AddDependency(design.Umbilical);
                }
                foreach (var section in design.Sections)
                {
                    section.Material = materialsService.GetMaterialByDesignSectionId(section.Id);
                    CachingServices.CurrentContext.AddDependency(section.Material);
                }
            }

            return results;
        }

        [Cache]
        public UltraBend.Services.DomainObjects.Design GetDesignById(Guid id)
        {
            var materialsService = new MaterialsService(FileName);
            var umbilicalsService = new UmbilicalsService(FileName);
            UltraBend.Services.DomainObjects.Design result;

            using (var context = new ProjectDbContext(FileName))
            {
                result = (context.Designs.Where(m => m.Id == id)
                    .AsEnumerable()
                    .Select(d => new UltraBend.Services.DomainObjects.Design
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Sections = new List<UltraBend.Services.DomainObjects.DesignSection>(d.Sections.Select(ds => new UltraBend.Services.DomainObjects.DesignSection()
                        {
                            Id = ds.Id,
                            Index = ds.Index,
                            Name = ds.Name,
                            Length = ds.Length,
                            RootOuterDiameter = ds.RootOuterDiameter,
                            TipOuterDiameter = ds.TipOuterDiameter,
                            MaterialId = ds.MaterialId
                        }).OrderBy(ds => ds.Index).ToList()),
                        UmbilicalId = d.UmbilicalId
                    }))
                    .FirstOrDefault();
            }

            if (result is null) return result;

            if (result.UmbilicalId.HasValue)
            {
                result.Umbilical = umbilicalsService.GetUmbilicalById(result.UmbilicalId.Value);
                CachingServices.CurrentContext.AddDependency(result.Umbilical);
            }
            foreach (var section in result.Sections)
            {
                section.Material = materialsService.GetMaterialByDesignSectionId(section.Id);
                CachingServices.CurrentContext.AddDependency(section.Material);
            }

            return result;
        }

        [Cache]
        public UltraBend.Services.DomainObjects.Design GetDesignByCaseId(Guid caseId)
        {
            var materialsService = new MaterialsService(FileName);
            var umbilicalsService = new UmbilicalsService(FileName);
            UltraBend.Services.DomainObjects.Design result;

            using (var context = new ProjectDbContext(FileName))
            {
                result = (context.Designs.Where(m => m.Cases.Any(c => c.Id == caseId))
                        .AsEnumerable()
                        .Select(d => new UltraBend.Services.DomainObjects.Design
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Sections = new List<UltraBend.Services.DomainObjects.DesignSection>(d.Sections.Select(ds => new UltraBend.Services.DomainObjects.DesignSection()
                            {
                                Id = ds.Id,
                                Index = ds.Index,
                                Name = ds.Name,
                                Length = ds.Length,
                                RootOuterDiameter = ds.RootOuterDiameter,
                                TipOuterDiameter = ds.TipOuterDiameter,
                                MaterialId = ds.MaterialId
                            }).OrderBy(ds => ds.Index).ToList()),
                            UmbilicalId = d.UmbilicalId
                        }))
                    .FirstOrDefault();
            }

            if (result is null) return result;

            if (result.UmbilicalId.HasValue)
            {
                result.Umbilical = umbilicalsService.GetUmbilicalById(result.UmbilicalId.Value);
                CachingServices.CurrentContext.AddDependency(result.Umbilical);
            }
            foreach (var section in result.Sections)
            {
                section.Material = materialsService.GetMaterialByDesignSectionId(section.Id);
                CachingServices.CurrentContext.AddDependency(section.Material);
            }

            return result;
        }

        public DomainObjects.Design UpsertDesign(UltraBend.Services.DomainObjects.Design design)
        {
            Guid id;

            using (var context = new ProjectDbContext(FileName))
            {
                var designEntity = context.Designs.Where(d => d.Id == design.Id).FirstOrDefault();
                if (designEntity != null && designEntity.UmbilicalId != null)
                {
                    designEntity.Umbilical = context.Umbilicals.FirstOrDefault(u => u.Id == designEntity.UmbilicalId);
                }

                if (designEntity == null)
                {
                    designEntity = new Design()
                    {
                        Id = design.Id
                    };
                    context.Designs.Add(designEntity);

                    Invalidate(designEntity.Id);
                }

                designEntity.Name = design.Name;
                
                // drop data and re-add it
                var existingData = context.DesignSections.Where(md => md.DesignId == design.Id).ToList();
                context.DesignSections.RemoveRange(existingData);
                foreach (var data in existingData)
                {
                    CachingServices.Invalidation.Invalidate(new UltraBend.Services.DomainObjects.DesignSection() { Id = data.Id } as ICacheDependency);
                }

                // add data back
                context.DesignSections.AddRange(design.Sections.Select(ds => new Databases.Project.DesignSection()
                {
                    Id = Guid.NewGuid(),
                    Index = ds.Index,
                    Name = ds.Name,
                    TipOuterDiameter = ds.TipOuterDiameter,
                    Length = ds.Length,
                    DesignId = designEntity.Id,
                    RootOuterDiameter = ds.RootOuterDiameter,
                    MaterialId = ds.MaterialId
                }));

                if ((design.Umbilical is null && design.UmbilicalId is null) && designEntity.Umbilical != null)
                {
                    context.Umbilicals.Remove(designEntity.Umbilical);
                    UmbilicalsService.Invalidate(designEntity.Umbilical.Id);
                }

                if (design.Umbilical != null && designEntity.Umbilical == null)
                {
                    var newUmbilical = new Umbilical
                    {
                        DesignId = design.Id,
                        Id = design.Umbilical.Id
                    };
                    designEntity.Umbilical = newUmbilical;
                    context.Umbilicals.Add(newUmbilical);
                    UmbilicalsService.Invalidate(newUmbilical.Id);
                }
                if (design.UmbilicalId != null && designEntity.Umbilical == null)
                {
                    var umbilicalsService = new UmbilicalsService(FileName);

                    var newUmbilical = umbilicalsService.GetUmbilicalById(design.UmbilicalId.Value);
                    designEntity.UmbilicalId = newUmbilical.Id;
                    Invalidate(designEntity.Id);
                }

                if (design.Umbilical != null && designEntity.Umbilical != null)
                {
                    designEntity.Umbilical.AxialStiffness = design.Umbilical.AxialStiffness;
                    designEntity.Umbilical.BendingStiffness = design.Umbilical.BendingStiffness;
                    designEntity.Umbilical.Length = design.Umbilical.Length;
                    designEntity.Umbilical.Mass = design.Umbilical.Mass;
                    designEntity.Umbilical.Diameter = design.Umbilical.Diameter;
                    designEntity.Umbilical.Name = design.Umbilical.Name;
                    designEntity.Umbilical.TorsionalStiffness = design.Umbilical.TorsionalStiffness;

                    UmbilicalsService.Invalidate(designEntity.Umbilical.Id);
                }

                CachingServices.Invalidation.Invalidate(designEntity);
                context.SaveChanges();

                if (designEntity.Umbilical is null && designEntity.UmbilicalId != null)
                {
                    designEntity.Umbilical = context.Umbilicals.Where(u => u.Id == designEntity.UmbilicalId.Value).First();
                    Invalidate(designEntity.Id);
                    context.SaveChanges();
                }

                id = designEntity.Id;
            }

            Invalidate(id);

            OnRepositoryUpdated(this);

            return GetDesignById(id);
        }
    }
}
