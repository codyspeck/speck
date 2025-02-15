using System.Text.Json;
using System.Text.Json.Serialization;

namespace Speck.DurableMessaging;

internal class MessageSerializer
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public string Serialize(object message) => JsonSerializer.Serialize(message, JsonSerializerOptions);
    
    public object Deserialize(string json, Type messageType) =>
        JsonSerializer.Deserialize(json, messageType, JsonSerializerOptions)!;
}