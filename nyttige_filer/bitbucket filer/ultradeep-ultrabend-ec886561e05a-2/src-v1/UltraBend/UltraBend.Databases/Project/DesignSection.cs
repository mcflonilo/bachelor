using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases.Project
{
    public class DesignSection
    {
        
        [Index(IsUnique = true)]
        public Guid Id { get; set; }
        
        [Index("IX_DesignDesignSection")]
        public Guid DesignId { get; set; }

        [Index]
        public string Name { get; set; }

        public int Index { get; set; }
        public double Length { get; set; }
        public double RootOuterDiameter { get; set; }

        public double TipOuterDiameter { get; set; }
        
        [Index("IX_DesignSectionMaterial")]
        public Guid? MaterialId { get; set; }

        public virtual Material Material { get; set; }
    }
}
