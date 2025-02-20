using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PostSharp.Patterns.Model;
using Telerik.WinControls.UI;
using Ultra_Bend.Common.Math;
using UltraBend.Common.Units;

namespace Ultra_Bend.Common.Data
{
    public class Material: IValidatableObject
    {
        [Category("Material")]
        [Description("Display name for the material which does not have to be unique")]
        public string Name { get; set; }

        [Category("Material")]
        [Description("Description for the material")]
        public string Description { get; set; }

        [Category("01Material")]
        [DisplayName("Interpolation")]
        [Browsable(false)]
        public ExpressionRegression Regression{ get; set; }
        
        [Category("Interpolation")]
        [Description("The 'order' of the regression")]
        public int RegressionOrder { get; set; }

        [Category("Material")]
        [Browsable(false)]
        public List<MaterialData> Data { get; set; }

        [Category("Interpolation")]
        [DisplayName("ForceZeroZero")]
        [Description("Forces interpolation to go through the origin by excluding the zeroth power term in the interpolation basis")]
        public bool ForceZeroZero { get; set; }

        [Category("Interpolation")]
        [DisplayName("RSquared")]
        [Description("R^2 value for the curve fit against all data points")]
        public double RSquared
        {
            get { return Regression?.RSquared ?? -1; }
            set { } // do nothing
        }

        [Category("Interpolation")]
        [DisplayName("RegressionExpression")]
        [Description("Analytical expression for the regression")]
        [IgnoreAutoChangeNotification]
        public string RegressionExpression {
            get { return Regression != null ? Regression.RegressionEquation() : string.Empty; }
            set { } // do nothing
        }

        [Category("Interpolation")]
        [DisplayName("AllowTemperatureExtrapolation")]
        [Description("Allow temperature extrapolation between the max and min values")]
        public bool AllowTemperatureExtrapolation { get; set; }

        [Category("Interpolation")]
        [DisplayName("TemperatureExtrapolationMin")]
        [Description("Minimum temperature permitted for extrapolation when extrapolation is permitted.  If zero, no minimum bound is used")]
        [TypeConverter(typeof(DimensionTypeConverter<TemperatureDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double TemperatureExtrapolationMin { get; set; }

        [Category("Interpolation")]
        [DisplayName("TemperatureExtrapolationMax")]
        [Description("Maximum temperature permitted for extrapolation when extrapolation is permitted  If zero, no maximum bound is used")]
        [TypeConverter(typeof(DimensionTypeConverter<TemperatureDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double TemperatureExtrapolationMax { get; set; }

        [Category("Material")]
        [DefaultValue(1130.0)]
        [Description("The density of the material")]
        [TypeConverter(typeof(DimensionTypeConverter<DensityDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double Density { get; set; } = 1130.0;

        [Category("Material")]
        public bool NonLinear { get; set; }

        [Category("Material")]
        [DefaultValue(150000000.0)]
        [Description("The linear elastic modulus of the material")]
        [TypeConverter(typeof(DimensionTypeConverter<PressureDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double LinearElasticModulus { get; set; } = 150000000.0;

        [Browsable(false)]
        public (double Stress, double Strain) GetStressStrain(double strain, double? temperature)
        {
            if (AllowTemperatureExtrapolation == false && temperature == null)
                throw new Exception("Temperature required for extrapolation");
            
            if (AllowTemperatureExtrapolation && temperature != null)
                return (Stress: Regression.Eval(new[] {strain, temperature.Value}), Strain: strain);

            if (Data.Select(d => d.Temperature).Distinct().Count() != 1)
                throw new Exception("Ambiguous temperature data");

            return (Stress: Regression.Eval(new[] {strain}), Strain: strain);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // todo: validate material 

            return results;
        }
    }
}
