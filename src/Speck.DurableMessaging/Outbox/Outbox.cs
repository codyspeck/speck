namespace Speck.DurableMessaging.Outbox;

internal class Outbox(
    IOutboxMessageRepository repository,
    OutboxMessageFactory factory,
    OutboxMessageTables tables,
    OutboxSignalScope signals) : IOutbox
{
    public Task InsertAsync(object message)
    {
        return InsertAsync(new OutboxMessageEnvelope(message));
    }

    public async Task InsertAsync(OutboxMessageEnvelope envelope)
    {
        var outboxMessage = factory.Create(envelope);

        var outboxTable = tables.GetOutboxTable(envelope.Message.GetType());
        
        await repository.InsertAsync(outboxMessage, outboxTable);
        
        signals.Signal(outboxTable);
    }
}