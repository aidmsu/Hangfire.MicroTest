using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace Hangfire.MicroTest.Shared
{
    internal sealed class JobFilterAttributeProvider : JobFilterAttributeFilterProvider
    {
        public IEnumerable<JobFilterAttribute> GetTypeFilters(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            return GetTypeAttributes(job);
        }

        public IEnumerable<JobFilterAttribute> GetMethodFilters(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            return GetMethodAttributes(job);
        }
    }
    
    public class MicroserviceBackgroundJobClient : IBackgroundJobClient
    {
        private readonly IBackgroundJobClient _inner;
        private readonly JobFilterAttributeProvider _attributeProvider;

        public MicroserviceBackgroundJobClient(IBackgroundJobClient inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _attributeProvider = new JobFilterAttributeProvider();
        }
        
        public string Create(Job job, IState state)
        {
            var typeFilters = _attributeProvider.GetTypeFilters(job).ToArray();
            var methodFilters = _attributeProvider.GetMethodFilters(job).ToArray();

            var invocationData = InvocationData.SerializeJob(job);
            var displayName = $"{job.Type.Name}.{job.Method.Name}";

            var proxyJob = Job.FromExpression(() => MicroserviceJob.Execute(displayName, new MicroserviceJob(
                    invocationData.Type,
                    invocationData.Method,
                    invocationData.ParameterTypes != String.Empty ? invocationData.ParameterTypes : null,
                    invocationData.Arguments,
                    typeFilters.Length > 0 ? typeFilters : null,
                    methodFilters.Length > 0 ? methodFilters : null), null));

            return _inner.Create(proxyJob, state);
        }

        public bool ChangeState(string jobId, IState state, string expectedState)
        {
            return _inner.ChangeState(jobId, state, expectedState);
        }
    }
}