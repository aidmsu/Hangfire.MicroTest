using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;

namespace Hangfire.MicroTest.Shared
{
    public class MicroserviceStorageConnectionDecorator : JobStorageConnection
    {
        private readonly JobStorageConnection _storageConnection;

        public MicroserviceStorageConnectionDecorator(JobStorageConnection storageConnection)
        {
            _storageConnection = storageConnection ?? throw new ArgumentNullException(nameof(storageConnection));
        }

        public override void Dispose()
        {
            _storageConnection.Dispose();
        }

        public override IWriteOnlyTransaction CreateWriteTransaction()
        {
            return _storageConnection.CreateWriteTransaction();
        }

        public override IDisposable AcquireDistributedLock(string resource, TimeSpan timeout)
        {
            return _storageConnection.AcquireDistributedLock(resource, timeout);
        }

        public override string CreateExpiredJob(Job job, IDictionary<string, string> parameters, DateTime createdAt, TimeSpan expireIn)
        {
            return _storageConnection.CreateExpiredJob(job, parameters, createdAt, expireIn);
        }

        public override IFetchedJob FetchNextJob(string[] queues, CancellationToken cancellationToken)
        {
            return _storageConnection.FetchNextJob(queues, cancellationToken);
        }

        public override void SetJobParameter(string id, string name, string value)
        {
            _storageConnection.SetJobParameter(id, name, value);
        }

        public override string GetJobParameter(string id, string name)
        {
            return _storageConnection.GetJobParameter(id, name);
        }

        public override JobData GetJobData(string jobId)
        {
            var jobData = _storageConnection.GetJobData(jobId);
            if (jobData?.Job == null) return jobData;

            if (jobData.Job.Type != typeof(MicroserviceJob)) return jobData;

            var proxyJob = jobData.Job.Args.Single(o => o is MicroserviceJob) as MicroserviceJob;

            if (proxyJob.TryGetOriginalJob(out var job))
            {
                jobData.Job = job;
            }

            return jobData;
        }

        public override StateData GetStateData(string jobId)
        {
            return _storageConnection.GetStateData(jobId);
        }

        public override void AnnounceServer(string serverId, ServerContext context)
        {
            _storageConnection.AnnounceServer(serverId, context);
        }

        public override void RemoveServer(string serverId)
        {
            _storageConnection.RemoveServer(serverId);
        }

        public override void Heartbeat(string serverId)
        {
            _storageConnection.Heartbeat(serverId);
        }

        public override int RemoveTimedOutServers(TimeSpan timeOut)
        {
            return _storageConnection.RemoveTimedOutServers(timeOut);
        }

        public override HashSet<string> GetAllItemsFromSet(string key)
        {
            return _storageConnection.GetAllItemsFromSet(key);
        }

        public override string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore)
        {
            return _storageConnection.GetFirstByLowestScoreFromSet(key, fromScore, toScore);
        }

        public override List<string> GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore, int count)
        {
            return _storageConnection.GetFirstByLowestScoreFromSet(key, fromScore, toScore, count);
        }

        public override long GetSetCount(string key)
        {
            return _storageConnection.GetSetCount(key);
        }

        public override List<string> GetRangeFromSet(string key, int startingFrom, int endingAt)
        {
            return _storageConnection.GetRangeFromSet(key, startingFrom, endingAt);
        }

        public override TimeSpan GetSetTtl(string key)
        {
            return _storageConnection.GetSetTtl(key);
        }

        public override void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            _storageConnection.SetRangeInHash(key, keyValuePairs);
        }

        public override Dictionary<string, string> GetAllEntriesFromHash(string key)
        {
            return _storageConnection.GetAllEntriesFromHash(key);
        }

        public override string GetValueFromHash(string key, string name)
        {
            return _storageConnection.GetValueFromHash(key, name);
        }

        public override long GetHashCount(string key)
        {
            return _storageConnection.GetHashCount(key);
        }

        public override TimeSpan GetHashTtl(string key)
        {
            return _storageConnection.GetHashTtl(key);
        }

        public override long GetListCount(string key)
        {
            return _storageConnection.GetListCount(key);
        }

        public override List<string> GetAllItemsFromList(string key)
        {
            return _storageConnection.GetAllItemsFromList(key);
        }

        public override List<string> GetRangeFromList(string key, int startingFrom, int endingAt)
        {
            return _storageConnection.GetRangeFromList(key, startingFrom, endingAt);
        }

        public override TimeSpan GetListTtl(string key)
        {
            return _storageConnection.GetListTtl(key);
        }

        public override long GetCounter(string key)
        {
            return _storageConnection.GetCounter(key);
        }
    }
}