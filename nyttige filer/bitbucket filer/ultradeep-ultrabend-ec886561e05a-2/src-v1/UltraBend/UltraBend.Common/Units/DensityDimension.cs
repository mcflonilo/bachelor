using System.Globalization;
using Gu.Units;

namespace UltraBend.Common.Units
{
    public class DensityDimension : IDimension {
        public double Parse(string value, CultureInfo culture)
        {
            return Density.Parse(value, culture).SiValue;
            //return UnitsNet.Density.Parse(value, culture).KilogramsPerCubicMeter;
        }

        public string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers)
        {
            return Density.From(value, DensityUnit.KilogramsPerCubicMetre).ToString(ApplicationState.DensityUnit, format);
        }
        public double FromDefault(double value)
        {
            return Density.From(value, ApplicationState.DensityUnit).SiValue;
        }
    }
}