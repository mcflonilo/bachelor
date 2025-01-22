using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Dependencies;
using UltraBend.Common;

namespace UltraBend.Services.DomainObjects
{
    [Serializable]
    [NotifyPropertyChanged]
    public class Study : IModelId, IValidatableObject, ICacheDependency
    {
        [ReadOnly(true)]
        [Description("Unique identifier for the case")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Description("Display name for the study which does not have to be unique")]
        public string Name { get; set; }

        [Description("The design shared across cases in the study")]
        [DisplayName("Design")]
        [TypeConverter(typeof(DesignIdConverter))]
        public Guid? DesignId { get; set; }


        private Design _design { get; set; }
        [Browsable(false)]
        public Design Design { get { return _design; } set { _design = value; DesignId = _design?.Id; } }


        public Study()
        {
            Cases = new List<Case>();
        }

        public List<Case> Cases { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Cases is null || Cases.Count == 0)
            {
                results.Add(new ValidationResult($"The study ({Name}) requires at least one case"));
            }
            else
            {
                foreach (var @case in Cases)
                {
                    results.AddRange(@case.Validate(validationContext));
                }
            }

            if (this.Design is null)
            {
                results.Add(new ValidationResult($"The study ({Name}) requires a design"));
            }
            else
            {
                results.AddRange(this.Design.Validate(validationContext));
            }

            return results;
        }

        public string GetCacheKey()
        {
            return this.Id.ToString();
        }
    }
}
