namespace Speck.Http.Authorization;

internal interface ITokenService
{
    Task<Token> GetToken();
}
