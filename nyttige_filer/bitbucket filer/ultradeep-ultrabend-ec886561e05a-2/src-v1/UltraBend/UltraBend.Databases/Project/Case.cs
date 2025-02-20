using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases.Project
{
    public class Case
    {
        [Index(IsUnique = true)] public Guid Id { get; set; }

        [Index]
        public string Name { get; set; }

        [Index("IX_CaseDesign")]
        public Guid? DesignId { get; set; }

        public virtual Design Design { get; set; }

        [Index("IX_CaseStudy")]
        public Guid? StudyId { get; set; }

        public virtual Study Study { get; set; }

        public double? DeflectionAngle { get; set; }

        public double? UmbilicalTension { get; set; }

        public double? Temperature { get; set; }

        [Index("IX_CaseResult")]
        public Guid? ResultId { get; set; }

        public virtual Result Result { get; set; }
    }
}
