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
            = new Lazy<IBackgroundJobClient>(() => new CustomBackgroundJobClient(new BackgroundJobClient()), LazyThreadSafetyMode.PublicationOnly);

        private static readonly Func<IBackgroundJobClient> CustomBackgroundJobClientFactory = () => CachedBackgroundJobClient.Value;

        public static IServiceCollection AddMicroserviceHangfireServer(this IServiceCollection services)
        {
            return services.AddMicroserviceHangfireServer(options => { });
        }

        public static IServiceCollection AddMicroserviceHangfireServer(this IServiceCollection services, Action<BackgroundJobServerOptions> config)
        {
            var options = new BackgroundJobServerOptions();
            config(options);

            var filterProvider = options.FilterProvider ?? JobFilterProviders.Providers;
            var activator = options.Activator ?? JobActivator.Current;

            if (services.Any(x => x.ServiceType == typeof(IBackgroundJobPerformer)))
            {
                services.Decorate<IBackgroundJobPerformer, CustomBackgroundJobPerformer>();
            }
            else
            {
                services.AddSingleton<IBackgroundJobPerformer>(x => new CustomBackgroundJobPerformer(new BackgroundJobPerformer(filterProvider, activator, options.TaskScheduler)));
            }
           
            services.TryAddSingleton<IBackgroundJobFactory>(x => new BackgroundJobFactory(filterProvider));
            services.TryAddSingleton<IBackgroundJobStateChanger>(x => new BackgroundJobStateChanger(filterProvider));

            services.AddSingleton(provider => CachedBackgroundJobClient.Value);

            var clientFactoryProperty = typeof(BackgroundJob).GetProperty("ClientFactory", BindingFlags.Static | BindingFlags.NonPublic);
            if (clientFactoryProperty == null)
            {
                throw new InvalidOperationException($"Non-public static property 'ClientFactory' is not found in {nameof(BackgroundJob)} type.");
            }
            clientFactoryProperty.SetValue(null, CustomBackgroundJobClientFactory);

            services.AddHangfireServer(config);

            return services;
        }
    }
}