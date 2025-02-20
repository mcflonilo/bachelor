using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using SQLite.CodeFirst;

namespace UltraBend.Databases.Project
{
    [AddINotifyPropertyChangedInterface]
    public class ProjectDetails : IEntity
    {
        [Autoincrement]
        public int Id { get; set; }

        [Index("IX_Project_ProjectsName")] // Test for named index.
        public string Name { get; set; }

        public string Description { get; set; }
    }
}