using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using Accord.Math.Optimization;
using Priority_Queue;

namespace LIPO
{
    class Program
    {
        private const bool Verbose = false;

        static void Main(string[] args)
        {
            int executionCount = 0;

            double[,] oneD_hyperParameters = new double[,] { { -20, 20 } };
            double[,] twoD_hyperParameters = new double[,] { { -20, 20 }, { -20, 20 } };

            //for (var i = 0; i < 10; i++)
            //{
            Func<double[], double> f = x =>
            {
                executionCount++;
                return x[0] * x[0];
            };

            Func<double[], double> f2 = x =>
            {
                executionCount++;
                return x[0] * x[0] + x[1] * x[1] + 5*x[1];
            };

            //var result = adaptive_lipo(100, 10, f2, twoD_hyperParameters);

            //result.x_min.Print("x_min");
            //Console.WriteLine($"f(x_min) = {result.f_min}");
            Cobyla cobyla = new Cobyla(2, f2);
            cobyla.Minimize();
            cobyla.MaxIterations = 10;
            cobyla.Solution.Print("x_min");

            Console.WriteLine($"Executions: {executionCount}");


            Console.ReadKey();


        }

        static (double[] x_min, double f_min) adaptive_lipo(int n, int t_max, Func<double[], double> f, double[,] hyperParameters)
        {
            int seedSize = 3;

            int dimensions = hyperParameters.GetLength(0);

            var x = new List<double[]>{new double[dimensions]};

            var k = new List<double> {0};

            int t = 0, t2 = 0;

            x[0].GetRandomPoints(hyperParameters, dimensions);

            var fx = new List<double> {-f(x[0])};


            while (t < t_max)
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
                    var L_xp_max = LXpMax(x, xi, fx, k, t);
                    //xi.Print("xi");

                    if (Verbose)
                        Console.WriteLine($"L_xp_max = {L_xp_max}");
                    r_val.Add((xi, L_xp_max));
                }

                //if (t > 3 && t % 2 == 0)
                //{
                //    // also consider a mid point
                //    var priorityQueue = new SimplePriorityQueue<(double[], double)>((a, b) => a.CompareTo(b));
                //    for (var index = 0; index < x.Count; index++)
                //    {
                //        priorityQueue.Enqueue((x[index], fx[index]), (float)fx[index]);
                //    }

                //    (var x0, var y0) = priorityQueue.Dequeue();
                //    (var x1, var y1) = priorityQueue.Dequeue();
                //    x0.Print("x0");
                //    x1.Print("x1");
                //    var x_avg = x0.Average(x1);

                //    if (t2 % 2 == 0)
                //    {
                //        x_avg[0] = x0[0];
                //    }
                //    else
                //    {
                //        x_avg[1] = x0[1];
                //    }

                //    t2++;

                //    x_avg.Print("x_avg");
                //    r_val.Clear();
                //    r_val.Add((x_avg, LXpMax(x, x_avg, fx, k, t)));
                //}
                //r_val.Sort((a, b) => b.L_xp.CompareTo(a.L_xp));

                //var best_xi = r_val.First();

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


                //xi_best.Print("best_xi");
                //Console.WriteLine("\tf(x) = " + f(best_xi.x));
                

                xi_best.Print("xi_best");

                x.Add(xi_best);
                fx.Add(-f(xi_best));


                var k_max = 0.0;
                for(int i = 0; i < x.Count; i++)
                for (int j = 0; j < x.Count; j++)
                {
                    if (i != j)
                    {
                        var l1norm = Math.Abs(x[i].Subtract(x[j]).GetL1Norm());
                        //Console.WriteLine(l1norm);
                        if (l1norm > double.Epsilon * 1000)
                        {
                            var kij = Math.Abs(fx[i] - fx[j]) / l1norm;
                            k_max = Math.Max(k_max, kij);
                        }
                    }
                }

                if (t > 2)
                {
                    //Console.WriteLine($"\tk_max = {k_max}");
                    k.Add(k_max);
                }
                else
                {
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
            double L_xp_max = double.MinValue;
            for (var index = 0; index < x.Count; index++)
            {
                var l2norm = xi.Subtract(x[index]).GetL2Norm();

                double L_xp = fx[index] - k[t] * l2norm;

                if (Verbose)
                    Console.WriteLine($"xi = {xi[0]}\tx[index] = {x[index][0]}\tL2 = {l2norm}\tL_xp = {L_xp}");

                L_xp_max = Math.Max(L_xp_max, L_xp);
            }

            return L_xp_max;
        }
    }

    public static class Helpers
    {

        private static Random random = new Random(0);

        public static void Print(this double[] x, string name, bool newline = true)
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

        public static double[] Average(this double[] x, double[] y)
        {
            var val = new double[x.Length];
            for (var d = 0; d < x.Length; d++)
                val[d] = 0.5*(x[d] + y[d]);
            return val;
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

            return Math.Sqrt(value);
        }

        public static double GetL1Norm(this double[] x)
        {
            double value = 0;
            for (var i = 0; i < x.Length; i++)
            {
                value += Math.Abs(x[i]);
            }
            return value;
        }

        public static void GetRandomPoints(this double[] x, double[,] hyperParameters, int dimensions)
        {
            for (var d = 0; d < dimensions; d++)
            {
                x[d] = random.NextDouble() * (hyperParameters[d, 1] - hyperParameters[d, 0]) + hyperParameters[d, 0];
                //Console.WriteLine($"x[{d}]={x[d]}");
            }
            //x.Print("x");
        }
    }
}
