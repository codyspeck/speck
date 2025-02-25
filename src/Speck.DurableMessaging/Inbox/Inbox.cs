namespace Speck.DurableMessaging.Inbox;

internal class Inbox(
    IInboxMessageRepository repository,
    InboxMessageFactory factory,
    InboxMessageTables tables,
    InboxSignalScope signals) : IInbox
{
    public Task InsertAsync(object message)
    {
        return InsertAsync(new InboxMessageEnvelope(message));
    }

    public async Task InsertAsync(InboxMessageEnvelope envelope)
    {
        var inboxMessage = factory.Create(envelope);

        var inboxTable = tables.GetInboxTable(envelope.Message.GetType());
        
        await repository.InsertAsync(inboxMessage, inboxTable);
        
        signals.Signal(inboxTable);
    }
}