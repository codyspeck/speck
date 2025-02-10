using Microsoft.Extensions.Internal;

namespace Speck.HttpExtensions.UnitTests;

public class FakeSystemClock : ISystemClock
{
    private DateTimeOffset _now = DateTimeOffset.UtcNow;

    public DateTimeOffset UtcNow => _now;

    public void AddSeconds(int seconds)
    {
        _now = _now.Add(TimeSpan.FromSeconds(seconds));
    }
}