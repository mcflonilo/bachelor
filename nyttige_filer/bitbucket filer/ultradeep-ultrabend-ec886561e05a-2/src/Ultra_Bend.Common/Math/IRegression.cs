using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra_Bend.Common.Math
{
    public interface IRegression
    {
        int Order { get; set; }
        double RSquared { get; set; }
        double[] a { get; set; }

        void Compute();
        double Eval(double x);
    }
}
