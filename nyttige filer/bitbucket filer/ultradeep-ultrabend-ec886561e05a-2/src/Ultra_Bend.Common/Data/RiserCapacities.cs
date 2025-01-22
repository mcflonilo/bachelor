using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra_Bend.Common.Data
{
    public class RiserCapacities : IValidatableObject
    {
        [DisplayName("Curvature"), Category("Normal Riser Capacity")]
        [Description("Normal Operation (80% utilization) Riser Capacity Curvature of the Load Contour")]
        public List<double> NormalDeflectionCurvature { get; set; } = new List<double>();
            
        [DisplayName("Umbilical Tensions"), Category("Normal Riser Capacity")]
        [Description("Normal Operation (80% utilization) Riser Capacity umbilical tensions for the Load Contour")]
        public List<double> NormalUmbilicalTensions { get; set; } = new List<double>();
        
        [DisplayName("Curvature"), Category("Abnormal Riser Capacity")]
        [Description("Abnormal Operation (100% utilization) Riser Capacity Curvature of the Load Contour")]
        public List<double> AbnormalDeflectionCurvature { get; set; } = new List<double>();
            
        [DisplayName("Umbilical Tensions"), Category("Abnormal Riser Capacity")]
        [Description("Abnormal Operation (100% utilization) Riser Capacity umbilical tensions for the Load Contour")]
        public List<double> AbnormalUmbilicalTensions { get; set; } = new List<double>();
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }
    }
}
