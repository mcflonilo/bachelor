using System.Globalization;
using Gu.Units;

namespace UltraBend.Common.Units
{
    public class LengthDimension : IDimension
    {
        public double Parse(string value, CultureInfo culture)
        {
            return Length.Parse(value, culture).SiValue;
        }

        public string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers)
        {
            return Length.From(value, LengthUnit.Metres).ToString(ApplicationState.LengthUnit, format);
        }
        public double FromDefault(double value)
        {
            return Length.From(value, ApplicationState.LengthUnit).SiValue;
        }
    }
}