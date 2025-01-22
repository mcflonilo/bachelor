using System;
using System.Globalization;
using Gu.Units;
using Ultra_Bend.Common;

namespace UltraBend.Common.Units
{
    public class TemperatureDimension : IDimension {
        public double Parse(string value, CultureInfo culture)
        {
            if (value.IndexOf("C", StringComparison.OrdinalIgnoreCase) > 0 && value.IndexOf("°C", StringComparison.OrdinalIgnoreCase) < 0)
                value = value.ToUpper().Replace("C", "°C");
            if (value.IndexOf("F", StringComparison.OrdinalIgnoreCase) > 0 && value.IndexOf("°F", StringComparison.OrdinalIgnoreCase) < 0)
                value = value.ToUpper().Replace("F", "°F");

            return Temperature.Parse(value, culture).SiValue;
        }

        public string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers)
        {
            return Temperature.From(value, TemperatureUnit.Kelvin).ToString(ApplicationState.TemperatureUnit, format);
        }
        public double FromDefault(double value)
        {
            return Temperature.From(value, ApplicationState.TemperatureUnit).SiValue;
        }
    }
}