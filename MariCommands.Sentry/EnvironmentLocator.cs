using System;

namespace MariCommands.Sentry
{
    internal static class EnvironmentLocator
    {
        internal static string Locate() => Environment.GetEnvironmentVariable(Constants.EnvironmentEnvironmentVariable);
    }
}