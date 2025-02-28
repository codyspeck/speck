namespace Speck.DurableMessaging.Outbox;

internal sealed class OutboxSignalScope(OutboxSignals signals) : IDisposable
{
    private readonly HashSet<string> _outboxTables = [];

    public void Signal(string outboxTable)
    {
        _outboxTables.Add(outboxTable);
    }
    
    public void Dispose()
    {
        foreach (var outboxTable in _outboxTables)
            signals.Signal(outboxTable);
    }
}