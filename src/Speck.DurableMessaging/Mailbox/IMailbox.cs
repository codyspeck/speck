namespace Speck.DurableMessaging.Mailbox;

/// <summary>
/// Exposes methods for interfacing with an mailbox messages database table.
/// </summary>
public interface IMailbox
{
    /// <summary>
    /// Inserts a message into the mailbox. 
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A task.</returns>
    Task InsertAsync(object message);
}
