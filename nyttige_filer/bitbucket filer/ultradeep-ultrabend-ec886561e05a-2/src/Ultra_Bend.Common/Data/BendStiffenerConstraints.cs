using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Telerik.WinControls.UI;
using UltraBend.Common.Units;

namespace Ultra_Bend.Common.Data
{
    public class BendStiffenerConstraints : IValidatableObject
    {
        public BendStiffenerConstraints()
        {

        }

        [DisplayName("Root Length")]
        [DefaultValue(.65)]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double RootLength { get; set; }

        [DisplayName("Tip Length")]
        [DefaultValue(.1)]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double TipLength { get; set; }

        [DisplayName("Minimum Root Outer Diameter")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double MinRootOuterDiameter { get; set; }
        
        [DisplayName("Maximum Root Outer Diameter")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double MaxRootOuterDiameter { get; set; }
        
        [DisplayName("Minimum Overall Length")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double MinOverallLength { get; set; }
        
        [DisplayName("Maximum Overall Length")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double MaxOverallLength { get; set; }
        
        [DisplayName("Clearance")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double Clearance { get; set; }
        
        [DisplayName("Minimum Thickness")]
        [TypeConverter(typeof(DimensionTypeConverter<LengthDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double MinThickness {get;set;}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }
    }
}