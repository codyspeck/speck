namespace Speck.DurableMessaging.Mailbox;

internal class MailboxMessageFactory(MailboxMessageTypeCollection typeCollection, MessageSerializer serializer)
{
    public MailboxMessage Create(MailboxMessageEnvelope envelope) => new()
    {
        Id = Guid.CreateVersion7(),
        Content = serializer.Serialize(envelope.Message),
        Type = typeCollection.Get(envelope.Message),
        MessageKey = envelope.MessageKey,
        CreatedAt = DateTime.UtcNow,
        LockedUntil = envelope.LockedUntil
    };
}