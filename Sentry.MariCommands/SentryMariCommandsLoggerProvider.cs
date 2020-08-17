using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry.Extensions.Logging;

namespace Sentry.MariCommands
{
    [ProviderAlias("Sentry")]
    internal class SentryMariCommandsLoggerProvider : SentryLoggerProvider
    {
        public SentryMariCommandsLoggerProvider(IOptions<SentryMariCommandsOptions> options, IHub hub)
            : base(options, hub)
        {
        }
    }
}