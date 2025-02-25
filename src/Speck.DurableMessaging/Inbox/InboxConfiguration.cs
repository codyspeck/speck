namespace Speck.DurableMessaging.Inbox;

public class InboxConfiguration
{
    internal string Table { get; private set; } = InboxDefaults.Table;

    internal int PollSize { get; private set; } = InboxDefaults.PollSize;

    internal TimeSpan IdlePollingInterval { get; private set; } = InboxDefaults.IdlePollingInterval;

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
    /// Configures the name of the inbox message table to use. The default is "inbox_messages".
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
    /// is 100.
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