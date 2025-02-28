namespace Speck.DurableMessaging.Outbox;

public class OutboxMessageEnvelope(object message)
{
    public object Message { get; } = message;

    public string? MessageKey { get; private set; }

    public DateTime? LockedUntil { get; private set; }

    public OutboxMessageEnvelope WithMessageKey(string messageKey)
    {
        MessageKey = messageKey;
        return this;
    }

    public OutboxMessageEnvelope WithLockedUntil(DateTime lockedUntil)
    {
        LockedUntil = lockedUntil;
        return this;
    }
}