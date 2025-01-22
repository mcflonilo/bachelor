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
using Gu.Units;
using PostSharp.Patterns.Caching.Dependencies;
using UltraBend.Common;
using UltraBend.Services.BSEngine.Input;
using UltraBend.Services.Project;
using Force = UltraBend.Services.BSEngine.Input.Force;

namespace UltraBend.Services.DomainObjects
{
    [Serializable]
    [NotifyPropertyChanged]
    public class Design : IModelId, IValidatableObject, ICacheDependency
    {
        [ReadOnly(true)]
        [Description("Unique identifier for the design")]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public Design()
        {
            Sections = new List<DesignSection>();
        }
        public List<DesignSection> Sections { get; set; }

        [Description("The umbilical for the design")]
        [DisplayName("Umbilical")]
        [TypeConverter(typeof(UmbilicalIdConverter))]
        public Guid? UmbilicalId { get;set; }
        
        [Browsable(false)]
        public Umbilical Umbilical { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string GetCacheKey()
        {
            return this.Id.ToString();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (this.Sections is null || this.Sections.Count == 0)
            {
                results.Add(new ValidationResult($"The design ({Name}) requires section(s)"));
            }
            else
            {
                foreach (var section in Sections)
                {
                    results.AddRange(section.Validate(validationContext));
                }
            }

            if (this.Umbilical is null)
            {
                results.Add(new ValidationResult("The design requires an umbilical"));
            }
            else
            {
                results.AddRange(this.Umbilical.Validate(validationContext));
            }

            return results;
        }
    }

    public class UmbilicalIdConverter : TypeConverter
    {
        public override Boolean GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
        public override Boolean GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

        /// <summary>
        /// Shields the repository against high frequency UI updates.
        /// </summary>
        /// <returns></returns>
        protected List<Umbilical> GetUmbilicals()
        {
            return ExpiringCache<List<Umbilical>>.Cached(MemoryCache.Default,
                "UltraBend.Services.DomainObjects.UmbilicalIdConverter.GetUmbilicals", TimeSpan.FromSeconds(1),
                () =>
                {
                    if (ApplicationState.CurrentProjectTemporaryFile != null)
                    {
                        var umbilicalsService = new UmbilicalsService(ApplicationState.CurrentProjectTemporaryFile);

                        return umbilicalsService.GetUmbilicals();
                    }

                    return new List<Umbilical>();
                }, false);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetUmbilicals().Select(m => m.Name).ToList());
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var material = GetUmbilicals().Where(m => m.Name == value.ToString()).FirstOrDefault();
            return material.Id;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is Guid)
                {
                    return GetUmbilicals().Where(m => m.Id == (Guid)value).Select(m => m.Name).FirstOrDefault();
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
