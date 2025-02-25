using System.Collections.Concurrent;

namespace Speck.DurableMessaging.Inbox;

internal class InboxSignals
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource> _taskCompletionSources = [];

    public Task Get(string inboxMessageTableName)
    {
        return _taskCompletionSources.GetOrAdd(inboxMessageTableName, new TaskCompletionSource()).Task;
    }

    public void Signal(string inboxMessageTableName)
    {
        _taskCompletionSources.GetOrAdd(inboxMessageTableName, new TaskCompletionSource()).TrySetResult();
    }

    public void Reset(string inboxMessageTableName)
    {
        if (Get(inboxMessageTableName).IsCompleted)
            _taskCompletionSources.TryRemove(inboxMessageTableName, out _);
    }
}