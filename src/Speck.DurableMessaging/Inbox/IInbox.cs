﻿namespace Speck.DurableMessaging.Inbox;

/// <summary>
/// Exposes methods for interfacing with an inbox messages database table.
/// </summary>
public interface IInbox
{
    /// <summary>
    /// Inserts a message into the inbox. 
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A task.</returns>
    Task InsertAsync(object message);
}
