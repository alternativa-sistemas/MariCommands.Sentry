using System.Collections.Generic;
using System.Linq;
using MariCommands.Builder;
using MariCommands.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry.Extensibility;
using Sentry.Extensions.Logging;
using Sentry.Infrastructure;

namespace Sentry.MariCommands
{
    /// <summary>
    /// Extension methods for <see cref="ICommandApplicationBuilder"/>
    /// </summary>
    public static class CommandApplicationBuilderExtensions
    {
        /// <summary>
        /// Use Sentry integration
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="registerSdkClose">Set it to false if this app is a Web ASP.NET Core app with the Sentry integration.</param>
        /// <returns></returns>
        public static ICommandApplicationBuilder UseSentry(this ICommandApplicationBuilder app, bool registerSdkClose = true)
        {
            // Container is built so resolve a logger and modify the SDK internal logger
            var options = app.ApplicationServices.GetService<IOptions<SentryMariCommandsOptions>>();
            if (options?.Value is SentryMariCommandsOptions o)
            {
                if (o.Debug && (o.DiagnosticLogger == null || o.DiagnosticLogger.GetType() == typeof(ConsoleDiagnosticLogger)))
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<ISentryClient>>();
                    o.DiagnosticLogger = new MelDiagnosticLogger(logger, o.DiagnosticsLevel);
                }

                var stackTraceFactory = app.ApplicationServices.GetService<ISentryStackTraceFactory>();
                if (stackTraceFactory != null)
                {
                    o.UseStackTraceFactory(stackTraceFactory);
                }

                if (app.ApplicationServices.GetService<IEnumerable<ISentryEventProcessor>>().Any())
                {
                    o.AddEventProcessorProvider(app.ApplicationServices.GetServices<ISentryEventProcessor>);
                }

                if (app.ApplicationServices.GetService<IEnumerable<ISentryEventExceptionProcessor>>().Any())
                {
                    o.AddExceptionProcessorProvider(app.ApplicationServices.GetServices<ISentryEventExceptionProcessor>);
                }
            }

            if (registerSdkClose)
            {
                var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
                lifetime?.ApplicationStopped.Register(SentrySdk.Close);
            }

            return app.UseMiddleware<SentryMiddleware>();
        }
    }
}