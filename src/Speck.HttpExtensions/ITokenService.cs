namespace Speck.HttpExtensions;

internal interface ITokenService
{
    Task<Token> GetTokenAsync();
}
