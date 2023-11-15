namespace Speck.Http.Authorization;

internal record Token(string AccessToken, int ExpiresIn);
