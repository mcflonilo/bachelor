using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching;
using UltraBend.Databases.Project;
using Result = UltraBend.Services.DomainObjects.Result;

namespace UltraBend.Services.Project
{
    public class ResultsService : BaseFileService<ProjectDbContext, Result>
    {
        public ResultsService(string fileName, bool createFile = false)
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
                var entity = context.Results.Where(m => m.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    context.Results.Remove(entity);
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
                return "New Result " + (context.Results.Count(m => m.Name.StartsWith("New Result")) + 1);
            }
        }


        public List<UltraBend.Services.DomainObjects.Result> GetResults()
        {
            var results = new List<DomainObjects.Result>();

            using (var context = new ProjectDbContext(FileName))
            {
                results = new List<UltraBend.Services.DomainObjects.Result>(context.Results
                    .AsEnumerable()
                    .Select(d => new UltraBend.Services.DomainObjects.Result
                    {
                        Id = d.Id,
                        Name = d.Name,
                        JsonOfResult = d.JsonOfResult
                    }).ToList());
            }
            
            return results;
        }

        [Cache]
        public UltraBend.Services.DomainObjects.Result GetResultById(Guid id)
        {
            var result = new DomainObjects.Result();

            using (var context = new ProjectDbContext(FileName))
            {
                result = context.Results
                    .Where(r => r.Id == id)
                    .AsEnumerable()
                    .Select(d => new UltraBend.Services.DomainObjects.Result
                    {
                        Id = d.Id,
                        Name = d.Name,
                        JsonOfResult = d.JsonOfResult
                    }).FirstOrDefault();
            }

            return result;
        }

        public DomainObjects.Result UpsertResult(DomainObjects.Result result)
        {
            Guid id;

            using (var context = new ProjectDbContext((FileName)))
            {
                var resultEntity = context.Results.Where(d => d.Id == result.Id).FirstOrDefault();

                if (resultEntity == null)
                {
                    resultEntity = new UltraBend.Databases.Project.Result()
                    {
                        Id = result.Id
                    };
                    context.Results.Add(resultEntity);
                }

                resultEntity.Name = result.Name;
                resultEntity.JsonOfResult = result.JsonOfResult;

                context.SaveChanges();
                
                id = resultEntity.Id;
            }

            Invalidate(id);

            OnRepositoryUpdated(this);


            return GetResultById(id);
        }
    }
}
