using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sentry.Extensibility;
using Sentry.Extensions.Logging.Extensions.DependencyInjection;
using MariCommands.Sentry;

// ReSharper disable once CheckNamespace -- Discoverability
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Sentry's services to the <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddSentry(this IServiceCollection services)
        {
            services.TryAddSingleton<ICommandUserFactory, DefaultUserFactory>();

            services.AddSentry<SentryMariCommandsOptions>();

            return services;
        }
    }
}
