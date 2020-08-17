using System;

namespace Sentry.MariCommands
{
    internal static class EnvironmentLocator
    {
        internal static string Locate() => Environment.GetEnvironmentVariable(Constants.EnvironmentEnvironmentVariable);
    }
}