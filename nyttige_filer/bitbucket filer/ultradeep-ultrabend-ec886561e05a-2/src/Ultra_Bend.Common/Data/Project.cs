using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Ultra_Bend.Common.Data
{
    public class Project
    {
        public Project()
        {
            ProjectInformation = new ProjectInformation();
            RiserInformation = new RiserInformation();
            RiserCapacities = new RiserCapacities();
            RiserResponses = new RiserResponses();
            BendStiffener = new BendStiffener();
            FiniteElementAnalysisParameters = new FiniteElementAnalysisParameters();
        }


        public ProjectInformation ProjectInformation { get; set; }

        public RiserInformation RiserInformation { get; set; }

        public RiserCapacities RiserCapacities { get; set; }

        public RiserResponses RiserResponses { get; set; }

        public BendStiffener BendStiffener { get; set; }

        public FiniteElementAnalysisParameters FiniteElementAnalysisParameters { get; set; }

        [Browsable(false)]
        [Description("Materials in the Project")]
        public List<Material> Materials { get; set; }

        public string GetNewMaterialName()
        {
            return $"New material {Materials.Count}";
        }
    }
}
