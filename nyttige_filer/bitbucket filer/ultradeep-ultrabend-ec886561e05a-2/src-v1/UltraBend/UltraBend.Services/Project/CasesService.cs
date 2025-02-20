using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Dependencies;
using UltraBend.Common.Math;
using UltraBend.Databases.Project;
using UltraBend.Services.DomainObjects;
using Case = UltraBend.Services.DomainObjects.Case;

namespace UltraBend.Services.Project
{
    public class CasesService : BaseFileService<ProjectDbContext, Case>
    {
        public CasesService(string fileName, bool createFile = false)
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
                var entity = context.Cases.Where(m => m.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    context.Cases.Remove(entity);
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
                return "New Case " + (context.Cases.Count(m => m.Name.StartsWith("New Case")) + 1);
            }
        }

        public List<UltraBend.Services.DomainObjects.Case> GetCases()
        {
            var materialsService = new MaterialsService(FileName);
            var designsService = new DesignsService(FileName);
            var studiesService = new StudiesService(FileName);
            var resultsService = new ResultsService(FileName);

            var results = new List<DomainObjects.Case>();

            using (var context = new ProjectDbContext(FileName))
            {
                results = new List<UltraBend.Services.DomainObjects.Case>(context.Cases
                    .AsEnumerable()
                    .Select(d => new UltraBend.Services.DomainObjects.Case
                    {
                        Id = d.Id,
                        Name = d.Name,
                        DeflectionAngle = d.DeflectionAngle,
                        UmbilicalTension = d.UmbilicalTension,
                        Temperature = d.Temperature,
                        DesignId = d.DesignId,
                        StudyId = d.StudyId,
                        ResultId = d.ResultId
                    }).ToList());
                
            }

            // use external services to get the design and material
            foreach (var result in results)
            {
                //result.Material = materialsService.GetMaterialByCaseId(result.Id);
                result.Design = designsService.GetDesignByCaseId(result.Id);

                CachingServices.CurrentContext.AddDependency(result.Design);

                if (result.StudyId.HasValue)
                {
                    result.Study = studiesService.GetStudyById(result.StudyId.Value);
                    CachingServices.CurrentContext.AddDependency(result.Study);
                }

                if (result.ResultId.HasValue)
                {
                    result.Result = resultsService.GetResultById(result.ResultId.Value);
                    CachingServices.CurrentContext.AddDependency(result.Result);
                }
            }

            return results;
        }

        [Cache]
        public UltraBend.Services.DomainObjects.Case GetCaseById(Guid id)
        {
            //var materialsService = new MaterialsService(FileName);
            var designsService = new DesignsService(FileName);
            var studiesService = new StudiesService(FileName);
            var resultsService = new ResultsService(FileName);

            Case result;

            using (var context = new ProjectDbContext(FileName))
            {
                result = context.Cases
                    .Where(c => c.Id == id)
                    .AsEnumerable()
                    .Select(d => new UltraBend.Services.DomainObjects.Case
                    {
                        Id = d.Id,
                        Name = d.Name,
                        DeflectionAngle = d.DeflectionAngle,
                        UmbilicalTension = d.UmbilicalTension,
                        Temperature = d.Temperature,
                        DesignId = d.DesignId,
                        StudyId = d.StudyId,
                        ResultId = d.ResultId
                    }).FirstOrDefault();
            }

            if (result != null)
            {
                // use external services to get the design and material
                //result.Material = materialsService.GetMaterialByCaseId(result.Id);
                if (result.DesignId.HasValue)
                {
                    result.Design = designsService.GetDesignByCaseId(result.Id);
                    CachingServices.CurrentContext.AddDependency(result.Design);
                }

                if (result.StudyId.HasValue)
                {
                    result.Study = studiesService.GetStudyById(result.StudyId.Value);
                    CachingServices.CurrentContext.AddDependency(result.Study);
                }

                if (result.ResultId.HasValue)
                {
                    result.Result = resultsService.GetResultById(result.ResultId.Value);
                    CachingServices.CurrentContext.AddDependency(result.Result);
                }
            }

            return result;
        }

        public DomainObjects.Case UpsertCase(DomainObjects.Case @case)
        {
            Guid id;

            using (var context = new ProjectDbContext((FileName)))
            {
                var caseEntity = context.Cases.Where(d => d.Id == @case.Id).FirstOrDefault();

                if (caseEntity == null)
                {
                    caseEntity = new UltraBend.Databases.Project.Case()
                    {
                        Id = @case.Id
                    };
                    context.Cases.Add(caseEntity);
                }

                caseEntity.Name = @case.Name;
                caseEntity.DeflectionAngle = @case.DeflectionAngle;
                caseEntity.UmbilicalTension = @case.UmbilicalTension;
                caseEntity.Temperature = @case.Temperature;
                caseEntity.DesignId = @case.DesignId;
                caseEntity.StudyId = @case.StudyId;
                caseEntity.ResultId = @case.ResultId;
                
                context.SaveChanges();

                id = caseEntity.Id;

            }

            Invalidate(id);

            OnRepositoryUpdated(this);
            return GetCaseById(id); ;
        }
    }
}
