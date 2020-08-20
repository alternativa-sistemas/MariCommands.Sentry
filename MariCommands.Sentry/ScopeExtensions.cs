using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Sentry;

namespace MariCommands.Sentry
{
    /// <summary>
    /// Scope Extensions
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ScopeExtensions
    {
        /// <summary>
        /// Populates the scope with the HTTP data
        /// </summary>
        /// <remarks>
        /// NOTE: The scope is applied to the event BEFORE running the event processors/exception processors.
        /// The main Sentry SDK has processors which run right before any additional processors to the Event
        /// </remarks>
        public static void Populate(this Scope scope, CommandContext context, SentryMariCommandsOptions options)
        {
            // With the logger integration, a BeginScope call is made with RequestId. That ends up adding
            // two tags with the same value: RequestId and TraceIdentifier, from SentryAspNetCore
            if (!scope.Tags.TryGetValue("RequestId", out var requestId) || requestId != context.TraceIdentifier)
            {
                scope.SetTag(nameof(context.TraceIdentifier), context.TraceIdentifier);
            }

            if (options?.SendDefaultPii == true && !scope.HasUser())
            {
                var userFactory = context.CommandServices?.GetService<ICommandUserFactory>();
                if (userFactory != null)
                {
                    scope.User = userFactory.Create(context);
                }
            }

            SetEnv(scope, context, options);

            if (context.Command != null)
                scope.SetTag("command", context.Command.Name);

            if (context.Result != null && !string.IsNullOrWhiteSpace(context.Result.Reason))
                scope.SetTag("result", context.Result.Reason);

            if (!string.IsNullOrWhiteSpace(context.RawArgs))
                scope.SetTag("rawargs", context.RawArgs);
        }


        private static void SetEnv(Scope scope, CommandContext context, SentryMariCommandsOptions options)
        {
            scope.Request.Env["SERVER_NAME"] = Environment.MachineName;
        }
    }
}