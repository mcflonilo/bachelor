using Gu.Units;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UltraBend.Common.Units;

namespace Ultra_Bend.Common.Units
{
    public class CurvatureDimension : IDimension
    {
        public string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers)
        {
            return value.ToString(CultureInfo.InvariantCulture) + " m^-1";
        }

        public double FromDefault(double value)
        {
            return value;
        }

        public double Parse(string value, CultureInfo culture)
        {
            var match = Regex.Match(value, @"([-+]?[0-9]*\.?[0-9]+)");
            if (match.Success)
                return Convert.ToDouble(match.Groups[1].Value, culture);
            return double.NaN;
        }
    }
}
