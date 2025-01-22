using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using UltraBend.Common.Units;

namespace Ultra_Bend.Common.Data
{
    public class RiserInformation : IValidatableObject
    {
        public string RiserIdentification { get; set; }
        
        [DisplayName("Outer Diameter")]
        [DefaultValue(.182)]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double OuterDiameter { get; set; } = 0.182;
        
        [DisplayName("Outer Diameter Tolerance")]
        [DefaultValue(.002)]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double OuterDiameterTolerance { get; set; } = 0.002;
        
        [DisplayName("Mass per Unit Length")]
        [DefaultValue(3.4)]
        [TypeConverter(typeof(DimensionTypeConverter<MassPerLengthStiffnessDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double Mass { get; set; } = 3.4;
        
        [DisplayName("Axial Stiffness")]
        [DefaultValue(547000000.0)]
        [TypeConverter(typeof(DimensionTypeConverter<ForceDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double AxialStiffness { get; set; } = 547000000.0;
        
        [DisplayName("Bending Stiffness")]
        [DefaultValue(86700.0)]
        [TypeConverter(typeof(DimensionTypeConverter<TorsionalStiffnessDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double BendingStiffness { get; set; } = 86700.0;
        
        [DisplayName("Torsional Stiffness")]
        [DefaultValue(74700.0)]
        [TypeConverter(typeof(DimensionTypeConverter<TorsionalStiffnessDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double TorsionalStiffness { get; set; } = 74700.0;
        
        [DisplayName("Riser Length")]
        [DefaultValue(0.5)]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double Length { get; set; } = 0.5;
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.TorsionalStiffness <= 0)
            {
                results.Add(new ValidationResult($"Umbilical ({RiserIdentification}) requires a positive torsional stiffness"));
            }

            if (this.AxialStiffness <= 0)
            {
                results.Add(new ValidationResult($"Umbilical ({RiserIdentification}) requires a positive axial stiffness"));
            }

            if (this.BendingStiffness <= 0)
            {
                results.Add(new ValidationResult($"Umbilical ({RiserIdentification}) requires a positive bending stiffness"));
            }

            if (this.OuterDiameter <= 0)
            {
                results.Add(new ValidationResult($"Umbilical ({RiserIdentification}) requires a positive diameter"));
            }

            if (this.Length <= 0)
            {
                results.Add(new ValidationResult($"Umbilical ({RiserIdentification}) requires a positive length"));
            }

            if (this.Mass <= 0)
            {
                results.Add(new ValidationResult($"Umbilical ({RiserIdentification}) requires a positive mass"));
            }

            return results; 
        }
    }
}
