using System;
using System.Linq;
using Sentry;
using Sentry.Extensions.Logging;

namespace MariCommands.Sentry
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
        /// Gets or sets a value indicating whether [include System.Diagnostic.Activity data] to events.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include activity data]; otherwise, <c>false</c>.
        /// </value>
        /// <see cref="System.Diagnostics.Activity"/>
        /// <seealso href="https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md"/>
        public bool IncludeActivityData { get; set; }

        /// <summary>
        /// Add a callback to configure the scope upon SDK initialization
        /// </summary>
        /// <param name="action">The function to invoke when initializing the SDK</param>
        public new void ConfigureScope(Action<Scope> action) => ConfigureScopeCallbacks = ConfigureScopeCallbacks.Concat(new[] { action }).ToArray();

        /// <summary>
        /// List of callbacks to be invoked when initializing the SDK
        /// </summary>
        internal Action<Scope>[] ConfigureScopeCallbacks { get; set; } = Array.Empty<Action<Scope>>();

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