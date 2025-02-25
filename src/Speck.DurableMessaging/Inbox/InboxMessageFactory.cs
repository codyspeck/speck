namespace Speck.DurableMessaging.Inbox;

internal class InboxMessageFactory(InboxMessageTypeCollection typeCollection, MessageSerializer serializer)
{
    public InboxMessage Create(InboxMessageEnvelope envelope) => new()
    {
        Id = Guid.CreateVersion7(),
        Content = serializer.Serialize(envelope.Message),
        Type = typeCollection.Get(envelope.Message.GetType()),
        MessageKey = envelope.MessageKey,
        CreatedAt = DateTime.UtcNow,
        LockedUntil = envelope.LockedUntil
    };
}