namespace Speck.DurableMessaging.Outbox;

public class OutboxConfiguration
{
    internal string Table { get; private set; } = OutboxDefaults.Table;

    internal int PollSize { get; private set; } = OutboxDefaults.PollSize;

    internal TimeSpan IdlePollingInterval { get; private set; } = OutboxDefaults.IdlePollingInterval;

    internal TimeSpan MessageLockDuration { get; private set; } = OutboxDefaults.MessageLockDuration;
    
    internal HashSet<Type> OutboxMessageTypes { get; } = [];

    /// <summary>
    /// Designates messages of a given type to be inserted into this Outbox. If an Outbox has no message types configured
    /// then it is considered the "default" Outbox that will receive all non-configured message types.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>This.</returns>
    public OutboxConfiguration HandlesMessageType<TMessage>()
    {
        OutboxMessageTypes.Add(typeof(TMessage));
        return this;
    }
    
    /// <summary>
    /// Configures the name of the Outbox message table to use. The default is "Outbox_messages".
    /// </summary>
    /// <param name="table">The Outbox message table.</param>
    /// <returns>This.</returns>
    public OutboxConfiguration WithTable(string table)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(table);
        Table = table;
        return this;
    }
    
    /// <summary>
    /// Configures the number of Outbox messages that will be polled at a time from the Outbox message table. The default
    /// is 100.
    /// </summary>
    /// <param name="pollSize">The poll size.</param>
    /// <returns>This.</returns>
    public OutboxConfiguration WithPollSize(int pollSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pollSize, 0);
        PollSize = pollSize;
        return this;
    }

    /// <summary>
    /// Configures the interval at which to poll the Outbox message table when no available messages for processing
    /// are queried. The default is three seconds.
    /// </summary>
    /// <param name="idlePollingInterval">The idle polling interval.</param>
    /// <returns>This.</returns>
    public OutboxConfiguration WithIdlePollingInterval(TimeSpan idlePollingInterval)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(idlePollingInterval, TimeSpan.Zero);
        IdlePollingInterval = idlePollingInterval;
        return this;
    }

    /// <summary>
    /// Configures how long messages are locked for after the Outbox polling process polls and locks the message
    /// until they are considered timed out and eligible to be picked up again. The default is 30 seconds.
    /// </summary>
    /// <param name="messageLockDuration">The duration to lock messages for.</param>
    /// <returns>This.</returns>
    /// <remarks>
    /// This behaves similar to AWS SQS's "Visibility Timeout" concept -
    /// https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/sqs-visibility-timeout.html.
    /// </remarks>
    public OutboxConfiguration WithMessageLockDuration(TimeSpan messageLockDuration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(messageLockDuration, TimeSpan.Zero);
        MessageLockDuration = messageLockDuration;
        return this;
    }
}