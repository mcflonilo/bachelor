using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra_Bend.Common.Data
{
    public class ProjectInformation
    {
        public ProjectInformation()
        {
            Name = "New Project";
            Client = "New Client";
            DesignerName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }
        public string Name { get; set; }
        public string Client { get; set; }
        public string DesignerName { get; set; }
    }
}
