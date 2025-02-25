namespace Speck.DurableMessaging.Inbox;

/// <summary>
/// Handles inbox messages.
/// </summary>
/// <typeparam name="TMessage">The inbox message type.</typeparam>
public interface IInboxMessageHandler<in TMessage>
{
    /// <summary>
    /// Handles an inbox message.
    /// </summary>
    /// <param name="message">The inbox message</param>
    /// <returns>A task.</returns>
    Task HandleAsync(TMessage message);
}