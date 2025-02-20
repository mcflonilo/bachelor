using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Common
{
    /// <summary>
    /// Uses ModelIdTypeConverterSettings to reflect a data source, while caching the get method in memory
    /// </summary>
    public class ModelIdTypeConverter<T> : TypeConverter where T: IModelId
    {
        public override Boolean GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
        public override Boolean GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }


        private Dictionary<Guid, string> GetModels(ITypeDescriptorContext context, Type modelType)
        {
            if (!ApplicationState.DefaultTypeSources.ContainsKey(modelType))
                throw new Exception($"Type {modelType} does not have a default type source");

            // get with a 1 second cache
            var models = ExpiringCache<Dictionary<Guid, string>>.Cached(MemoryCache.Default,
                $"UltraBend.Common.ModelIdTypeConverter.GetModels({typeof(T).FullName}",
                TimeSpan.FromSeconds(1),
                () => ApplicationState.DefaultTypeSources[modelType]
                    .Invoke(ApplicationState.CurrentProjectTemporaryFile)?.ToDictionary(m => m.Id, m => m.Name), false);
            //var models = ApplicationState.DefaultTypeSources[modelType].Invoke(ApplicationState.CurrentProjectTemporaryFile)?.ToDictionary(m => m.Id, m => m.Name);

            var result = new Dictionary<Guid, string>();
            var nameCounts = new Dictionary<string, int>();
            foreach (var model in models)
            {
                if (nameCounts.ContainsKey(model.Value))
                    nameCounts[model.Value] += 1;
                else
                    nameCounts[model.Value] = 1;

                if (nameCounts[model.Value] > 1)
                {
                    result[model.Key] = model.Value + $" ({nameCounts[model.Value]})";
                }
                else
                {
                    result[model.Key] = model.Value;
                }
            }

            return result;
        }


        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetModels(context, typeof(T)).Values);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (ApplicationState.DefaultTypeSources.ContainsKey(typeof(T)))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return GetModels(context, typeof(T)).Where(m => m.Value == value.ToString()).Select(m => m.Key).FirstOrDefault();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is Guid)
                {
                    return GetModels(context, typeof(T)).Where(m => m.Key == (Guid) value).Select(m => m.Value).FirstOrDefault();
                }

                if (value != null && Nullable.GetUnderlyingType(value.GetType()) == typeof(Guid))
                {
                    return GetModels(context, typeof(T)).Where(m => m.Key == (Guid?) value).Select(m => m.Value).FirstOrDefault();
                }
                
                if (value is string)
                {
                    return GetModels(context, typeof(T)).Where(m => m.Value == (string) value).Select(m => m.Value).FirstOrDefault();
                }
            }

            if (destinationType == typeof(Guid?) || destinationType == typeof(Guid))
            {
                if (value is string)
                {
                    return GetModels(context, typeof(T)).Where(m => m.Value == (string) value).Select(m => m.Key).FirstOrDefault();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

    }
}
