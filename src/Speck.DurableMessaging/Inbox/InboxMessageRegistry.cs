namespace Speck.DurableMessaging.Inbox;

internal class InboxMessageRegistry
{
    private readonly Dictionary<Type, string> _messageStrings = [];
    private readonly Dictionary<string, Type> _messageTypes = [];

    public void Add<TMessage>(string messageType)
    {
        _messageStrings.Add(typeof(TMessage), messageType);
        _messageTypes.Add(messageType, typeof(TMessage));
    }
}