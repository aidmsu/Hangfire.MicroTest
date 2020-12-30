using System;
using System.Linq;
using Hangfire.Server;
using Hangfire.Storage;

namespace Hangfire.MicroTest.Shared
{
    public class MicroserviceBackgroundJobPerformer : IBackgroundJobPerformer
    {
        private readonly IBackgroundJobPerformer _innerPerformer;

        public MicroserviceBackgroundJobPerformer(IBackgroundJobPerformer innerPerformer)
        {
            _innerPerformer = innerPerformer ?? throw new ArgumentNullException(nameof(innerPerformer));
        }

        public object Perform(PerformContext performContext)
        {
            if (performContext.BackgroundJob.Job.Type == typeof(MicroserviceJob))
            {
                var proxyJob = performContext.BackgroundJob.Job.Args.Single(o => o is MicroserviceJob) as MicroserviceJob;

                var invocationData = new InvocationData(
                    proxyJob.Type,
                    proxyJob.Method,
                    proxyJob.ParameterTypes ?? String.Empty,
                    proxyJob.Args);

                var job = invocationData.DeserializeJob();
                var context = new PerformContext(performContext.Storage, performContext.Connection, new BackgroundJob(performContext.BackgroundJob.Id, job, performContext.BackgroundJob.CreatedAt), performContext.CancellationToken);

                return _innerPerformer.Perform(context);
            }

            return _innerPerformer.Perform(performContext);
        }
    }
}