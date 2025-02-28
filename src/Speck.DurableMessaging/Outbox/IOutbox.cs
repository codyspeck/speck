namespace Speck.DurableMessaging.Outbox;

/// <summary>
/// Exposes methods for interfacing with an Outbox messages database table.
/// </summary>
public interface IOutbox
{
    /// <summary>
    /// Inserts a message into the Outbox. 
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A task.</returns>
    Task InsertAsync(object message);
}
