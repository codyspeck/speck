using System.Collections.Concurrent;

namespace Speck.DurableMessaging.Outbox;

internal class OutboxSignals
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource> _taskCompletionSources = [];

    public Task Get(string outboxMessageTableName)
    {
        return _taskCompletionSources.GetOrAdd(outboxMessageTableName, new TaskCompletionSource()).Task;
    }

    public void Signal(string outboxMessageTableName)
    {
        _taskCompletionSources.GetOrAdd(outboxMessageTableName, new TaskCompletionSource()).TrySetResult();
    }

    public void Reset(string outboxMessageTableName)
    {
        if (Get(outboxMessageTableName).IsCompleted)
            _taskCompletionSources.TryRemove(outboxMessageTableName, out _);
    }
}