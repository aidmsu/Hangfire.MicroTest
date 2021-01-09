using System.Collections.Generic;
using Hangfire.Logging;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;

namespace Hangfire.MicroTest.Shared
{
    public class MicroserviceStorageDecorator : JobStorage
    {
        private readonly JobStorage _coreStorage;

        public MicroserviceStorageDecorator(JobStorage coreStorage)
        {
            _coreStorage = coreStorage;
        }

        public override IMonitoringApi GetMonitoringApi()
        {
            return _coreStorage.GetMonitoringApi();
        }

        public override IStorageConnection GetConnection()
        {
            return new MicroserviceStorageConnectionDecorator(_coreStorage.GetConnection() as JobStorageConnection);
        }

#pragma warning disable 618
        public override IEnumerable<IServerComponent> GetComponents()
#pragma warning restore 618
        {
            return _coreStorage.GetComponents();
        }

        public override IEnumerable<IStateHandler> GetStateHandlers()
        {
            return _coreStorage.GetStateHandlers();
        }

        public override void WriteOptionsToLog(ILog logger)
        {
            _coreStorage.WriteOptionsToLog(logger);
        }

        public override bool LinearizableReads => _coreStorage.LinearizableReads;

    }
}