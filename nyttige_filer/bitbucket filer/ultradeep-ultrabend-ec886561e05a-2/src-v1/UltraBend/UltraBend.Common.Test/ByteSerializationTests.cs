using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using UltraBend.Common.Data;
using UltraBend.Common.Math;

namespace UltraBend.Common.Test
{
    [TestFixture]
    public class ByteSerializationTests
    {
        [Test]
        public void SerializesAndDeserializesExpressionRegression()
        {
            var regression = new ExpressionRegression();
            regression.x = new[] { new[] { 1.0, 2, 3, 4, 5, 6, 7, 7.5, 12.1 } };
            regression.y = regression.x[0].Select(x => 1.1 + 3.2 * System.Math.Pow(x, 3)).ToArray();

            regression.Compute(5);

            var bytes = ByteSerialization.GetBytesByDataContract(regression);
            var deserialized = ByteSerialization.GetDataContractByBytes<ExpressionRegression>(bytes);

            var strBytes = Encoding.Default.GetString(bytes);

            Assert.AreEqual(2, deserialized.Coefficients.Count);
            Assert.That(deserialized.Coefficients[0].Coefficient, Is.EqualTo(1.1).Within(1E-6));
            Assert.That(deserialized.Coefficients[1].Coefficient, Is.EqualTo(3.2).Within(1E-6));
            Assert.AreEqual(1, deserialized.RSquared);
        }

        [Test]
        public void SerializeAndDeserializeDoubleArray()
        {
            var testArray = new double[] {1.1, 0.0, double.MaxValue, double.MinValue, double.NaN, double.PositiveInfinity, double.NegativeInfinity};

            var bytes = UltraBend.Common.Data.ByteSerialization.GetBytes(testArray);

            var resultArray = UltraBend.Common.Data.ByteSerialization.GetDoubles(bytes);

            CollectionAssert.AreEqual(testArray, resultArray);
        }

        [Test]
        public void SerializeAndDeserializeEmptyDoubleArray()
        {
            var testArray = new double[] { };

            var bytes = UltraBend.Common.Data.ByteSerialization.GetBytes(testArray);

            var resultArray = UltraBend.Common.Data.ByteSerialization.GetDoubles(bytes);

            CollectionAssert.AreEqual(testArray, resultArray);
        }
    }
}
