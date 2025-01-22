using System.Globalization;
using Gu.Units;
using Ultra_Bend.Common;

namespace UltraBend.Common.Units
{
    public class PressureDimension : IDimension
    {
        public double Parse(string value, CultureInfo culture)
        {
            return Pressure.Parse(value, culture).SiValue;
        }

        public string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers)
        {
            return Pressure.From(value, PressureUnit.Pascals).ToString(ApplicationState.PressureUnit, format);
        }
        public double FromDefault(double value)
        {
            return Pressure.From(value, ApplicationState.PressureUnit).SiValue;
        }
    }
}