using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Reports.DataSources
{
    public class BendStiffenerResult
    {
        public static List<BendStiffenerResult> Data = new List<BendStiffenerResult>();

        public double LengthCoordinate { get; set; }
        public double OuterDiameter { get; set; }
        public double Curvature { get; set; }
        public double Moment { get; set; }
        public double ShearForce { get; set; }
        public double Strain { get; set; }

        public List<BendStiffenerResult> GetData()
        {
            return Data;
        }
    }
}
