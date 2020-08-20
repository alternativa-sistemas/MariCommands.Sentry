using System;
using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sentry.Extensibility;

namespace MariCommands.Sentry
{
    /// <summary>
    /// Extension methods to <see cref="IHostBuilder"/>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SentryHostBuilderExtensions
    {
        /// <summary>
        /// Uses Sentry integration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="addLoggerProvider">Set it to <c>false</c> if this app is a 
        /// Web ASP.NET Core app with the Sentry integration.</param>
        /// <returns></returns>
        public static IHostBuilder UseSentry(this IHostBuilder builder, bool addLoggerProvider = true)
            => UseSentry(builder, (Action<SentryMariCommandsOptions>)null, addLoggerProvider);

        /// <summary>
        /// Uses Sentry integration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="dsn">The DSN.</param>
        /// <param name="addLoggerProvider">Set it to <c>false</c> if this app is a 
        /// Web ASP.NET Core app with the Sentry integration.</param>
        /// <returns></returns>
        public static IHostBuilder UseSentry(this IHostBuilder builder, string dsn, bool addLoggerProvider = true)
            => builder.UseSentry(o => o.Dsn = dsn, addLoggerProvider);

        /// <summary>
        /// Uses Sentry integration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <param name="addLoggerProvider">Set it to <c>false</c> if this app is a 
        /// Web ASP.NET Core app with the Sentry integration.</param>
        /// <returns></returns>
        public static IHostBuilder UseSentry(
            this IHostBuilder builder,
            Action<SentryMariCommandsOptions> configureOptions, bool addLoggerProvider = true)
            => builder.UseSentry((context, options) => configureOptions?.Invoke(options), addLoggerProvider);

        /// <summary>
        /// Uses Sentry integration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <param name="addLoggerProvider">Set it to <c>false</c> if this app is a 
        /// Web ASP.NET Core app with the Sentry integration.</param>
        /// <returns></returns>
        public static IHostBuilder UseSentry(
            this IHostBuilder builder,
            Action<HostBuilderContext, SentryMariCommandsOptions> configureOptions, bool addLoggerProvider = true)
        {
            // The earliest we can hook the SDK initialization code with the framework
            // Initialization happens at a later time depending if the default MEL backend is enabled or not.
            // In case the logging backend was replaced, init happens later, at the StartupFilter
            builder.ConfigureLogging((context, logging) =>
            {
                logging.AddConfiguration();

                var section = context.Configuration.GetSection("Sentry");
                logging.Services.Configure<SentryMariCommandsOptions>(section);

                if (configureOptions != null)
                {
                    logging.Services.Configure<SentryMariCommandsOptions>(options =>
                    {
                        configureOptions(context, options);
                    });
                }

                logging.Services.AddSingleton<IConfigureOptions<SentryMariCommandsOptions>, SentryMariCommandsOptionsSetup>();

                if (addLoggerProvider)
                {
                    logging.Services.AddSingleton<ILoggerProvider, SentryMariCommandsLoggerProvider>();
                    logging.Services.AddSingleton<ISentryEventProcessor, MariCommandsEventProcessor>();
                }


                logging.AddFilter<SentryMariCommandsLoggerProvider>(
                    "MariCommands.Middlewares.DefaultExceptionMiddleware",
                    LogLevel.None);

                logging.Services.AddSentry();
            });

            return builder;
        }
    }
}