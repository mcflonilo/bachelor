using PostSharp.Patterns.Model;
using System;
using System.ComponentModel;
using PostSharp.Patterns.Caching.Dependencies;
using Telerik.WinControls.UI;
using UltraBend.Common;
using UltraBend.Common.Units;

namespace UltraBend.Services.DomainObjects
{
    [Serializable]
    [NotifyPropertyChanged]
    public class MaterialData : IModelId, ICacheDependency
    {
        [Browsable(false)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Browsable(false)]
        public int Index { get; set; }

        [DisplayName("Strain")]
        public double Strain { get; set; }

        [DisplayName("Stress")]
        [TypeConverter(typeof(DimensionTypeConverter<PressureDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double Stress { get; set; }

        [DisplayName("Temperature")]
        [TypeConverter(typeof(DimensionTypeConverter<TemperatureDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double Temperature { get; set; }

        public string DisplayName => $"{Index:000} - {Strain:E2}  {Stress:E2}";

        // note: note used
        public string Name { get; set; }
        public string GetCacheKey()
        {
            return this.Id.ToString();
        }
    }
}