using System.Globalization;
using Gu.Units;
using Ultra_Bend.Common;

namespace UltraBend.Common.Units
{
    public class ForceDimension : IDimension
    {
        public double Parse(string value, CultureInfo culture)
        {
            return Force.Parse(value, culture).SiValue;
        }

        public string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers)
        {
            return Force.From(value, ForceUnit.Newtons).ToString(ApplicationState.ForceUnit, format);
        }
        public double FromDefault(double value)
        {
            return Force.From(value, ApplicationState.ForceUnit).SiValue;
        }
    }
}