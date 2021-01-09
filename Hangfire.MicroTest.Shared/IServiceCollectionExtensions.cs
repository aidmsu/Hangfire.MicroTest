using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hangfire.MicroTest.Shared
{
    public static class IServiceCollectionExtensions
    {
        private static readonly Lazy<IBackgroundJobClient> CachedBackgroundJobClient
            = new Lazy<IBackgroundJobClient>(() => new MicroserviceBackgroundJobClient(new BackgroundJobClient()), LazyThreadSafetyMode.PublicationOnly);

        private static readonly Func<IBackgroundJobClient> MicroserviceBackgroundJobClientFactory = () => CachedBackgroundJobClient.Value;

        public static IServiceCollection AddMicroservices(this IServiceCollection services)
        {
            services.AddSingleton(provider => CachedBackgroundJobClient.Value);

            var clientFactoryProperty = typeof(BackgroundJob).GetProperty("ClientFactory", BindingFlags.Static | BindingFlags.NonPublic);
            if (clientFactoryProperty == null)
            {
                throw new InvalidOperationException($"Non-public static property 'ClientFactory' is not found in {nameof(BackgroundJob)} type.");
            }
            clientFactoryProperty.SetValue(null, MicroserviceBackgroundJobClientFactory);

            return services;
        }
    }
}