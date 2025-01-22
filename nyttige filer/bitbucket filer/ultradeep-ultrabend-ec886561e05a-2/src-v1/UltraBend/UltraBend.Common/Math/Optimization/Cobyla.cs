using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Optimization;

namespace UltraBend.Common.Math.Optimization
{
    public static class Cobyla
    {
        /// <summary>
        /// Wrapper around the Cobyla solver in Accord.Math.Optimization.  Great for local convergence.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="x0"></param>
        /// <param name="tMax"></param>
        /// <returns></returns>
        static (double[] x_min, double f_min) cobyla(Func<double[], double> f, double[] x0, int tMax = 50)
        {

            var cobyla = new Accord.Math.Optimization.Cobyla(2, f);
            cobyla.Solution = x0;
            cobyla.MaxIterations = 10;
            cobyla.Minimize();
            return (cobyla.Solution, cobyla.Value);
        }
    }
}
