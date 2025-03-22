namespace Speck.DurableMessaging.Mailbox;

/// <summary>
/// Handles mailbox messages.
/// </summary>
/// <typeparam name="TMessage">The mailbox message type.</typeparam>
public interface IMailboxMessageBatchHandler<in TMessage>
{
    /// <summary>
    /// Handles a batch of mailbox messages.
    /// </summary>
    /// <param name="messages">The mailbox messages.</param>
    /// <returns>A task.</returns>
    Task HandleAsync(IReadOnlyCollection<TMessage> messages);
}