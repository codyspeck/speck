﻿namespace Speck.DurableMessaging.Inbox;

internal interface IInboxMessageRepository
{
    Task<InboxMessage> GetInboxMessageAsync(Guid inboxMessageId, string inboxMessageTable);
    
    Task<IReadOnlyCollection<InboxMessage>> GetInboxMessagesAsync(string inboxMessageTable, int count);
    
    Task LockInboxMessagesAsync(IEnumerable<InboxMessage> messages, string inboxMessageTable, DateTime lockedUntil);
    
    Task ProcessInboxMessageAsync(InboxMessage message, string inboxMessageTable);
}