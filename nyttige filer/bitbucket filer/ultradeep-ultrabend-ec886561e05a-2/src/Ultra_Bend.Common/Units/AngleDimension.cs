using System.Globalization;
using Gu.Units;
using Ultra_Bend.Common;

namespace UltraBend.Common.Units
{
    public class AngleDimension : IDimension
    {
        public double Parse(string value, CultureInfo culture)
        {
            return Angle.Parse(value, culture).SiValue;
        }

        public string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers)
        {
            return Angle.From(value, AngleUnit.Radians).ToString(ApplicationState.AngleUnit, format);
        }

        public double FromDefault(double value)
        {
            return Angle.From(value, ApplicationState.AngleUnit).SiValue;
        }
    }
}