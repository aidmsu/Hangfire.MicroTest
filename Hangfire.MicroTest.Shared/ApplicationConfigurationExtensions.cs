using System;
using Hangfire.Common;

namespace Hangfire.MicroTest.Shared
{
    public static class ApplicationConfigurationExtensions
    {
        public static IGlobalConfiguration UseApplicationConfiguration(this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            JobFilterProviders.Providers.Add(new CustomJobFilterProvider());

            return configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseRecommendedSerializerSettings()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseIgnoredAssemblyVersionTypeResolver()
                .UseResultsInContinuations()
                .UseRedisStorage();
        }
    }
}