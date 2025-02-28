namespace Speck.DurableMessaging.Outbox;

/// <summary>
/// Handles Outbox messages.
/// </summary>
/// <typeparam name="TMessage">The Outbox message type.</typeparam>
public interface IOutboxMessageBatchHandler<in TMessage>
{
    /// <summary>
    /// Handles a batch of Outbox messages.
    /// </summary>
    /// <param name="messages">The Outbox messages.</param>
    /// <returns>A task.</returns>
    Task HandleAsync(IReadOnlyCollection<TMessage> messages);
}