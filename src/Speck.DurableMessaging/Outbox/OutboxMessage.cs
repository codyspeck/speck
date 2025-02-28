namespace Speck.DurableMessaging.Outbox;

internal class OutboxMessage
{
    public Guid Id { get; init; }

    public string Type { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public string? MessageKey { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? LockedUntil { get; init; }

    public DateTime? ProcessedAt { get; init; }
}