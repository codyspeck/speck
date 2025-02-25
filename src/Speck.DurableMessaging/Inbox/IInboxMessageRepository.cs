namespace Speck.DurableMessaging.Inbox;

internal interface IInboxMessageRepository
{
    Task<InboxMessage> GetInboxMessageAsync(Guid inboxMessageId, string inboxMessageTable);
    
    Task<IReadOnlyCollection<InboxMessage>> GetInboxMessagesAsync(string inboxMessageTable, int count);
    
    Task<IReadOnlyCollection<InboxMessage>> GetInboxMessagesAsync(IEnumerable<Guid> inboxMessageIds, string inboxMessageTable);

    Task InsertAsync(InboxMessage inboxMessage, string inboxMessageTable);
    
    Task LockInboxMessagesAsync(IReadOnlyCollection<InboxMessage> messages, string inboxMessageTable, DateTime lockedUntil);
    
    Task ProcessInboxMessageAsync(InboxMessage message, string inboxMessageTable);
    
    Task ProcessInboxMessagesAsync(IReadOnlyCollection<InboxMessage> messages, string inboxMessageTable);
}