namespace Speck.DurableMessaging.Inbox;

/// <summary>
/// Handles inbox messages.
/// </summary>
/// <typeparam name="TMessage">The inbox message type.</typeparam>
public interface IInboxMessageBatchHandler<in TMessage>
{
    /// <summary>
    /// Handles a batch of inbox messages.
    /// </summary>
    /// <param name="messages">The inbox messages.</param>
    /// <returns>A task.</returns>
    Task HandleAsync(IReadOnlyCollection<TMessage> messages);
}