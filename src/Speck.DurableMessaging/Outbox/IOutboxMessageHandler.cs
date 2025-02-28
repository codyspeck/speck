namespace Speck.DurableMessaging.Outbox;

/// <summary>
/// Handles Outbox messages.
/// </summary>
/// <typeparam name="TMessage">The Outbox message type.</typeparam>
public interface IOutboxMessageHandler<in TMessage>
{
    /// <summary>
    /// Handles an Outbox message.
    /// </summary>
    /// <param name="message">The Outbox message</param>
    /// <returns>A task.</returns>
    Task HandleAsync(TMessage message);
}