using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace UltraBend.Databases.Material
{
    [AddINotifyPropertyChangedInterface]
    public class Material
    {
        [Index(IsUnique = true)]
        public Guid Id { get; set; }

        [Index]
        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Regression { get; set; }

        public int RegressionOrder { get; set; }

        public virtual ICollection<MaterialData> MaterialData { get; set; }

        public bool ForceZeroZero { get; set; }

        public double RSquared { get; set; }

        public bool AllowTemperatureExtrapolation { get; set; }

        public double TemperatureExtrapolationMin { get; set; }

        public double TemperatureExtrapolationMax { get; set; }
    }
}