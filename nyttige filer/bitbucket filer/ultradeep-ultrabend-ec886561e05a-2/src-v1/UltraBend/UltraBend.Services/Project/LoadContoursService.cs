using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching;
using UltraBend.Databases.Project;
using LoadContour = UltraBend.Services.DomainObjects.LoadContour;

namespace UltraBend.Services.Project
{
    public class LoadContoursService : BaseFileService<ProjectDbContext, DomainObjects.LoadContour>
    {
        public LoadContoursService(string fileName, bool createFile = false)
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
                var entity = context.LoadContours.FirstOrDefault(m => m.Id == id);
                if (entity != null)
                {
                    context.LoadContours.Remove(entity);
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
                return "New Load Contour " + (context.LoadContours.Count(m => m.Name.StartsWith("New Load Contour")) + 1);
            }
        }

        public List<DomainObjects.LoadContour> GetLoadContours()
        {
            var results = new List<DomainObjects.LoadContour>();

            using (var context = new ProjectDbContext(FileName))
            {
                results = new List<LoadContour>(
                    context.LoadContours.AsEnumerable()
                        .Select(d => new DomainObjects.LoadContour()
                        {
                            Name = d.Name,
                            Id = d.Id,
                            DeflectionAngles = Common.Data.ByteSerialization.GetDataContractByBytes<List<double>>(d.DeflectionAngleData),
                            UmbilicalTensions = Common.Data.ByteSerialization.GetDataContractByBytes<List<double>>(d.UmbilicalTensionData),
                        }).ToList());

                return results;
            }
        }

        [Cache]
        public DomainObjects.LoadContour GetLoadContourById(Guid id)
        {

            using (var context = new ProjectDbContext(FileName))
            {
                return context.LoadContours.Where(i => i.Id == id)
                    .AsEnumerable()
                    .Select(d => new DomainObjects.LoadContour()
                    {
                        Name = d.Name,
                        Id = d.Id,
                        DeflectionAngles = Common.Data.ByteSerialization.GetDataContractByBytes<List<double>>(d.DeflectionAngleData),
                        UmbilicalTensions = Common.Data.ByteSerialization.GetDataContractByBytes<List<double>>(d.UmbilicalTensionData)
                    }).FirstOrDefault();
            }
        }

        public DomainObjects.LoadContour UpsertLoadContour(DomainObjects.LoadContour LoadContour)
        {

            Guid id;

            using (var context = new ProjectDbContext((FileName)))
            {
                var LoadContourEntity = context.LoadContours.Where(d => d.Id == LoadContour.Id).FirstOrDefault();

                if (LoadContourEntity == null)
                {
                    LoadContourEntity = new Databases.Project.LoadContour
                    {
                        Id = LoadContour.Id
                    };
                    context.LoadContours.Add(LoadContourEntity);
                }

                LoadContourEntity.Name = LoadContour.Name;
                LoadContourEntity.DeflectionAngleData =
                    Common.Data.ByteSerialization.GetBytesByDataContract(LoadContour.DeflectionAngles);
                LoadContourEntity.UmbilicalTensionData =
                    Common.Data.ByteSerialization.GetBytesByDataContract(LoadContour.UmbilicalTensions);

                context.SaveChanges();

                id = LoadContourEntity.Id;
            }

            Invalidate(id);

            OnRepositoryUpdated(this);
            return GetLoadContourById(id);
        }
    }
}
