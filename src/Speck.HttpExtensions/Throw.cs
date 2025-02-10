using System.Runtime.CompilerServices;

namespace Speck.HttpExtensions;

internal static class Throw
{
    public static async Task<T> ThrowIfNull<T>(
        this Task<T?> argument,
        [CallerArgumentExpression(nameof(argument))] string paramName = "")
    {
        var result = await argument;
        
        if (result is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return result;
    } 
}
