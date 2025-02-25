using System.Collections.Concurrent;

namespace Speck.DurableMessaging.Inbox;

internal class InboxMessageTables(IEnumerable<InboxConfiguration> inboxConfigurations)
{
    private readonly ConcurrentDictionary<Type, string> _inboxTables = [];

    public string GetInboxTable(Type inboxMessageType)
    {
        return _inboxTables.GetOrAdd(inboxMessageType, type =>
        {
            var inbox =
                inboxConfigurations.FirstOrDefault(configuration => configuration.InboxMessageTypes.Contains(type)) ??
                inboxConfigurations.FirstOrDefault(configuration => configuration.InboxMessageTypes.Count == 0) ??
                throw new InvalidOperationException($"No suitable inboxes configured for message type {inboxMessageType}.");

            return inbox.Table;
        });
    }
}