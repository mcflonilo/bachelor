using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Telerik.WinControls.UI;
using UltraBend.Common.Units;

namespace Ultra_Bend.Common.Data
{
    public class BendStiffenerSection : IValidatableObject
    {
        public BendStiffenerSection()
        {

        }
        
        [Description("Length of the section")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double Length { get; set; }

        [Description("Root outer diameter")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double RootOuterDiameter { get; set; }

        [Description("Tip outer diameter")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(Telerik.WinControls.UI.PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double TipOuterDiameter { get; set; }
        
        [Description("Optional name for the section")]
        public string Name { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            //if (this.Material is null)
            //{
            //    results.Add(new ValidationResult($"The section requires a material"));
            //}
            //else
            //{
            //    results.AddRange(this.Material.Validate(validationContext));
            //}

            if (this.Length <= 0)
            {
                results.Add(new ValidationResult($"Length of section must be positive"));
            }

            if (this.RootOuterDiameter <= 0)
            {
                results.Add(new ValidationResult($"Root outer diameter of section must be positive"));
            }

            if (this.TipOuterDiameter <= 0)
            {
                results.Add(new ValidationResult($"Tip outer diameter of section must be positive"));
            }

            return results;
        }
    }
}