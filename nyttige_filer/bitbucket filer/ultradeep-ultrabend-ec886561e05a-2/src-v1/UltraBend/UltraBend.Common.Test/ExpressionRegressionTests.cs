using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using MathNet.Symbolics;
using NUnit.Framework;
using UltraBend.Common.Math;
using Assert = NUnit.Framework.Assert;

namespace UltraBend.Common.Test
{
    [TestFixture]
    public class ExpressionRegressionTests
    {

        [Test]
        public void ThrowsOnNoData()
        {
            var regression = new ExpressionRegression();
            regression.x = new[] {new double[] { }};
            regression.y = new double[] { };
            var ex = Assert.Throws<Exception>(() => { regression.Compute(2); });

            Assert.AreEqual("Insufficient data", ex.Message);
        }

        [Test]
        public void ThrowsOnUnbalancedData()
        {
            var regression = new ExpressionRegression();
            regression.x = new[] { new double[] { 2.0, 3.0 }, new double[] { 1.0 } };
            regression.y = new double[] { };
            var ex = Assert.Throws<Exception>(() => { regression.Compute(2); });

            Assert.AreEqual("Inconsistent dimension lengths", ex.Message);
        }

        [Test]
        public void ThrowsOnXLengthNotMatchingYLength()
        {
            var regression = new ExpressionRegression();
            regression.x = new[] { new double[] { 2.0, 3.0 }, new double[] { 1.0, 1.0 } };
            regression.y = new double[] { };
            var ex = Assert.Throws<Exception>(() => { regression.Compute(2); });

            Assert.AreEqual("Inconsistent length between x and y", ex.Message);
        }

        [Test]
        public void LinearFitWithConstantAutomaticallyRemoved()
        {
            var regression = new ExpressionRegression();
            regression.x = new[] { new[] {1.0, 2, 3}};
            regression.y = new[] {1.0, 2, 3};
            regression.Compute(2);

            WriteResult(regression);
            Assert.AreEqual(1, regression.Coefficients.Count);
            Assert.AreEqual(1, regression.RSquared);

        }

        private static void WriteResult(ExpressionRegression regression)
        {
            foreach (var coeff in regression.Coefficients)
            {
                Console.WriteLine(Infix.Format(coeff.GetExpression()) + " " + coeff.Coefficient);
            }
            Console.WriteLine(regression.RegressionEquation());
            Console.WriteLine($"R^2:    {regression.RSquared}");
        }

        [Test]
        public void FitsPolynomial()
        {
            var regression = new ExpressionRegression();
            regression.x = new[] { new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1 } };
            regression.y = regression.x[0].Select(x => 1.1 + 3.2*System.Math.Pow(x,3)).ToArray();

            regression.Compute(5);

            WriteResult(regression);
            Assert.AreEqual(2, regression.Coefficients.Count);
            Assert.That(regression.Coefficients[0].Coefficient, Is.EqualTo(1.1).Within(1E-6));
            Assert.That(regression.Coefficients[1].Coefficient, Is.EqualTo(3.2).Within(1E-6));
            Assert.AreEqual(1, regression.RSquared);
        }

        [Test]
        public void FitsPolynomialAndSetsRSquaredToNegativeInsteadOfNanInf()
        {
            var regression = new ExpressionRegression();
            regression.x = new[] { new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1 } };
            regression.y = regression.x[0].Select(x => double.PositiveInfinity + 3.2 * System.Math.Pow(x, 3)).ToArray();

            regression.Compute(5);

            WriteResult(regression);
            Assert.AreEqual(-1, regression.RSquared);
        }

        [Test]
        public void FitsXYPolynomial()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1 },
                new[] { 4.1, 6.3, 7.5, 9, 11, 14, 200, 212, 312 },
            };
            regression.y = new double[regression.x[0].Length];
            for (var i = 0; i < regression.y.Length; i++)
                regression.y[i] = 1.6 + 3.6 * regression.x[0][i] * regression.x[1][i];

            regression.Compute(2);

            WriteResult(regression);
            Assert.AreEqual(2, regression.Coefficients.Count);
            Assert.That(regression.Coefficients[0].Coefficient, Is.EqualTo(1.6).Within(1E-6));
            Assert.That(regression.Coefficients[1].Coefficient, Is.EqualTo(3.6).Within(1E-6));
            Assert.AreEqual(1, regression.RSquared);

        }

        [Test]
        public void FitsXYPolynomialReducingOrderAutomatically()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1 },
                new[] { 4.1, 6.3, 7.5, 9, 11, 14, 200, 212, 312 },
            };
            regression.y = new double[regression.x[0].Length];
            for (var i = 0; i < regression.y.Length; i++)
                regression.y[i] = 1.6 + 3.6 * regression.x[0][i] * regression.x[1][i];

            regression.Compute(10);

            WriteResult(regression);
            Assert.AreEqual(2, regression.Coefficients.Count);
            Assert.That(regression.Coefficients[0].Coefficient, Is.EqualTo(1.6).Within(1E-6));
            Assert.That(regression.Coefficients[1].Coefficient, Is.EqualTo(3.6).Within(1E-6));
            Assert.AreEqual(1, regression.RSquared);
        }

        [Test]
        public void FitsXYPolynomialIgnoringSingularZ()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1 },
                new[] { 4.1, 6.3, 7.5, 9, 11, 14, 200, 212, 312 },
                new[] { 1.0, 1, 1, 1, 1, 1, 1, 1, 1 }
            };
            regression.y = new double[regression.x[0].Length];
            for (var i = 0; i < regression.y.Length; i++)
                regression.y[i] = 1.6 + 3.6 * regression.x[0][i] * regression.x[1][i];

            regression.Compute(2);

            WriteResult(regression);
            Assert.AreEqual(2, regression.Coefficients.Count);
            Assert.That(regression.Coefficients[0].Coefficient, Is.EqualTo(1.6).Within(1E-6));
            Assert.That(regression.Coefficients[1].Coefficient, Is.EqualTo(3.6).Within(1E-6));
            Assert.AreEqual(1, regression.RSquared);
        }

        [Test]
        public void FitsXZPolynomialIgnoringSingularY()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1 },
                new[] { 1.0, 1, 1, 1, 1, 1, 1, 1, 1 },
                new[] { 4.1, 6.3, 7.5, 9, 11, 14, 200, 212, 312 }
            };
            regression.y = new double[regression.x[0].Length];
            for (var i = 0; i < regression.y.Length; i++)
                regression.y[i] = 1.6 + 3.6 * regression.x[0][i] * regression.x[1][i];

            regression.Compute(2);

            WriteResult(regression);
            Assert.AreEqual(2, regression.Coefficients.Count);
            Assert.That(regression.Coefficients[0].Coefficient, Is.EqualTo(1.6).Within(1E-6));
            Assert.That(regression.Coefficients[1].Coefficient, Is.EqualTo(3.6).Within(1E-6));
            Assert.AreEqual(1, regression.RSquared);
        }

        [Test]
        public void FitsXYZPolynomial()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1, 13, 14, 15, 16 },
                new[] { 1.0, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 4 },
                new[] { 4.1, 6.3, 7.5, 9, 11, 14, 200, 212, 312, 313, 314, 315, 316 }
            };
            regression.y = new double[regression.x[0].Length];
            for (var i = 0; i < regression.y.Length; i++)
                regression.y[i] = 1.6 + 3.6 * regression.x[0][i] * regression.x[1][i] + 4.2 * regression.x[1][i] * regression.x[2][i];

            regression.Compute(2);

            WriteResult(regression);
            Assert.AreEqual(3, regression.Coefficients.Count);
            Assert.That(regression.Coefficients[0].Coefficient, Is.EqualTo(1.6).Within(1E-6));
            Assert.That(regression.Coefficients[2].Coefficient, Is.EqualTo(3.6).Within(1E-6));
            Assert.That(regression.Coefficients[1].Coefficient, Is.EqualTo(4.2).Within(1E-6));
            Assert.AreEqual(1, regression.RSquared);
        }

        [Test]
        public void FitsXYZPolynomialWithAutomaticOrder()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1, 13, 14, 15, 16 },
                new[] { 1.0, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 4 },
                new[] { 4.1, 6.3, 7.5, 9, 11, 14, 200, 212, 312, 313, 314, 315, 316 }
            };
            regression.y = new double[regression.x[0].Length];
            for (var i = 0; i < regression.y.Length; i++)
                regression.y[i] = 1.6 + 3.6 * regression.x[0][i] * regression.x[1][i] + 4.2 * regression.x[1][i] * regression.x[2][i];

            regression.Compute();

            WriteResult(regression);
            Assert.AreEqual(3, regression.Coefficients.Count);
            Assert.That(regression.Coefficients[0].Coefficient, Is.EqualTo(1.6).Within(1E-6));
            Assert.That(regression.Coefficients[2].Coefficient, Is.EqualTo(3.6).Within(1E-6));
            Assert.That(regression.Coefficients[1].Coefficient, Is.EqualTo(4.2).Within(1E-6));
            Assert.AreEqual(1, regression.RSquared);
        }

        [Test]
        public void FitsTemperatureSampleData()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new []
                {
                    0.0, 0.005, 0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1,
                    0.0, 0.005,  0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1,
                    0.0, 0.005,  0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1
                },
                new []
                {
                    2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02,
                    297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55,
                    303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95
                }
            };
            regression.y =
                new[]
                {
                    0.00E+00, 1.40E+06, 2.57E+06,3.61E+06, 4.55E+06, 5.36E+06, 6.03E+06, 6.59E+06, 7.02E+06, 7.37E+06, 7.67E+06, 7.92E+06, 8.13E+06, 8.31E+06, 8.47E+06, 8.61E+06, 8.74E+06, 8.86E+06, 8.96E+06, 9.06E+06, 9.16E+06,
                    0.00E+00, 1.22E+06, 2.27E+06, 3.20E+06, 4.04E+06, 4.77E+06, 5.39E+06, 5.90E+06, 6.30E+06, 6.63E+06, 6.92E+06, 7.16E+06, 7.37E+06, 7.55E+06, 7.70E+06, 7.85E+06, 7.98E+06, 8.10E+06, 8.21E+06, 8.32E+06, 8.42E+06,
                    0.00E+00, 1.10E+06, 2.06E+06, 2.91E+06, 3.69E+06, 4.37E+06, 4.95E+06, 5.42E+06, 5.81E+06,  6.12E+06, 6.40E+06, 6.64E+06, 6.84E+06, 7.03E+06, 7.18E+06, 7.33E+06, 7.47E+06, 7.59E+06, 7.71E+06, 7.81E+06, 7.92E+06,
                };

            regression.Compute(null, 1E-3, new List<string>(){ "ε", "T" }, "σ");
            WriteResult(regression);
        }

        [Test]
        public void FitsTemperatureSampleSingleTemperatureData()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new []
                {
                    0.0, 0.005, 0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1,
                    //0.0, 0.005,  0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1,
                    //0.0, 0.005,  0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1
                },
                new []
                {
                    2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02,
                    //297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55,
                    //303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95
                }
            };
            regression.y =
                new[]
                {
                    0.00E+00, 1.40E+06, 2.57E+06,3.61E+06, 4.55E+06, 5.36E+06, 6.03E+06, 6.59E+06, 7.02E+06, 7.37E+06, 7.67E+06, 7.92E+06, 8.13E+06, 8.31E+06, 8.47E+06, 8.61E+06, 8.74E+06, 8.86E+06, 8.96E+06, 9.06E+06, 9.16E+06,
                    //0.00E+00, 1.22E+06, 2.27E+06, 3.20E+06, 4.04E+06, 4.77E+06, 5.39E+06, 5.90E+06, 6.30E+06, 6.63E+06, 6.92E+06, 7.16E+06, 7.37E+06, 7.55E+06, 7.70E+06, 7.85E+06, 7.98E+06, 8.10E+06, 8.21E+06, 8.32E+06, 8.42E+06,
                    //0.00E+00, 1.10E+06, 2.06E+06, 2.91E+06, 3.69E+06, 4.37E+06, 4.95E+06, 5.42E+06, 5.81E+06,  6.12E+06, 6.40E+06, 6.64E+06, 6.84E+06, 7.03E+06, 7.18E+06, 7.33E+06, 7.47E+06, 7.59E+06, 7.71E+06, 7.81E+06, 7.92E+06,
                };

            regression.Compute(null, 1E-3, new List<string>() { "ε", "T" }, "σ");
            WriteResult(regression);
        }

        [Test]
        public void FitsTemperatureSampleSingleTemperatureDataWithOrder6()
        {
            var regression = new ExpressionRegression();
            regression.x = new[]
            {
                new []
                {
                    0.0, 0.005, 0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1,
                    //0.0, 0.005,  0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1,
                    //0.0, 0.005,  0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.085, 0.09, 0.095, 0.1
                },
                new []
                {
                    2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02, 2.8815E+02,
                    //297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55, 297.55,
                    //303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95, 303.95
                }
            };
            regression.y =
                new[]
                {
                    0.00E+00, 1.40E+06, 2.57E+06,3.61E+06, 4.55E+06, 5.36E+06, 6.03E+06, 6.59E+06, 7.02E+06, 7.37E+06, 7.67E+06, 7.92E+06, 8.13E+06, 8.31E+06, 8.47E+06, 8.61E+06, 8.74E+06, 8.86E+06, 8.96E+06, 9.06E+06, 9.16E+06,
                    //0.00E+00, 1.22E+06, 2.27E+06, 3.20E+06, 4.04E+06, 4.77E+06, 5.39E+06, 5.90E+06, 6.30E+06, 6.63E+06, 6.92E+06, 7.16E+06, 7.37E+06, 7.55E+06, 7.70E+06, 7.85E+06, 7.98E+06, 8.10E+06, 8.21E+06, 8.32E+06, 8.42E+06,
                    //0.00E+00, 1.10E+06, 2.06E+06, 2.91E+06, 3.69E+06, 4.37E+06, 4.95E+06, 5.42E+06, 5.81E+06,  6.12E+06, 6.40E+06, 6.64E+06, 6.84E+06, 7.03E+06, 7.18E+06, 7.33E+06, 7.47E+06, 7.59E+06, 7.71E+06, 7.81E+06, 7.92E+06,
                };

            regression.Compute(6, 1E-3, new List<string>() { "ε", "T" }, "σ");
            WriteResult(regression);
        }

        [Test]
        public void SerializesAndDeserializes()
        {
            var regression = new ExpressionRegression();
            regression.x = new[] { new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1 } };
            regression.y = regression.x[0].Select(x => 1.1 + 3.2 * System.Math.Pow(x, 3)).ToArray();

            regression.Compute(5);

            WriteResult(regression);
            Assert.AreEqual(2, regression.Coefficients.Count);
            Assert.That(regression.Coefficients[0].Coefficient, Is.EqualTo(1.1).Within(1E-6));
            Assert.That(regression.Coefficients[1].Coefficient, Is.EqualTo(3.2).Within(1E-6));
            Assert.AreEqual(1, regression.RSquared);

            ExpressionRegression deserializedObject = null;

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(ExpressionRegression));
                XmlDictionaryWriter binaryDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream);
                
                serializer.WriteObject(stream, regression);
                
                binaryDictionaryWriter.Flush();

                //stream.Close();
                stream.Position = 0;

                deserializedObject = (ExpressionRegression)serializer.ReadObject(stream);
            }

            regression = null;

            WriteResult(deserializedObject);
            Assert.AreEqual(2, deserializedObject.Coefficients.Count);
            Assert.That(deserializedObject.Coefficients[0].Coefficient, Is.EqualTo(1.1).Within(1E-6));
            Assert.That(deserializedObject.Coefficients[1].Coefficient, Is.EqualTo(3.2).Within(1E-6));
            Assert.AreEqual(1, deserializedObject.RSquared);
        }
    }
}
