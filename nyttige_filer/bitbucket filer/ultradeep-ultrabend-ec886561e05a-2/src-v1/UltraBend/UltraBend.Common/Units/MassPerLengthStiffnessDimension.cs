using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Gu.Units;

namespace UltraBend.Common.Units
{
    public class MassPerLengthStiffnessDimension : IDimension
    {
        public double Parse(string value, CultureInfo culture)
        {
            var match = Regex.Match(value, @"([-+]?[0-9]*\.?[0-9]+)");
            if (match.Success)
                return Convert.ToDouble(match.Groups[1].Value, culture);
            return double.NaN;
        }

        public string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers)
        {
            return value.ToString(CultureInfo.InvariantCulture) + " kg m^-1";
            
            //return temp.ToString();

            //return Stiffness.From(value, ApplicationState.StiffnessUnit).ToString(ApplicationState.StiffnessUnit, format);
        }
        public double FromDefault(double value)
        {
            return value;
        }
    }
}