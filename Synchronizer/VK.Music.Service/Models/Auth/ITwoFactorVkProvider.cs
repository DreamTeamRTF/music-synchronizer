namespace VK.Music.Service.Models.Auth;

public interface ITwoFactorVkProvider
{
    public Task<string> GetAuthCodeAsync();
    public string GetAuthCode();
}