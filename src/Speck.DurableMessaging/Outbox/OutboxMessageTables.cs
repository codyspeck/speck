using System.Collections.Concurrent;

namespace Speck.DurableMessaging.Outbox;

internal class OutboxMessageTables(IEnumerable<OutboxConfiguration> outboxConfigurations)
{
    private readonly ConcurrentDictionary<Type, string> _outboxTables = [];

    public string GetOutboxTable(Type outboxMessageType)
    {
        return _outboxTables.GetOrAdd(outboxMessageType, type =>
        {
            var outbox =
                outboxConfigurations.FirstOrDefault(configuration => configuration.OutboxMessageTypes.Contains(type)) ??
                outboxConfigurations.FirstOrDefault(configuration => configuration.OutboxMessageTypes.Count == 0) ??
                throw new InvalidOperationException($"No suitable Outboxes configured for message type {outboxMessageType}.");

            return outbox.Table;
        });
    }
}