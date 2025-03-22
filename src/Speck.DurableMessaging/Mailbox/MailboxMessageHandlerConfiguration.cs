namespace Speck.DurableMessaging.Mailbox;

public class MailboxMessageHandlerConfiguration
{
    internal int MaxDegreeOfParallelism { get; private set; } = MailboxDefaults.MaxDegreeOfParallelism;

    internal int BoundedCapacity { get; private set; } = MailboxDefaults.BoundedCapacity;

    internal MailboxMessageHandlerConfiguration()
    {
    }
    
    /// <summary>
    /// The number of messages this handler can buffer.
    /// </summary>
    /// <param name="boundedCapacity">The size of the buffer.</param>
    /// <returns>This.</returns>
    public MailboxMessageHandlerConfiguration WithBoundedCapacity(int boundedCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(boundedCapacity, 0);
        BoundedCapacity = boundedCapacity;
        return this;
    }
    
    /// <summary>
    /// The maximum number of messages to handle in parallel at any given moment.
    /// </summary>
    /// <param name="maxDegreeOfParallelism">The max degree of parallelism.</param>
    /// <returns>This.</returns>
    public MailboxMessageHandlerConfiguration WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxDegreeOfParallelism, 0);
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        return this;
    }
}