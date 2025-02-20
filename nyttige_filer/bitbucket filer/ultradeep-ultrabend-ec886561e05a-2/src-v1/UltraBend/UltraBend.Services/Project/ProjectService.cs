using System;
using System.CodeDom.Compiler;
using System.Linq;
using UltraBend.Databases.Project;

namespace UltraBend.Services.Project
{
    public class ProjectService : BaseFileService<ProjectDbContext>
    {
        //public override event EventHandler RepositoryUpdated;

        public ProjectService(string fileName, bool createFile = false)
        {
            FileName = fileName;

            if (createFile)
                using (new ProjectDbContext(fileName, createFile)) ;
        }

        public ProjectDetails GetProjectDetails()
        {
            bool hasUpdate = false;
            ProjectDetails returnValue;

            using (var context = new ProjectDbContext(FileName))
            {
                if (!context.ProjectDetails.Any())
                {
                    context.ProjectDetails.Add(new ProjectDetails { Id = 1 });
                    context.SaveChanges();
                    hasUpdate = true;
                }
                
                returnValue = context.ProjectDetails.First();
            }

            if (hasUpdate) OnRepositoryUpdated(this);

            return returnValue;
        }

        public void SetProjectDetails(ProjectDetails details)
        {
            using (var context = new ProjectDbContext(FileName))
            {
                if (!context.ProjectDetails.Any())
                {
                    context.ProjectDetails.Add(new ProjectDetails { Id = 1 });
                    context.SaveChanges();
                }

                var entity = context.ProjectDetails.First();
                entity.Name = details.Name;
                entity.Description = details.Description;

                context.SaveChanges();
            }
            
            OnRepositoryUpdated(this);
        }

        public void InitializeNewProject()
        {
            var details = GetProjectDetails();

            details.Name = "New Project";
            details.Description = "";

            SetProjectDetails(details);
        }
    }
}
