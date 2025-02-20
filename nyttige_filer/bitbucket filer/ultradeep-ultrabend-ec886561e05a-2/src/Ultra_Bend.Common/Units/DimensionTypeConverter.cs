using System;
using System.ComponentModel;
using System.Globalization;


namespace UltraBend.Common.Units
{
    public class DimensionTypeConverter<T> : TypeConverter where T:IDimension, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(double) || sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || destinationType == typeof(double);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var converter = new T();

            if (value.GetType() == typeof(double))
            {
                try
                {
                    return converter.Parse(value.ToString(), culture);
                }
                catch (FormatException)
                {
                    Common.TryParseDouble(value.ToString(), out var parsedDouble);
                    return parsedDouble;
                }
            }

            if (value is string)
            {
                try
                {
                    return converter.Parse(value.ToString(), culture);
                }
                catch (FormatException)
                {
                    Common.TryParseDouble(value.ToString(), out var parsedDouble);
                    return converter.FromDefault(parsedDouble);
                }

                //return converter.Format(Convert.ToDouble(value));
            }


            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var converter = new T();
            if (destinationType == typeof(double) && value is double)
            {
                return value;
            }

            if (destinationType == typeof(string) && value is double)
            {
                return converter.Format((double)value);
            }

            if (destinationType == typeof(double) && value is string)
            {
                try
                {
                    return converter.Parse(value.ToString(), culture);
                }
                catch (FormatException)
                {
                    Common.TryParseDouble(value.ToString(), out var parsedDouble);
                    return converter.FromDefault(parsedDouble);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
