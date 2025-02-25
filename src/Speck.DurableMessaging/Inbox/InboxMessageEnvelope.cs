namespace Speck.DurableMessaging.Inbox;

public class InboxMessageEnvelope(object message)
{
    public object Message { get; } = message;

    public string? MessageKey { get; private set; }

    public DateTime? LockedUntil { get; private set; }

    public InboxMessageEnvelope WithMessageKey(string messageKey)
    {
        MessageKey = messageKey;
        return this;
    }

    public InboxMessageEnvelope WithLockedUntil(DateTime lockedUntil)
    {
        LockedUntil = lockedUntil;
        return this;
    }
}