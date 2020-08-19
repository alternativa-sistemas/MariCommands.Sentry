using System.Diagnostics;
using MariCommands;
using Sentry.Protocol;

namespace MariCommands.Sentry
{
    internal class DefaultUserFactory : ICommandUserFactory
    {
        public User Create(CommandContext context)
        {
            Debug.Assert(context != null);

            // Don't has a default implementation in MariCommands that
            // can retrieve a user if exists.            
            return null;
        }
    }
}