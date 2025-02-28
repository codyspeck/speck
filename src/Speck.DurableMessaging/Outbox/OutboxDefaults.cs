﻿namespace Speck.DurableMessaging.Outbox;

internal static class OutboxDefaults
{
    public const string Table = "outbox_messages";

    public const int BatchSize = 10;
    
    public const int BoundedCapacity = 100;
    
    public const int MaxDegreeOfParallelism = 1;
    
    public const int PollSize = 100;

    public static readonly TimeSpan BatchTimeout = TimeSpan.FromMilliseconds(20);
    
    public static readonly TimeSpan IdlePollingInterval = TimeSpan.FromSeconds(3);
    
    public static readonly TimeSpan MessageLockDuration = TimeSpan.FromSeconds(30);
}