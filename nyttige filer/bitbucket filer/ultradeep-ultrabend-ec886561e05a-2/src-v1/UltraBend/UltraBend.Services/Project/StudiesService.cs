using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching;
using UltraBend.Databases.Project;

namespace UltraBend.Services.Project
{
    public class StudiesService : BaseFileService<ProjectDbContext, DomainObjects.Study>
    {
        public StudiesService(string fileName, bool createFile = false)
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
                var entity = context.Studies.Where(s => s.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    context.Studies.Remove(entity);
                    context.SaveChanges();
                    hasUpdate = true;
                    Invalidate(id);
                }
            }
            if (hasUpdate) OnRepositoryUpdated(this);
        }
        public string GetNewName()
        {
            using (var context = new ProjectDbContext(FileName))
            {
                return "New Study " + (context.Studies.Count(m => m.Name.StartsWith("New Study")) + 1);
            }
        }

        public List<UltraBend.Services.DomainObjects.Study> GetStudies()
        {
            var studiesService = new StudiesService(FileName);
            var designsService = new DesignsService(FileName);
            var resultsService = new ResultsService(FileName);
            List<UltraBend.Services.DomainObjects.Study> results;

            using (var context = new ProjectDbContext(FileName))
            {
                results = new List<UltraBend.Services.DomainObjects.Study>(context.Studies
                    .AsEnumerable()
                    .Select(s => new UltraBend.Services.DomainObjects.Study
                    {
                        Id = s.Id,
                        Name = s.Name,
                        DesignId = s.DesignId,
                        Cases = new List<UltraBend.Services.DomainObjects.Case>(s.Cases.Select(d => new DomainObjects.Case
                        {
                            Id = d.Id,
                            Name = d.Name,
                            DeflectionAngle = d.DeflectionAngle,
                            UmbilicalTension = d.UmbilicalTension,
                            Temperature = d.Temperature,
                            DesignId = d.DesignId,
                            StudyId = d.StudyId
                        }))
                    }).ToList());

            }

            foreach (var study in results)
            {
                foreach (var @case in study.Cases)
                {
                    CachingServices.CurrentContext.AddDependency(@case);

                    @case.Design = designsService.GetDesignByCaseId(@case.Id);
                    CachingServices.CurrentContext.AddDependency(@case.Design);

                    if (@case.ResultId.HasValue)
                    {
                        @case.Result = resultsService.GetResultById(@case.ResultId.Value);
                        CachingServices.CurrentContext.AddDependency(@case.Result);
                    }
                }

                if (study.DesignId.HasValue)
                {
                    study.Design = designsService.GetDesignById(study.DesignId.Value);
                    CachingServices.CurrentContext.AddDependency(study.Design);
                }
            }

            return results;
        }

        [Cache]
        public UltraBend.Services.DomainObjects.Study GetStudyById(Guid id)
        {
            var studiesService = new StudiesService(FileName);
            var designsService = new DesignsService(FileName);
            var resultsService = new ResultsService(FileName);
            var result = new DomainObjects.Study();

            using (var context = new ProjectDbContext(FileName))
            {
                result = context.Studies
                    .AsEnumerable()
                    .Where(c => c.Id == id)
                    .Select(s => new UltraBend.Services.DomainObjects.Study
                    {
                        Id = s.Id,
                        Name = s.Name,
                        DesignId = s.DesignId,
                        Cases = new List<UltraBend.Services.DomainObjects.Case>(s.Cases.Select(d => new DomainObjects.Case
                        {
                            Id = d.Id,
                            Name = d.Name,
                            DeflectionAngle = d.DeflectionAngle,
                            UmbilicalTension = d.UmbilicalTension,
                            Temperature = d.Temperature,
                            DesignId = d.DesignId,
                            StudyId = d.StudyId,
                            ResultId = d.ResultId
                        }))
                    }).FirstOrDefault();
            }

            if (result != null)
            {
                foreach (var @case in result.Cases)
                {
                    @case.Design = designsService.GetDesignByCaseId(@case.Id);
                    CachingServices.CurrentContext.AddDependency(@case.Design);

                    if (@case.ResultId.HasValue)
                    {
                        @case.Result = resultsService.GetResultById(@case.ResultId.Value);
                        CachingServices.CurrentContext.AddDependency(@case.Result);
                    }
                }

                if (result.DesignId.HasValue)
                {
                    result.Design = designsService.GetDesignById(result.DesignId.Value);
                    CachingServices.CurrentContext.AddDependency(result.Design);
                }
            }

            return result;
        }

        public DomainObjects.Study UpsertStudy(DomainObjects.Study study)
        {
            Guid id;

            using (var context = new ProjectDbContext(FileName))
            {
                var studyEntity = context.Studies.Where(d => d.Id == study.Id).FirstOrDefault();

                if (studyEntity == null)
                {
                    studyEntity = new Study
                    {
                        Id = study.Id
                    };
                    context.Studies.Add(studyEntity);
                }

                studyEntity.Name = study.Name;
                studyEntity.DesignId = study.DesignId;

                var caseService = new CasesService(FileName, false);
                foreach(var @case in study.Cases)
                {
                    caseService.UpsertCase(@case);
                }

                context.SaveChanges();

                id = studyEntity.Id;
            }

            Invalidate(id);

            OnRepositoryUpdated(this);

            return GetStudyById(id);
        }
    }
}
