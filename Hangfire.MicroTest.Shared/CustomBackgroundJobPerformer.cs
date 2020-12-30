using System;
using System.Linq;
using Hangfire.Server;
using Hangfire.Storage;

namespace Hangfire.MicroTest.Shared
{
    public class CustomBackgroundJobPerformer : IBackgroundJobPerformer
    {
        private readonly IBackgroundJobPerformer _innerPerformer;

        public CustomBackgroundJobPerformer(IBackgroundJobPerformer innerPerformer)
        {
            _innerPerformer = innerPerformer ?? throw new ArgumentNullException(nameof(innerPerformer));
        }

        public object Perform(PerformContext proxyContext)
        {
            if (proxyContext.BackgroundJob.Job.Type == typeof(CustomJob))
            {
                var proxyJob = proxyContext.BackgroundJob.Job.Args.Single(o => o is CustomJob) as CustomJob;

                var invocationData = new InvocationData(
                    proxyJob.Type,
                    proxyJob.Method,
                    proxyJob.ParameterTypes ?? String.Empty,
                    proxyJob.Args);

                var job = invocationData.DeserializeJob();
                var context = new PerformContext(proxyContext.Storage, proxyContext.Connection, new BackgroundJob(proxyContext.BackgroundJob.Id, job, proxyContext.BackgroundJob.CreatedAt), proxyContext.CancellationToken);

                return _innerPerformer.Perform(context);
            }

            return _innerPerformer.Perform(proxyContext);
        }
    }
}