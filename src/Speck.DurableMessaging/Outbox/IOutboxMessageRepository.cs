namespace Speck.DurableMessaging.Outbox;

internal interface IOutboxMessageRepository
{
    Task<OutboxMessage> GetOutboxMessageAsync(Guid outboxMessageId, string outboxMessageTable);
    
    Task<IReadOnlyCollection<OutboxMessage>> GetOutboxMessagesAsync(string outboxMessageTable, int count);
    
    Task<IReadOnlyCollection<OutboxMessage>> GetOutboxMessagesAsync(IEnumerable<Guid> outboxMessageIds, string outboxMessageTable);

    Task InsertAsync(OutboxMessage outboxMessage, string outboxMessageTable);
    
    Task LockOutboxMessagesAsync(IReadOnlyCollection<OutboxMessage> messages, string outboxMessageTable, DateTime lockedUntil);
    
    Task ProcessOutboxMessageAsync(OutboxMessage message, string outboxMessageTable);
    
    Task ProcessOutboxMessagesAsync(IReadOnlyCollection<OutboxMessage> messages, string outboxMessageTable);
}