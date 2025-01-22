using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;

namespace Ultra_Bend.Common.Data
{
    public class BendStiffener: IValidatableObject
    {
        public BendStiffener()
        {
            Sections = new List<BendStiffenerSection>()
            {
                //new BendStiffenerSection()
                //{
                    
                //}
            };
            BendStiffenerConstraints = new BendStiffenerConstraints();
            var testMaterial = new Material();
        }

        public List<BendStiffenerSection> Sections { get; set; }
        
        [Description("Material")]
        [TypeConverter(typeof(MaterialInstanceTypeConverter))]
        public Material Material { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BendStiffenerConstraints BendStiffenerConstraints { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }
    }
}
