using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math.Optimization;

namespace UltraBend.Common.Math.Optimization
{
    public static class Lipo
    {
        public static bool Verbose = false;

        /// <summary>
        /// Green implementation of LIPO, guaranteed to converge on a global minimum.  Note the fastest to converge, however. Works by
        /// estimating the worst case gradient, and using that to predict the global minimum, and samples there.  Then re-estimates
        /// the worst case gradient, and repeats the process. Local convergence is slow, but global performance is good.
        ///
        /// Handles n dimensions
        ///
        /// Well suited for searching for a global minimum while minimizing function evaluations.
        ///
        /// Use a method such as Cobyla for rapid local convergence with a domain (hyperparameters) around the solution found by this method. 
        /// </summary>
        /// <param name="f">Function to evaluate per iteration.  This is assumed as costly to evaluate, and the method is tuned to evaluate it as little as possible</param>
        /// <param name="hyperParameters">The numerical bounds/domain of each dimension</param>
        /// <param name="n">Random samples to consider per iteration (only 1, the 'best') will be evaluated per iteration.  This is not the number of evaluations per iteration, that is always one.</param>
        /// <param name="tMax">Max number of iterations.  Local convergence is slow, so using high values for this to achieve local precision will not be efficient.</param>
        /// <returns></returns>
        static (double[] x_min, double f_min) adaptive_lipo(Func<double[], double> f, double[,] hyperParameters, int n = 10, int tMax = 50)
        {
            int dimensions = hyperParameters.GetLength(0);

            var x = new List<double[]> { new double[dimensions] };

            var k = new List<double> { 0 };

            int t = 0;

            x[0].GetRandomPoints(hyperParameters, dimensions);

            var fx = new List<double> { -f(x[0]) };


            while (t < tMax)
            {
                if (Verbose)
                    Console.WriteLine($"k[t] = {k.Last()}");

                var r_val = new List<(double[] x, double L_xp)>();


                for (var i = 0; i < n; i++)
                {
                    var xi = new double[dimensions];
                    xi.GetRandomPoints(hyperParameters, dimensions);

                    if (Verbose)
                        xi.Print("xi");
                    var lXpMax = LXpMax(x, xi, fx, k, t);

                    if (Verbose)
                        Console.WriteLine($"L_xp_max = {lXpMax}");
                    r_val.Add((xi, lXpMax));
                }

                var best_L2 = double.MinValue;
                var xi_best = new double[dimensions];

                for (var i = 0; i < r_val.Count; i++)
                {
                    if (r_val[i].L_xp > best_L2)
                    {
                        best_L2 = r_val[i].L_xp;
                        xi_best = r_val[i].x;
                    }
                }

                xi_best.Print("xi_best");

                x.Add(xi_best);
                fx.Add(-f(xi_best));


                var k_max = 0.0;
                for (int i = 0; i < x.Count; i++)
                    for (int j = 0; j < x.Count; j++)
                    {
                        if (i != j)
                        {
                            var l1norm = System.Math.Abs(x[i].Subtract(x[j]).GetL1Norm());

                            if (l1norm > double.Epsilon * 1000)
                            {
                                var kij = System.Math.Abs(fx[i] - fx[j]) / l1norm;
                                k_max = System.Math.Max(k_max, kij);
                            }
                        }
                    }

                if (t > 2)
                {
                    k.Add(k_max);
                }
                else
                {
                    // this is to handle fresh startup well
                    k.Add(0);
                }

                Console.WriteLine();

                t++;
            }

            var indexOfMin = fx.IndexOf(fx.Max());

            return (x[indexOfMin], fx[indexOfMin]);
        }

        private static double LXpMax(List<double[]> x, double[] xi, List<double> fx, List<double> k, int t)
        {
            double lXpMax = double.MinValue;
            for (var index = 0; index < x.Count; index++)
            {
                var l2norm = xi.Subtract(x[index]).GetL2Norm();

                double L_xp = fx[index] - k[t] * l2norm;

                if (Verbose)
                    Console.WriteLine($"xi = {xi[0]}\tx[index] = {x[index][0]}\tL2 = {l2norm}\tL_xp = {L_xp}");

                lXpMax = System.Math.Max(lXpMax, L_xp);
            }

            return lXpMax;
        }


        private static readonly Random Random = new Random(0);

        public static void Print(this double[] x, string name, bool newline = true)
        {
            if (Verbose)
            {
                Console.Write($"{name}=[");
                for (var d = 0; d < x.Length; d++)
                {
                    Console.Write(x[d]);
                    if (d < x.Length - 1)
                        Console.Write(",");
                }

                if (newline)
                    Console.Write("]\r\n");
            }
        }

        public static double[] Subtract(this double[] x, double[] y)
        {
            var val = new double[x.Length];
            for (var d = 0; d < x.Length; d++)
                val[d] = x[d] - y[d];
            return val;
        }

        public static double GetL2Norm(this double[] x)
        {
            double value = 0;
            for (var i = 0; i < x.Length; i++)
            {
                value += x[i] * x[i];
            }

            return System.Math.Sqrt(value);
        }

        public static double GetL1Norm(this double[] x)
        {
            double value = 0;
            for (var i = 0; i < x.Length; i++)
            {
                value += System.Math.Abs(x[i]);
            }
            return value;
        }

        public static void GetRandomPoints(this double[] x, double[,] hyperParameters, int dimensions)
        {
            for (var d = 0; d < dimensions; d++)
            {
                x[d] = Random.NextDouble() * (hyperParameters[d, 1] - hyperParameters[d, 0]) + hyperParameters[d, 0];
            }
        }
    }
}
