using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases.Project
{
    public class Material
    {
        [Index(IsUnique = true)]
        public Guid Id { get; set; }

        [Index]
        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Regression { get; set; }

        public int RegressionOrder { get; set; }

        public virtual ICollection<MaterialData> MaterialData { get; set; } = new List<MaterialData>();

        public bool ForceZeroZero { get; set; }

        public double RSquared { get; set; }

        public bool AllowTemperatureExtrapolation { get; set; }

        public double TemperatureExtrapolationMin { get; set; }

        public double TemperatureExtrapolationMax { get; set; }

        public double Density { get; set; }
        public bool NonLinear { get; set; }
        public double LinearElasticModulus { get; set; }
        
        public virtual ICollection<DesignSection> DesignSections { get; set; } = new List<DesignSection>();
    }
}
