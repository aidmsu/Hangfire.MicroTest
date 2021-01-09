using System.Collections.Generic;
using Hangfire.Common;

namespace Hangfire.MicroTest.Shared
{
    internal class MicroserviceJobFilterProvider : IJobFilterProvider
    {
        public IEnumerable<JobFilter> GetFilters(Job job)
        {
            if (job?.Type != typeof(MicroserviceJob)) yield break;
            
            foreach (var arg in job.Args)
            {
                if (arg is MicroserviceJob microserviceJob)
                {
                    if (microserviceJob.MethodFilters != null)
                    {
                        foreach (var filter in microserviceJob.MethodFilters)
                        {
                            yield return new JobFilter(filter, JobFilterScope.Method, null);
                        }
                    }

                    if (microserviceJob.TypeFilters != null)
                    {
                        foreach (var filter in microserviceJob.TypeFilters)
                        {
                            yield return new JobFilter(filter, JobFilterScope.Type, null);
                        }
                    }
                }
            }
        }
    }
}