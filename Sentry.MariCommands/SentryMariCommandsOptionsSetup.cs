using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Sentry.Extensions.Logging;

namespace Sentry.MariCommands
{
    internal class SentryMariCommandsOptionsSetup : ConfigureFromConfigurationOptions<SentryMariCommandsOptions>
    {
        private readonly IHostEnvironment _hostingEnvironment;

        public SentryMariCommandsOptionsSetup(
            ILoggerProviderConfiguration<SentryMariCommandsLoggerProvider> providerConfiguration,
            IHostEnvironment hostingEnvironment)
            : base(providerConfiguration.Configuration)
            => _hostingEnvironment = hostingEnvironment;

        public override void Configure(SentryMariCommandsOptions options)
        {
            base.Configure(options);

            options.Environment
                = options.Environment
                  ?? EnvironmentLocator.Locate()
                  ?? _hostingEnvironment?.EnvironmentName;

            // TODO: Add log entry filter to Exception Middleware.
        }
    }
}