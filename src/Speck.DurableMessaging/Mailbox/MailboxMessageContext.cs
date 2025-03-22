namespace Speck.DurableMessaging.Mailbox;

public class MailboxMessageContext(Guid mailboxMessageId, string mailboxMessageTable, object message)
{
    public Guid MailboxMessageId { get; } = mailboxMessageId;

    public string MailboxMessageTable { get; } = mailboxMessageTable;

    public object Message { get; } = message;
}