using System;
using System.ComponentModel;
using Hangfire.Annotations;
using Hangfire.Common;
using Hangfire.Server;
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
        public static void Execute(string displayName, MicroserviceJob microserviceJob, PerformContext performContext)
        {
            var originalJob = microserviceJob.GetOriginalJob();
            performContext.BackgroundJob = new BackgroundJob(performContext.BackgroundJob.Id, originalJob, performContext.BackgroundJob.CreatedAt);

            if (performContext.Performer == null)
            {
                throw new InvalidOperationException("The Performer property is not set for performContext.");
            }

            performContext.Performer.Perform(performContext);
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