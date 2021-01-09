using System;
using System.ComponentModel;
using Hangfire.Annotations;
using Hangfire.Common;
using Hangfire.Storage;
using Newtonsoft.Json;

namespace Hangfire.MicroTest.Shared
{
    internal class MicroserviceJob
    {
        public MicroserviceJob(
            [NotNull] string type,
            [NotNull] string method,
            [CanBeNull] string parameterTypes,
            [CanBeNull] string args,
            [CanBeNull] JobFilterAttribute[] typeFilters,
            [CanBeNull] JobFilterAttribute[] methodFilters)
        {
            Type = type;
            Method = method;
            ParameterTypes = parameterTypes;
            Args = args;
            TypeFilters = typeFilters;
            MethodFilters = methodFilters;
        }
        
        [JsonProperty("t")]
        public string Type { get; }
        
        [JsonProperty("m")]
        public string Method { get; }
        
        [JsonProperty("p", NullValueHandling = NullValueHandling.Ignore)]
        public string ParameterTypes { get; }
        
        [JsonProperty("a", NullValueHandling = NullValueHandling.Ignore)]
        public string Args { get; }

        [JsonProperty("tf", NullValueHandling = NullValueHandling.Ignore)]
        public JobFilterAttribute[] TypeFilters { get; }
        
        [JsonProperty("mf", NullValueHandling = NullValueHandling.Ignore)]
        public JobFilterAttribute[] MethodFilters { get; }

        [DisplayName("{0}")]
        public static void Execute(string displayName, MicroserviceJob microserviceJob)
        {
            throw new InvalidOperationException("This method should not be invoked.");
        }

        internal bool TryGetOriginalJob(out Job job)
        {
            job = null;
            try
            {
                job = GetOriginalJob();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Job GetOriginalJob()
        {
            var invocationData = new InvocationData(
                Type,
                Method,
                ParameterTypes ?? String.Empty,
                Args);

            return invocationData.DeserializeJob();
        }
    }
}