using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases.Material
{
    public class MaterialData
    {
        [Index(IsUnique = true)]
        public Guid Id { get; set; }

        [Index("IX_MaterialMaterialData")]
        public Guid MaterialId { get; set; }

        public int Index { get; set; }

        public double Strain { get; set; }

        public double Stress { get; set; }

        public double Temperature { get; set; }

        public virtual Material Material { get; set; }
    }
}
