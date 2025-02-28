namespace Speck.DurableMessaging.Outbox;

internal class OutboxMessageTypeCollection
{
    private readonly Dictionary<Type, string> _messageStrings = [];
    private readonly Dictionary<string, Type> _messageTypes = [];

    public void Add<TMessage>(string messageType)
    {
        _messageStrings.Add(typeof(TMessage), messageType);
        _messageTypes.Add(messageType, typeof(TMessage));
    }

    public string Get(object message)
    {
        return _messageStrings[message.GetType()];
    }

    public Type Get(string messageType)
    {
        return _messageTypes[messageType];
    }
}