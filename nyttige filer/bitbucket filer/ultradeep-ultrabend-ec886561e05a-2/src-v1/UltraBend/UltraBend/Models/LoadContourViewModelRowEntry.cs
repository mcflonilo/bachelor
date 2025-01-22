using PostSharp.Patterns.Model;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using UltraBend.Common.Units;

namespace UltraBend.Models
{
    [Serializable]
    [NotifyPropertyChanged]
    public class LoadContourViewModelRowEntry
    {
        [DefaultValue(0)]
        [TypeConverter(typeof(DimensionTypeConverter<AngleDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double DeflectionAngle { get; set; }

        [DefaultValue(1000)]
        [TypeConverter(typeof(DimensionTypeConverter<ForceDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double UmbilicalTension { get; set; }
    }
}
