namespace Speck.DurableMessaging.Mailbox;

internal class Mailbox(
    IMailboxMessageRepository repository,
    MailboxMessageFactory factory,
    MailboxMessageTables tables,
    MailboxSignalScope signals) : IMailbox
{
    public Task InsertAsync(object message)
    {
        return InsertAsync(new MailboxMessageEnvelope(message));
    }

    public async Task InsertAsync(MailboxMessageEnvelope envelope)
    {
        var mailboxMessage = factory.Create(envelope);

        var mailboxTable = tables.GetMailboxTable(envelope.Message.GetType());
        
        await repository.InsertAsync(mailboxMessage, mailboxTable);
        
        signals.Signal(mailboxTable);
    }
}