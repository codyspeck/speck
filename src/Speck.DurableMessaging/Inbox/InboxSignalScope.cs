namespace Speck.DurableMessaging.Inbox;

internal sealed class InboxSignalScope(InboxSignals signals) : IDisposable
{
    private readonly HashSet<string> _inboxTables = [];

    public void Signal(string inboxTable)
    {
        _inboxTables.Add(inboxTable);
    }
    
    public void Dispose()
    {
        foreach (var inboxTable in _inboxTables)
            signals.Signal(inboxTable);
    }
}