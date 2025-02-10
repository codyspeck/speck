using Microsoft.Extensions.Internal;

namespace Speck.HttpExtensions;

internal class TestingOptions
{
    public ISystemClock? Clock { get; init; }
}
