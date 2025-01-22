using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UltraBend.Common.Test
{
    [TestFixture]
    public class CommonTests
    {
        [Test]
        public void SerializesToJsonAsExpected()
        {
            Assert.AreEqual("{\"Test\":\"Test Value\"}", new { Test = "Test Value" }.ToJson(false));
        }

        [Test]
        public void SerializesToPrettyJsonAsExpected()
        {
            Assert.AreEqual("{\r\n  \"Test\": \"Test Value\"\r\n}", new { Test = "Test Value" }.ToJson(true));
        }

        [Test]
        public void RoundToSignificantDigits_HandlesEpsilonAsZero()
        {
            Assert.AreEqual(0, UltraBend.Common.Common.RoundToSignificantDigits(double.Epsilon, 1));
        }

        [Test]
        public void RoundToSignificantDigits_RoundsUpToThreeDigits()
        {
            Assert.AreEqual(0.00168, Common.RoundToSignificantDigits(0.001677777, 3));
        }

        [Test]
        public void EllipsisString_CutsWithEllipsis()
        {
            const string source = "Hello world, how are you?";
            const string expected = "Hello…";
            var result = Common.EllipsisString(source, 10, ' ');
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EllipsisString_CutsWithCustomEllipsis()
        {
            const string source = "Hello world, how are you?";
            const string expected = "Hello...";
            var result = Common.EllipsisString(source, 10, ' ', "...");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EllipsisString_HandlesNull()
        {
            const string source = null;
            const string expected = null;
            var result = Common.EllipsisString(source, 10, ' ', "...");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EllipsisString_HandlesEmpty()
        {
            const string source = "";
            const string expected = "";
            var result = Common.EllipsisString(source, 10, ' ', "...");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EllipsisString_HandlesShort()
        {
            const string source = "Hello there";
            const string expected = "Hello there";
            var result = Common.EllipsisString(source, source.Length, ' ', "...");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EllipsisString_ShorterThanEllipsis()
        {
            const string source = "Aa";
            const string expected = "A";
            var result = Common.EllipsisString(source, 1, ' ', "...");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EllipsisString_HandlesNoDelim()
        {
            const string source = "Hello there, how are you?";
            string expected = source.Substring(0, 7) + "...";
            var result = Common.EllipsisString(source, 10, 'B', "...");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetRange_ReturnsExpectedRange()
        {
            var expectedResult = new[] { 1.0, 1.5, 2.0 };
            var result = Common.GetRange(3, 1.0, 2.0);
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [Test]
        public void GetRange_ReturnsExpectedRange2()
        {
            var expectedResult = new[] { 1.0, 2.0 };
            var result = Common.GetRange(2, 1.0, 2.0);
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [Test]
        public void GetRange_ReturnsExpectedRange3()
        {
            var expectedResult = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var result = Common.GetRange(5, 1.0, 5.0);
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [Test]
        public void TryParseDouble_FailsToParse()
        {
            var expectedResult = false;
            double outValue = 3.14;
            var result = Common.TryParseDouble("asdf", out outValue);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(0.0, outValue);
        }

        [Test]
        public void TryParseDouble_FailsWithZeroOnNull()
        {
            var expectedResult = false;
            double outValue = 3.14;
            var result = Common.TryParseDouble(null, out outValue);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(0.0, outValue);
        }

        [Test]
        public void TryParseDouble_FailsWithZeroOnEmpty()
        {
            var expectedResult = false;
            double outValue = 3.14;
            var result = Common.TryParseDouble("", out outValue);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(0.0, outValue);
        }

        [Test]
        public void TryParseDouble_FailsWithInvalidCharacter()
        {
            var expectedResult = false;
            double outValue = 3.14;
            var result = Common.TryParseDouble("3.1v4", out outValue);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(0.0, outValue);
        }

        [Test]
        public void TryParseDouble_FallsBackWhenENotationFails()
        {
            var expectedResult = true;
            double outValue = 3.14;
            var result = Common.TryParseDouble("3.1EE4", out outValue);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(outValue, outValue);
        }

        [Test]

        public void TryParseDouble_ENotation()
        {
            var expectedResult = true;
            double outValue = 31000.0d;
            var result = Common.TryParseDouble("3.1E4", out outValue);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(outValue, outValue);
        }
    }
}
