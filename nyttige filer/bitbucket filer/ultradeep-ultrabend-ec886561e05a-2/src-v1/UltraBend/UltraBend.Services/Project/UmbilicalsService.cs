using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching;
using UltraBend.Databases.Project;

namespace UltraBend.Services.Project
{
    public class UmbilicalsService : BaseFileService<ProjectDbContext, DomainObjects.Umbilical>
    {
        public UmbilicalsService(string fileName, bool createFile = false)
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
                var entity = context.Umbilicals.Where(m => m.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    context.Umbilicals.Remove(entity);
                    context.SaveChanges();
                    hasUpdate = true;
                    Invalidate(id);
                }
            }
            if (hasUpdate) OnRepositoryUpdated(this);
        }

        public List<DomainObjects.Umbilical> GetUmbilicals()
        {
            using (var context = new ProjectDbContext(FileName))
            {
                var result = new List<DomainObjects.Umbilical>(context.Umbilicals
                    .AsEnumerable()
                    .Select(m => new DomainObjects.Umbilical
                    {
                        Id = m.Id,
                        AxialStiffness = m.AxialStiffness,
                        BendingStiffness = m.BendingStiffness,
                        Diameter = m.Diameter,
                        Length = m.Length,
                        Mass = m.Mass,
                        Name = m.Name,
                        TorsionalStiffness = m.TorsionalStiffness
                    })).ToList();

                return result;
            }
        }

        [Cache]
        public DomainObjects.Umbilical GetUmbilicalById(Guid id)
        {
            using (var context = new ProjectDbContext(FileName))
            {
                return new List<DomainObjects.Umbilical>(context.Umbilicals
                    .Where(d => d.Id == id)
                    .AsEnumerable()
                    .Select(m => new DomainObjects.Umbilical
                    {
                        Id = m.Id,
                        AxialStiffness = m.AxialStiffness,
                        BendingStiffness = m.BendingStiffness,
                        Diameter = m.Diameter,
                        Length = m.Length,
                        Mass = m.Mass,
                        Name = m.Name,
                        TorsionalStiffness = m.TorsionalStiffness
                    })).FirstOrDefault();
            }
        }
        public string GetNewName()
        {
            using (var context = new ProjectDbContext(FileName))
            {
                return "New Umbilical " + (context.Materials.Count(m => m.Name.StartsWith("New Umbilical")) + 1);
            }
        }

        public void UpsertUmbilical(DomainObjects.Umbilical umbilical)
        {
            using (var context = new ProjectDbContext(FileName))
            {
                var umbilicalEntity = context.Umbilicals.Where(m => m.Id == umbilical.Id).FirstOrDefault();

                if (umbilicalEntity == null)
                {
                    umbilicalEntity = new Umbilical()
                    {
                        Id = umbilical.Id
                    };
                    context.Umbilicals.Add(umbilicalEntity);
                }

                umbilicalEntity.Name = umbilical.Name;
                umbilicalEntity.Length = umbilical.Length;
                umbilicalEntity.Mass = umbilical.Mass;
                umbilicalEntity.TorsionalStiffness = umbilical.TorsionalStiffness;
                umbilicalEntity.AxialStiffness = umbilical.AxialStiffness;
                umbilicalEntity.BendingStiffness = umbilical.BendingStiffness;
                umbilicalEntity.Diameter = umbilical.Diameter;

                context.SaveChanges();

                Invalidate(umbilicalEntity.Id);
            }
            OnRepositoryUpdated(this);
        }
    }
}
