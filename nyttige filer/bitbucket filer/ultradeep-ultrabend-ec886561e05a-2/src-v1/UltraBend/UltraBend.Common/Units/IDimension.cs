using System.Globalization;
using Gu.Units;

namespace UltraBend.Common.Units
{
    public interface IDimension
    {
        double Parse(string value, CultureInfo culture);
        string Format(double value, SymbolFormat format = SymbolFormat.SignedHatPowers);
        double FromDefault(double value);
    }
}