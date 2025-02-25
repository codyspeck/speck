namespace Speck.DurableMessaging.Inbox;

public class InboxConfiguration
{
    private const string DefaultInboxMessageTable = "inbox_message";
    private const int DefaultPollSize = 1000;
    
    private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(3);

    internal string Table { get; private set; } = DefaultInboxMessageTable;

    internal int PollSize { get; private set; } = DefaultPollSize;

    internal TimeSpan IdlePollingInterval { get; private set; } = DefaultPollingInterval;

    internal HashSet<Type> InboxMessageTypes { get; } = [];

    /// <summary>
    /// Designates messages of a given type to be inserted into this inbox. If an inbox has no message types configured
    /// then it is considered the "default" inbox that will receive all non-configured message types.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>This.</returns>
    public InboxConfiguration HandlesMessageType<TMessage>()
    {
        InboxMessageTypes.Add(typeof(TMessage));
        return this;
    }
    
    /// <summary>
    /// Configures the name of the inbox message table to use. The default is "inbox_message".
    /// </summary>
    /// <param name="table">The inbox message table.</param>
    /// <returns>This.</returns>
    public InboxConfiguration WithTable(string table)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(table);
        Table = table;
        return this;
    }
    
    /// <summary>
    /// Configures the number of inbox messages that will be polled at a time from the inbox message table. The default
    /// is 1000.
    /// </summary>
    /// <param name="pollSize">The poll size.</param>
    /// <returns>This.</returns>
    public InboxConfiguration WithPollSize(int pollSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pollSize, 0);
        PollSize = pollSize;
        return this;
    }

    /// <summary>
    /// Configures the interval at which to poll the inbox message table when no available messages for processing
    /// are queried. The default is three seconds.
    /// </summary>
    /// <param name="idlePollingInterval">The idle polling interval.</param>
    /// <returns>This.</returns>
    public InboxConfiguration WithIdlePollingInterval(TimeSpan idlePollingInterval)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(idlePollingInterval, TimeSpan.Zero);
        IdlePollingInterval = idlePollingInterval;
        return this;
    }
}