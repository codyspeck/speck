namespace Speck.DurableMessaging.Mailbox;

internal sealed class MailboxSignalScope(MailboxSignals signals) : IDisposable
{
    private readonly HashSet<string> _mailboxTables = [];

    public void Signal(string mailboxTable)
    {
        _mailboxTables.Add(mailboxTable);
    }
    
    public void Dispose()
    {
        foreach (var mailboxTable in _mailboxTables)
            signals.Signal(mailboxTable);
    }
}