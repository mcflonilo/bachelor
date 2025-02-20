using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Dependencies;
using PostSharp.Patterns.Model;
using UltraBend.Common;
using UltraBend.Services.BSEngine.Output;

namespace UltraBend.Services.DomainObjects
{
    [Serializable]
    [NotifyPropertyChanged]
    public class Result : IModelId, ICacheDependency
    {
        [ReadOnly(true)]
        [Description("Unique identifier for the design")]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string JsonOfResult { get; set; }

        private BendStiffenerOutput output { get; set; }

        private bool hasOutput { get; set; }

        
        private BendStiffenerOutput GetOutputResultFromJson(string json)
        {
            if (!hasOutput)
            {
                output = Deserialize(json);
                hasOutput = true;
            }

            return output;
        }

        private void SetJsonResult(BendStiffenerOutput result)
        {
            this.JsonOfResult = JsonConvert.SerializeObject(result);
            hasOutput = false;
            GetOutputResultFromJson(JsonOfResult);
        }

        [Pure]
        private BendStiffenerOutput Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<BendStiffenerOutput>(json);
        }

        public BendStiffenerOutput Output
        {
            get => GetOutputResultFromJson(JsonOfResult);
            set => SetJsonResult(value);
        }

        public string GetCacheKey()
        {
            return this.Id.ToString();
        }
    }
}
