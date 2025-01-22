using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra_Bend.Common.Data
{
    public class RiserResponses : IValidatableObject
    {
        [DisplayName("Delection Angles"), Category("Normal Riser Response")]
        [Description("Normal Operation (80% utilization) Riser Response deflection angles of the Load Contour")]
        public List<double> NormalDeflectionAngles { get; set; } = new List<double>();
            
        [DisplayName("Umbilical Tensions"), Category("Normal Riser Response")]
        [Description("Normal Operation (80% utilization) Riser Response umbilical tensions for the Load Contour")]
        public List<double> NormalUmbilicalTensions { get; set; } = new List<double>();
        
        [DisplayName("Delection Angles"), Category("Abnormal Riser Response")]
        [Description("Abnormal Operation (100% utilization) Riser Response deflection angles of the Load Contour")]
        public List<double> AbnormalDeflectionAngles { get; set; } = new List<double>();
            
        [DisplayName("Umbilical Tensions"), Category("Abnormal Riser Response")]
        [Description("Abnormal Operation (100% utilization) Riser Response umbilical tensions for the Load Contour")]
        public List<double> AbnormalUmbilicalTensions { get; set; } = new List<double>();
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }
    }
}
