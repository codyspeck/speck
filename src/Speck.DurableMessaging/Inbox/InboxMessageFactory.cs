using System.Text.Json;
using System.Text.Json.Serialization;

namespace Speck.DurableMessaging.Inbox;

internal class InboxMessageFactory(InboxMessageTypeCollection typeCollection, MessageSerializer serializer)
{
    public InboxMessage Create(object message) => new()
    {
        Id = Guid.CreateVersion7(),
        Content = serializer.Serialize(message),
        Type = typeCollection.Get(message),
        CreatedAt = DateTime.UtcNow
    };
}