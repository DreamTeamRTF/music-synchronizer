using VK.Music.Service.Exceptions;

namespace VK.Music.Service.Models.Auth;

public class DefaultTwoFactorProvider : ITwoFactorVkProvider
{
    public Task<string> GetAuthCodeAsync()
    {
        throw new NotImplementedException();
    }

    public string GetAuthCode(string username)
    {
        if (TwoFactorRequiredUsers.RequiredSecondFactor.TryGetValue(username, out var code))
        {
            return code;
        }

        throw new AuthApiException("Code not found");
    }
}