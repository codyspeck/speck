namespace Speck.DurableMessaging.Outbox;

public class OutboxMessageContext(Guid outboxMessageId, string outboxMessageTable, object message)
{
    public Guid OutboxMessageId { get; } = outboxMessageId;

    public string OutboxMessageTable { get; } = outboxMessageTable;

    public object Message { get; } = message;
}