namespace Speck.DurableMessaging.Outbox;

public class OutboxMessageBatchHandlerConfiguration
{
    internal int MaxDegreeOfParallelism { get; private set; } = OutboxDefaults.MaxDegreeOfParallelism;

    internal int BatchSize { get; private set; } = OutboxDefaults.BatchSize;
    
    internal int BoundedCapacity { get; private set; } = OutboxDefaults.BoundedCapacity;

    internal TimeSpan BatchTimeout { get; private set; } = OutboxDefaults.BatchTimeout;
    
    internal OutboxMessageBatchHandlerConfiguration()
    {
    }

    /// <summary>
    /// The maximum number of messages to batch.
    /// </summary>
    /// <param name="batchSize">The batch size.</param>
    /// <returns>This.</returns>
    public OutboxMessageBatchHandlerConfiguration WithBatchSize(int batchSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(batchSize, 0);
        BatchSize = batchSize;
        return this;
    }

    /// <summary>
    /// The duration to wait between receiving messages before the buffer is flushed and a batch is processed with
    /// fewer messages than the configured count.
    /// </summary>
    /// <param name="timeout">The batch timeout.</param>
    /// <returns>This.</returns>
    public OutboxMessageBatchHandlerConfiguration WithBatchTimeout(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeout, TimeSpan.Zero);
        BatchTimeout = timeout;
        return this;
    }
    
    /// <summary>
    /// The number of messages this handler can buffer.
    /// </summary>
    /// <param name="boundedCapacity">The size of the buffer.</param>
    /// <returns>This.</returns>
    public OutboxMessageBatchHandlerConfiguration WithBoundedCapacity(int boundedCapacity)
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
    public OutboxMessageBatchHandlerConfiguration WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxDegreeOfParallelism, 0);
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        return this;
    }
}