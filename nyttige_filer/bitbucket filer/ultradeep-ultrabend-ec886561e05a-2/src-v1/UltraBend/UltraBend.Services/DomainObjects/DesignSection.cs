using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Dependencies;
using Telerik.WinControls.UI;
using UltraBend.Common;
using UltraBend.Common.Units;
using UltraBend.Services.Project;

namespace UltraBend.Services.DomainObjects
{
    [Serializable]
    [NotifyPropertyChanged]
    public class DesignSection : IModelId, IValidatableObject, ICacheDependency
    {
        [ReadOnly(true)]
        [Description("Unique identifier for the section")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Description("Display name for the section which does not have to be unique")]
        public string Name { get; set; }

        [ReadOnly(true)]
        [Description("Sorting index of the section")]
        public int Index { get; set; }

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

        [Description("The material for the section")]
        [DisplayName("Material")]
        [TypeConverter(typeof(ModelIdTypeConverter<Material>))]
        public Guid? MaterialId { get;set; }

        [Browsable(false)]
        public Material Material { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (this.Material is null)
            {
                results.Add(new ValidationResult($"The section ({Name}) requires a material"));
            }
            else
            {
                results.AddRange(this.Material.Validate(validationContext));
            }

            if (this.Length <= 0)
            {
                results.Add(new ValidationResult($"Length of section ({Name}) must be positive"));
            }

            if (this.RootOuterDiameter <= 0)
            {
                results.Add(new ValidationResult($"Root outer diameter of section ({Name}) must be positive"));
            }

            if (this.TipOuterDiameter <= 0)
            {
                results.Add(new ValidationResult($"Tip outer diameter of section ({Name}) must be positive"));
            }

            return results;
        }

        public string GetCacheKey()
        {
            return this.Id.ToString();
        }
    }
}
