using System;
using System.ComponentModel;
using System.Linq;
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
            if (microserviceJob == null) throw new ArgumentNullException(nameof(microserviceJob));

            var invocationData = new InvocationData(
                microserviceJob.Type,
                microserviceJob.Method,
                microserviceJob.ParameterTypes ?? String.Empty,
                microserviceJob.Args);

            var job = invocationData.DeserializeJob();

            if (!job.Method.IsStatic)
            {
                using (var scope = JobActivator.Current.BeginScope(performContext))
                {
                    var obj = scope.Resolve(job.Type);
                    job.Method.Invoke(obj, job.Args.ToArray());
                }

                return;
            }

            job.Method.Invoke(null, job.Args.ToArray());
        }
    }
}