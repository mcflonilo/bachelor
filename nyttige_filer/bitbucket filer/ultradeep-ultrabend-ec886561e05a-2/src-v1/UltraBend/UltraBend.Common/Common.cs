using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UltraBend.Common
{
    public static class Common
    {
        public static string ToJson(this object obj, bool prettyPrint = true)
        {
            return JsonConvert.SerializeObject(
                obj, prettyPrint == false ? Formatting.None : Formatting.Indented, new StringEnumConverter());
        }

        public static double RoundToSignificantDigits(this double d, int digits)
        {
            if (System.Math.Abs(d) < 2 * double.Epsilon)
                return 0;

            var scale = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(System.Math.Abs(d))) + 1);
            return scale * System.Math.Round(d / scale, digits);
        }

        public static string EllipsisString(this string value, int maxLength = 30, char delimiter = '\\',
            string ellipsis = "…")
        {
            if (value == null || value.Length <= maxLength)
                return value;

            if (value.Length <= ellipsis.Length)
                return value.Substring(0, System.Math.Min(value.Length, maxLength));

            var lastIndexOf = value.LastIndexOf(delimiter.ToString(), maxLength, StringComparison.Ordinal);

            if (lastIndexOf <= 0)
                return value.Substring(0, maxLength - ellipsis.Length) + ellipsis;

            return value.Substring(0, lastIndexOf) + ellipsis;
        }

        public static bool TryParseDouble(string input, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;

            var numberBuilder = new StringBuilder();
            foreach (var c in input)
                if ("0123456789.E".IndexOf(c) > -1)
                    numberBuilder.Append(c);
                else return false;

            if (double.TryParse(numberBuilder.ToString(),
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.Float |
                NumberStyles.Integer | NumberStyles.Number, CultureInfo.InvariantCulture, out value)) return true;

            numberBuilder.Clear();
            foreach (var c in input)
                if ("0123456789.".IndexOf(c) > -1)
                    numberBuilder.Append(c);
                else break;

            return double.TryParse(numberBuilder.ToString(),
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.Float |
                NumberStyles.Integer | NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        public static IEnumerable<double> GetRange(int count, double min, double max)
        {
            var data = new List<double>(count);
            for (var j = 0; j < count; j++) data.Add(min + j * (max - min) / (count - 1));

            return data;
        }
    }
}