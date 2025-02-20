using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using UltraBend.Common.Units;

namespace Ultra_Bend.Common.Data
{
    public class MaterialData
    {
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
    }
}
