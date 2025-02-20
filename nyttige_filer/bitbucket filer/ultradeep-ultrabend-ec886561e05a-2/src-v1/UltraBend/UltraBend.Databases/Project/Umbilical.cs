using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases.Project
{
    public class Umbilical
    {
        [Index(IsUnique = true)]
        public Guid Id { get; set; }

        [Index]
        public string Name { get; set; }
        
        public virtual Design Design { get; set; }
        
        [Index("IX_UmbilicalDesign")]
        public Guid? DesignId { get; set; }
        public double Length { get; set; }
        public double BendingStiffness { get; set; }
        public double AxialStiffness { get; set; }
        public double TorsionalStiffness { get; set; }
        public double Mass { get; set; }
        public double Diameter { get; set; }
    }
}
