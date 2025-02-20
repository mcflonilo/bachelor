using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Statistics;
using MathNet.Symbolics;

namespace UltraBend.Common.Math
{
    [DataContract(IsReference = false, Name = "ExpressionRegression", Namespace = "http://www.ultradeep.com/")]
    [KnownType(typeof(RegressionCoefficient))]
    public class ExpressionRegression
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        [DataMember]
        public double RSquared { get; set; }

        [DataMember]
        public double[][] x { get; set; }

        [DataMember]
        public double[] y { get; set; }

        [DataMember]
        public List<string> VariableNames { get; private set; }

        [DataMember]
        public string FunctionName { get; private set; }

        [DataMember]
        public int Order;

        [DataMember]
        public List<RegressionCoefficient> Coefficients { get; set; } = new List<RegressionCoefficient>();

        public ExpressionRegression(int order = 100)
        {
            Order = order;
            VariableNames = new List<string>();
        }


        private List<RegressionCoefficient> GetBasis(string variableName, int order, double referenceValue, int[] skipOrders = null)
        {
            int currentOrder = 0;
            var basis = new List<RegressionCoefficient>();
            for (var i = currentOrder; currentOrder < order; i++)
            {
                if (skipOrders == null || skipOrders.IndexOf(i) < 0)
                {
                    basis.Add(new RegressionCoefficient(Infix.ParseOrThrow($"({variableName}^{i})"), referenceValue));
                    currentOrder++;
                }
            }
            return basis;
        }

        public void Compute(int? order = null, double coeffThreshold = 1E-6, List<string> variableNames = null, string functionName = "f", int[] skipOrders = null)
        {
            FunctionName = functionName;
            VariableNames = variableNames;

            if (VariableNames == null)
            {
                VariableNames = new List<string>();
                for (var d = 0; d < x.Length; d++)
                    VariableNames.Add($"X{d}");
            }

            var uniqueData = GetUniqueData();

            Coefficients.Clear();

            var dimensions = uniqueData.Where(i => i.Distinct().Length > 1).Count();
            
            // limit the order to the DOF to prevent ill-conditioning
            var maxOrder = Convert.ToInt32(System.Math.Floor(System.Math.Pow(uniqueData[0].Distinct().Length, 1.0 / dimensions))) - (skipOrders?.Length ?? 0);
            if (order == null || maxOrder < order)
                order = System.Math.Min(Order, maxOrder);


            // get the first dimension's basis function
            var dim0 = GetBasis(VariableNames[0], System.Math.Min(order.Value, uniqueData[0].Distinct().Length + (skipOrders?.Length ?? 0)), uniqueData[0].Max(), skipOrders);
            foreach(var coeff in dim0)
                Coefficients.Add(coeff);

            // tensor product each additional dimension
            for (var d = 1; d < uniqueData.Length; d++)
            {
                if (uniqueData[d].Distinct().Length > 1)
                {
                    int count = Coefficients.Count;
                    var dimd = GetBasis(VariableNames[d],
                        System.Math.Min(order.Value, uniqueData[d].Distinct().Length + (skipOrders?.Length ?? 0)), uniqueData[d].Max(), skipOrders);
                    for (var i = 0; i < count; i++)
                    {
                        for (var j = 0; j < dimd.Count; j++)
                        {
                            Coefficients.Add(new RegressionCoefficient(
                                Coefficients[i].GetExpression() * dimd[j].GetExpression(), Coefficients[i].Scaling,
                                dimd[j].Scaling));
                        }
                    }

                    // remove the old 'reference' values used in the tensor product
                    for (var i = 0; i < count; i++)
                        Coefficients.RemoveAt(0);
                }
            }

            if (Coefficients.Count > 0)
            {
                LeastSquaresFit(uniqueData);

                // remove zeros
                var maxValue = Coefficients.Max(c => System.Math.Abs(c.Value));
                var coefficientsToRemove = Coefficients.Where(c => System.Math.Abs(c.Value / maxValue) < coeffThreshold)
                    .ToList();
                foreach (var coeff in coefficientsToRemove)
                    Coefficients.Remove(coeff);

                if (coefficientsToRemove.Any())
                    LeastSquaresFit(uniqueData);

                ComputeRSquared();
            }
        }

        private void LeastSquaresFit(double[][] uniqueData)
        {
            var dataCount = uniqueData[0].Length;

            var X = new Dictionary<string, FloatingPoint>();
            var A = Matrix.Zeros(dataCount, Coefficients.Count);
            for (var i = 0; i < dataCount; i++)
            {
                // get data point
                for (var d = 0; d < uniqueData.Length; d++)
                    X[VariableNames[d]] = uniqueData[d][i];

                for (var j = 0; j < Coefficients.Count; j++)
                {
                    A[i, j] = Coefficients[j].Eval(X);
                }
            }

            var a = A.PseudoInverse().Dot(y);
            for (var i = 0; i < Coefficients.Count; i++)
                Coefficients[i].Value = a[i];
        }

        private double[][] GetUniqueData()
        {
            var uniqueData = x.Where(d => d.Any() && d.DistinctCount() > 0).ToArray();

            if (uniqueData.Length == 0)
                throw new Exception("Insufficient data");

            if (uniqueData.Any(d => d.Length != uniqueData[0].Length))
                throw new Exception("Inconsistent dimension lengths");

            if (uniqueData[0].Length != y.Length)
                throw new Exception("Inconsistent length between x and y");
            return uniqueData;
        }

        public double Eval(double[] x)
        {
            var X = new Dictionary<string, FloatingPoint>();
            for (var i = 0; i < x.Length; i++)
                X[VariableNames[i]] = x[i];

            double result = 0;
            for (var i = 0; i < Coefficients.Count; i++)
                result += Coefficients[i].Value * Coefficients[i].Eval(X);

            return result;
        }

        public string RegressionEquation()
        {
            var expression = Expression.Zero;
            for (var i = 0; i < Coefficients.Count; i++)
                expression += Coefficients[i].Coefficient * Coefficients[i].GetExpression();
            return FunctionName + "(" + string.Join(",", VariableNames.ToArray()) + ") = " + Infix.Format(expression);
        }

        private void ComputeRSquared()
        {
            var uniqueData = GetUniqueData();

            var mean = y.Mean();

            var num = 0.0;
            var den = 0.0;
            var dataPoint = new double[uniqueData.Length];
            for (var i = 0; i < uniqueData[0].Length; i++)
            {
                // get data point
                for (var d = 0; d < uniqueData.Length; d++)
                    dataPoint[d] = uniqueData[d][i];
                
                num += System.Math.Pow(y[i] - Eval(dataPoint), 2);
                den += System.Math.Pow(y[i] - mean, 2);
            }

            RSquared = 1.0 - num / den;

            if (double.IsInfinity(RSquared) || double.IsNaN(RSquared))
                RSquared = -1;
        }
    }
}
