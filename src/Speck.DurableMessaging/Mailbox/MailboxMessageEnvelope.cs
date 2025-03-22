namespace Speck.DurableMessaging.Mailbox;

public class MailboxMessageEnvelope(object message)
{
    public object Message { get; } = message;

    public string? MessageKey { get; private set; }

    public DateTime? LockedUntil { get; private set; }

    public MailboxMessageEnvelope WithMessageKey(string messageKey)
    {
        MessageKey = messageKey;
        return this;
    }

    public MailboxMessageEnvelope WithLockedUntil(DateTime lockedUntil)
    {
        LockedUntil = lockedUntil;
        return this;
    }
}