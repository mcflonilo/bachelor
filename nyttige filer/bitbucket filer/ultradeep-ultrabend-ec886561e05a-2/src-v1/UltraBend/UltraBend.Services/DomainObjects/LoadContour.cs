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
    public class LoadContour : IModelId, IValidatableObject, ICacheDependency
    {
        [ReadOnly(true)]
        [Description("Unique identifier for the Load Contour")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Description("Display name for the Load Contour")]
        public string Name { get; set; }

        public List<double> DeflectionAngles { get; set; } = new List<double>();

        public List<double> UmbilicalTensions { get; set; } = new List<double>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }

        public string GetCacheKey()
        {
            return this.Id.ToString();
        }
    }
}
