namespace Speck.DurableMessaging.Outbox;

internal class OutboxMessageFactory(OutboxMessageTypeCollection typeCollection, MessageSerializer serializer)
{
    public OutboxMessage Create(OutboxMessageEnvelope envelope) => new()
    {
        Id = Guid.CreateVersion7(),
        Content = serializer.Serialize(envelope.Message),
        Type = typeCollection.Get(envelope.Message),
        MessageKey = envelope.MessageKey,
        CreatedAt = DateTime.UtcNow,
        LockedUntil = envelope.LockedUntil
    };
}