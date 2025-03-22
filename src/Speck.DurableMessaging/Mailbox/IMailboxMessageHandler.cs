namespace Speck.DurableMessaging.Mailbox;

/// <summary>
/// Handles mailbox messages.
/// </summary>
/// <typeparam name="TMessage">The mailbox message type.</typeparam>
public interface IMailboxMessageHandler<in TMessage>
{
    /// <summary>
    /// Handles an mailbox message.
    /// </summary>
    /// <param name="message">The mailbox message</param>
    /// <returns>A task.</returns>
    Task HandleAsync(TMessage message);
}