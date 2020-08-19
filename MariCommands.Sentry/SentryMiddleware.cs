using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using MariCommands;
using MariCommands.Middlewares;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry;
using Sentry.Protocol;
using Sentry.Reflection;

namespace MariCommands.Sentry
{
    internal class SentryMiddleware : ICommandMiddleware
    {
        private readonly Func<IHub> _hubAccessor;
        private readonly SentryMariCommandsOptions _options;
        private readonly ILogger<SentryMiddleware> _logger;

        internal static readonly SdkVersion NameAndVersion
            = typeof(SentryMiddleware).Assembly.GetNameAndVersion();

        private static readonly string ProtocolPackageName = "nuget:" + NameAndVersion.Name;

        public SentryMiddleware(
            Func<IHub> hubAccessor,
            IOptions<SentryMariCommandsOptions> options,
            ILogger<SentryMiddleware> logger)
        {
            _hubAccessor = hubAccessor ?? throw new ArgumentNullException(nameof(hubAccessor));
            _options = options?.Value;

            if (_options != null)
            {
                var hub = _hubAccessor();

                foreach (var callback in _options.ConfigureScopeCallbacks)
                {
                    hub.ConfigureScope(callback);
                }
            }

            _logger = logger;
        }

        public async Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var hub = _hubAccessor();
            if (!hub.IsEnabled)
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            using (hub.PushAndLockScope())
            {
                try
                {
                    await next(context);

                    // TODO: Exception Feature.
                    // // When an exception was handled by other component (i.e: UseExceptionHandler feature).
                    // var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    // if (exceptionFeature?.Error != null)
                    // {
                    //     CaptureException(exceptionFeature.Error);
                    // }
                }
                catch (Exception e)
                {
                    CaptureException(hub, context, e);

                    ExceptionDispatchInfo.Capture(e).Throw();
                }

                if (_options != null)
                {
                    if (_options.FlushOnCompletedRequest)
                    {
                        // Serverless environments flush the queue at the end of each request, from SentryAspNetCore.
                        // This can be used if your app uses MariCommands to process a message.
                        await hub.FlushAsync(_options.FlushTimeout);
                    }
                }
            }
        }

        private void CaptureException(IHub hub, CommandContext context, Exception e)
        {
            hub.ConfigureScope(scope =>
            {
                PopulateScope(context, scope);
            });

            var evt = new SentryEvent(e);

            _logger?.LogTrace("Sending event '{SentryEvent}' to Sentry.", evt);

            var id = hub.CaptureEvent(evt);

            _logger?.LogInformation("Event '{id}' queued.", id);
        }

        internal void PopulateScope(CommandContext context, Scope scope)
        {
            scope.Sdk.Name = Constants.SdkName;
            scope.Sdk.Version = NameAndVersion.Version;
            scope.Sdk.AddPackage(ProtocolPackageName, NameAndVersion.Version);

            // TODO: Set CommandContext#TraceIdentifier
            //scope.Populate(context, _options);

            if (_options?.IncludeActivityData == true && Activity.Current != null)
            {
                scope.SetTags(Activity.Current.Tags);
            }
        }
    }
}