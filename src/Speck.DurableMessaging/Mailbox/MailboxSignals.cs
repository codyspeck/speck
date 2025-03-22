using System.Collections.Concurrent;

namespace Speck.DurableMessaging.Mailbox;

internal class MailboxSignals
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource> _taskCompletionSources = [];

    public Task Get(string mailboxMessageTableName)
    {
        return _taskCompletionSources.GetOrAdd(mailboxMessageTableName, new TaskCompletionSource()).Task;
    }

    public void Signal(string mailboxMessageTableName)
    {
        _taskCompletionSources.GetOrAdd(mailboxMessageTableName, new TaskCompletionSource()).TrySetResult();
    }

    public void Reset(string mailboxMessageTableName)
    {
        if (Get(mailboxMessageTableName).IsCompleted)
            _taskCompletionSources.TryRemove(mailboxMessageTableName, out _);
    }
}