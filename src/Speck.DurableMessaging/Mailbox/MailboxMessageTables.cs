using System.Collections.Concurrent;

namespace Speck.DurableMessaging.Mailbox;

internal class MailboxMessageTables(IEnumerable<MailboxConfiguration> mailboxConfigurations)
{
    private readonly ConcurrentDictionary<Type, string> _mailboxTables = [];

    public string GetMailboxTable(Type mailboxMessageType)
    {
        return _mailboxTables.GetOrAdd(mailboxMessageType, type =>
        {
            var mailbox =
                mailboxConfigurations.FirstOrDefault(configuration => configuration.MailboxMessageTypes.Contains(type)) ??
                mailboxConfigurations.FirstOrDefault(configuration => configuration.MailboxMessageTypes.Count == 0) ??
                throw new InvalidOperationException($"No suitable mailboxes configured for message type {mailboxMessageType}.");

            return mailbox.Table;
        });
    }
}