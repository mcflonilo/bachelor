using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using Gu.Units;
using PostSharp.Patterns.Caching.Dependencies;
using PostSharp.Patterns.Model;
using Telerik.WinControls.UI;
using UltraBend.Common;
using UltraBend.Common.Units;
using UltraBend.Services.BSEngine.Input;
using UltraBend.Services.Project;
using Force = UltraBend.Services.BSEngine.Input.Force;

namespace UltraBend.Services.DomainObjects
{
    [Serializable]
    [NotifyPropertyChanged]
    public class Case : IModelId, IValidatableObject, ICacheDependency
    {
        [Description("A study (collection of cases) that this belongs to")]
        [DisplayName("Study")]
        [TypeConverter(typeof(StudyIdConverter))]
        public Guid? StudyId { get; set; }

        private Study _study { get; set; }

        [Browsable(false)]
        public Study Study
        {
            get => _study;
            set
            {
                _study = value;
                StudyId = _study?.Id;
            }
        }

        [Description("The design")]
        [DisplayName("Design")]
        [TypeConverter(typeof(DesignIdConverter))]
        public Guid? DesignId { get; set; }


        private Design _design { get; set; }

        [Browsable(false)]
        public Design Design
        {
            get => _design;
            set
            {
                _design = value;
                DesignId = _design?.Id;
            }
        }

        [Description("The last result")]
        [DisplayName("Result")]
        [TypeConverter(typeof(ResultIdConverter))]
        public Guid? ResultId { get; set; }


        private Result _result { get; set; }

        [Browsable(false)]
        public Result Result
        {
            get => _result;
            set
            {
                _result = value;
                ResultId = _result?.Id;
            }
        }

        [DefaultValue(0)]
        [TypeConverter(typeof(DimensionTypeConverter<AngleDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double? DeflectionAngle { get; set; }

        [DefaultValue(1000)]
        [TypeConverter(typeof(DimensionTypeConverter<ForceDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double? UmbilicalTension { get; set; }

        [DefaultValue(273)]
        [TypeConverter(typeof(DimensionTypeConverter<TemperatureDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double? Temperature { get; set; }

        public double? BSMomentAtRootEnd => Result?.Output?.KeyResults?.BSMomentAtRootEnd;
        public double? BSShearForceAtRootEnd => Result?.Output?.KeyResults?.BSShearForceAtRootEnd;
        public double? RiserTensionAtRootEnd => Result?.Output?.KeyResults?.RiserTensionAtRootEnd;
        public double? MaximumBSCurvature => Result?.Output?.KeyResults?.MaximumBSCurvature;
        public double? MaximumBSStrainAtOuterDiameter => Result?.Output?.KeyResults?.MaximumBSStrainAtOuterDiameter;
        public double? MaximumCurvature => Result?.Output?.KeyResults?.MaximumCurvature;
        public double? ZDisplacementOfRiserTip => Result?.Output?.KeyResults?.ZDisplacementOfRiserTip;

        [ReadOnly(true)]
        [Description("Unique identifier for the case")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Description("Display name for the case which does not have to be unique")]
        public string Name { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (Design is null)
                results.Add(new ValidationResult($"The case ({Name}) requires a design"));
            else
                results.AddRange(Design.Validate(validationContext));

            if (DeflectionAngle is null)
                results.Add(new ValidationResult($"The deflection angle must be defined for the case ({Name})"));

            if (UmbilicalTension is null)
                results.Add(new ValidationResult($"The tension must be defined for the case ({Name})"));

            if (Temperature is null)
                results.Add(new ValidationResult($"The temperature must be defined for the case ({Name})"));

            return results;
        }

        public string GetCacheKey()
        {
            return this.Id.ToString();
        }
    }

    public class StudyIdConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Shields the repository against high frequency UI updates.
        /// </summary>
        /// <returns></returns>
        protected List<Study> GetStudies()
        {
            return ExpiringCache<List<Study>>.Cached(MemoryCache.Default,
                "UltraBend.Services.DomainObjects.StudyIdConverter.GetStudies", TimeSpan.FromSeconds(1),
                () =>
                {
                    if (ApplicationState.CurrentProjectTemporaryFile != null)
                    {
                        var studiesService = new StudiesService(ApplicationState.CurrentProjectTemporaryFile);

                        return studiesService.GetStudies();
                    }

                    return new List<Study>();
                }, false);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetStudies().Select(m => m.Name).ToList());
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var material = GetStudies().Where(m => m.Name == value.ToString()).FirstOrDefault();
            return material.Id;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
                if (value is Guid)
                    return GetStudies().Where(m => m.Id == (Guid) value).Select(m => m.Name).FirstOrDefault();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }


    public static class CaseHelpers
    {
        public static BendStiffenerConfiguration GetConfiguration(this Case @case, Design design)
        {
            var matIdMaterialNameMap = new Dictionary<Guid, string>();
            var materialsUsed = new List<Material>();

            var segments = design.Sections.Select(s =>
            {
                if (s.Material is null)
                    throw new NullReferenceException();

                // assign index based name to MAT-ID
                if (s.Material.NonLinear && !matIdMaterialNameMap.ContainsKey(s.Material.Id))
                {
                    matIdMaterialNameMap.Add(s.Material.Id, "MAT" + matIdMaterialNameMap.Count);
                    materialsUsed.Add(s.Material);
                }

                return new Segment
                {
                    Length = s.Length,
                    NumberOfElements =
                        Convert.ToInt32(
                            s.Length / 0.005), //10, // 5 cm lengths? - sensitivity analysis/dynamically reduce
                    OuterDiameter1 = s.RootOuterDiameter,
                    OuterDiameter2 = s.TipOuterDiameter,
                    Density = s.Material.Density, // 2000
                    NonLinear = s.Material.NonLinear,
                    EModulus = !s.Material.NonLinear
                        ? Pressure.From(s.Material.LinearElasticModulus, PressureUnit.Pascals).Kilopascals
                        : default(double), // 10000.0E06
                    MaterialName = s.Material.NonLinear ? matIdMaterialNameMap[s.Material.Id] : string.Empty
                };
            }).ToList();

            var elementRanges = new List<ElementRange>();
            var counter = 1;
            for (var i = 0; i < segments.Count; i++)
            {
                elementRanges.Add(new ElementRange
                {
                    StartElementIndex = counter,
                    EndElementIndex = counter + segments[i].NumberOfElements
                });
                counter += segments[i].NumberOfElements;
            }


            return new BendStiffenerConfiguration
            {
                Name = "Functional testing",
                BendStiffenerData = new BendStiffenerData
                {
                    InnerDiameter = design.Umbilical.Diameter, //0.142,
                    Segments = segments,
                    Materials = materialsUsed.Select(m =>
                    {
                        var minT = m.AllowTemperatureExtrapolation
                            ? m.TemperatureExtrapolationMin
                            : m.Data.Min(d => d.Temperature);
                        var maxT = m.AllowTemperatureExtrapolation
                            ? m.TemperatureExtrapolationMax
                            : m.Data.Max(d => d.Temperature);

                        var minStrain = m.Data.Min(d => d.Strain);
                        var maxStrain = m.Data.Max(d => d.Strain);

                        // todo move to case
                        var temperature = @case.Temperature;
                        if (temperature == null && m.AllowTemperatureExtrapolation)
                            throw new Exception("Temperature required for material");

                        if (m.AllowTemperatureExtrapolation && (temperature > maxT || temperature < minT))
                            throw new Exception("Temperature out of range");

                        // todo move to configuration
                        var dataPoints = 66;

                        return new BSEngine.Input.Material
                        {
                            Name = matIdMaterialNameMap[m.Id],
                            DisplayName = m.Name,
                            StressStrain = Common.Common.GetRange(dataPoints, minStrain, maxStrain)
                                .Select(strain => m.GetStressStrain(strain, temperature))
                                .ToList()
                        };
                    }).ToList()
                },
                UmbilicalData = new UmbilicalData
                {
                    Length = design.Umbilical.Length, //3
                    // todo, sensitivity study
                    NumberOfElements = 100, //Convert.ToInt32(design.Umbilical.Length / 0.01), //30,
                    BendingStiffness = design.Umbilical.BendingStiffness / 1000.0, // 28.0
                    AxialStiffness = design.Umbilical.AxialStiffness / 1000.0, //448000,
                    TorsionalStiffness = design.Umbilical.TorsionalStiffness / 1000.0, // 27.0,
                    Mass = design.Umbilical.Mass //31.4
                },
                ElementPrint = new ElementPrint
                {
                    ElementRanges = elementRanges
                },
                FiniteElementAnalysisParameters = new FiniteElementAnalysisParameters
                {
                    // todo, generalize
                    ToleranceNorm = 1.0E-07,
                    MaxIterations = 30,
                    BasisIncrement = .1,
                    MinimumIncrement = 0.01,
                    MaximumIncrement = 1.0
                },
                CurvatureRange = new CurvatureRange
                {
                    // todo generalize
                    MaximumCurvature = 0.7,
                    Number = 100
                },
                Force = new Force
                {
                    ForceDirection = Angle.From(@case.DeflectionAngle ?? 0, AngleUnit.Radians).Degrees, // 20.8
                    Tension = (@case.UmbilicalTension ?? 0) / 1000.0 // 350 kN
                }
            };
        }
    }

    public class DesignIdConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Shields the repository against high frequency UI updates.
        /// </summary>
        /// <returns></returns>
        protected List<Design> GetDesigns()
        {
            return ExpiringCache<List<Design>>.Cached(MemoryCache.Default,
                "UltraBend.Services.DomainObjects.DesignIdConverter.GetDesigns", TimeSpan.FromSeconds(1),
                () =>
                {
                    if (ApplicationState.CurrentProjectTemporaryFile != null)
                    {
                        var designsService = new DesignsService(ApplicationState.CurrentProjectTemporaryFile);

                        return designsService.GetDesigns();
                    }

                    return new List<Design>();
                }, false);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetDesigns().Select(m => m.Name).ToList());
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var material = GetDesigns().Where(m => m.Name == value.ToString()).FirstOrDefault();
            return material.Id;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
                if (value is Guid)
                    return GetDesigns().Where(m => m.Id == (Guid) value).Select(m => m.Name).FirstOrDefault();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }


    public class ResultIdConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Shields the repository against high frequency UI updates.
        /// </summary>
        /// <returns></returns>
        protected List<Result> GetResults()
        {
            return ExpiringCache<List<Result>>.Cached(MemoryCache.Default,
                "UltraBend.Services.DomainObjects.ResultIdConverter.GetResults", TimeSpan.FromSeconds(1),
                () =>
                {
                    if (ApplicationState.CurrentProjectTemporaryFile != null)
                    {
                        var resultsService = new ResultsService(ApplicationState.CurrentProjectTemporaryFile);

                        return resultsService.GetResults();
                    }

                    return new List<Result>();
                }, false);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetResults().Select(m => m.Name).ToList());
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var result = GetResults().Where(m => m.Name == value.ToString()).FirstOrDefault();
            return result.Id;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
                if (value is Guid)
                    return GetResults().Where(m => m.Id == (Guid) value).Select(m => m.Name).FirstOrDefault();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}