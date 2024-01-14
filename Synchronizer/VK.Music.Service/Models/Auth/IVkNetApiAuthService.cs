using VkNet;

namespace VK.Music.Service.Models.Auth;

public interface IVkNetApiAuthService
{
    public Task<VkApi> AuthAsync(VkApi api, string username);
    public Task CreateAuthSessionAsync(string username, string login, string password, string? code);
    public void AddTestToken(string username, AuthorizationParameters authParams);
}