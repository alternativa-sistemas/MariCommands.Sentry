using MariCommands;
using Sentry.Protocol;

namespace Sentry.MariCommands
{
    /// <summary>
    /// Sentry User Factory
    /// </summary>
    public interface ICommandUserFactory
    {
        /// <summary>
        /// Creates a <see cref="User"/> from the <see cref="CommandContext"/>
        /// </summary>
        /// <param name="context">The CommandContext where the user resides</param>
        /// <returns>The protocol user</returns>
        User Create(CommandContext context);
    }
}