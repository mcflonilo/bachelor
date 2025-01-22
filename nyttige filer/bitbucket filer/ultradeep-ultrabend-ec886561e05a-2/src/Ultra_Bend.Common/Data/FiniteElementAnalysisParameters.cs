using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using Ultra_Bend.Common.Units;
using UltraBend.Common.Units;

namespace Ultra_Bend.Common.Data
{
    public class FiniteElementAnalysisParameters : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }

        public FiniteElementAnalysisParameters()
        {
            ToleranceNorm = 1E-7;
            MaxIterations = 30;
            BasisIncrement = .1;
            MinimumIncrement = .1;
            MaximumIncrement = 1;
            Threads = 16;
            MaximumCurvature = 0.5;
            ElementsInUmbilical = 30;
            ElementsInSections = 100;
        }

        /// <summary>
        ///     Value of displacement norm, used as convergence criterion for the Newton – Rapshon iteration.
        /// </summary>
        [DisplayName("Tolerance Norm")]
        [DefaultValue(1E-7)]
        public double ToleranceNorm { get; set; }

        /// <summary>
        ///     Maximum number of iterations
        /// </summary>
        [DisplayName("Maximum Iterations")]
        [DefaultValue(30)]
        public int MaxIterations { get; set; }

        /// <summary>
        ///     Basis increment (percentage of full load). This should be specified as ‘best estimate’ increment size based on experience.
        /// </summary>
        [DisplayName("Basis Increment")]
        [DefaultValue(0.1)]
        public double BasisIncrement { get; set; }

        /// <summary>
        ///     Minimum increment (percentage of full load).
        /// </summary>
        [DisplayName("Minimum Increment")]
        [DefaultValue(0.1)]
        public double MinimumIncrement { get; set; }

        /// <summary>
        ///     Maximum increment (percentage of full load).
        /// </summary>
        [DisplayName("Maximum Increment")]
        [DefaultValue(1)]
        public double MaximumIncrement { get; set; }

        [DisplayName("Maximum Curvature")]
        [DefaultValue(0.5)]
        [TypeConverter(typeof(DimensionTypeConverter<CurvatureDimension>))]
        [Editor(typeof(PropertyGridTextBoxEditor), typeof(BaseInputEditor))]
        public double MaximumCurvature { get; set; }

        [DisplayName("Thread Count")]
        [DefaultValue(16)]
        public int Threads { get; set; }

        [DisplayName("Points in moment-curvature table")]
        [DefaultValue(32)]
        public int Number { get; set; }

        [DisplayName("Number of FE points in Umbilical")]
        [DefaultValue(30)]
        public int ElementsInUmbilical { get; set; }

        [DisplayName("Number of FE elements per section")]
        [DefaultValue(100)]
        public int ElementsInSections { get; set; }
    }
}
