using System;
using Sentry.Extensions.Logging;

namespace Sentry.MariCommands
{
    /// <summary>
    /// An options class for the MariCommands Sentry integration
    /// </summary>
    public class SentryMariCommandsOptions : SentryLoggingOptions
    {
        /// <summary>
        /// Flush on completed request
        /// </summary>
        public bool FlushOnCompletedRequest { get; set; }

        /// <summary>
        /// How long to wait for the flush to finish. Defaults to 2 seconds.
        /// </summary>
        public TimeSpan FlushTimeout { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Creates a new instance of <see cref="SentryMariCommandsOptions"/>.
        /// </summary>
        public SentryMariCommandsOptions()
        {
            // Don't report Environment.UserName as the user.
            IsEnvironmentUser = false;
        }
    }
}