namespace Speck.DurableMessaging.Inbox;

internal static class InboxDefaults
{
    public const string Table = "inbox_messages";

    public const int BoundedCapacity = 100;
    
    public const int MaxDegreeOfParallelism = 1;
    
    public const int PollSize = 100;
    
    public static readonly TimeSpan IdlePollingInterval = TimeSpan.FromSeconds(3);
}