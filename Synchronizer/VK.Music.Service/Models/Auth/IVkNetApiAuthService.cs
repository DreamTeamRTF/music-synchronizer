using VkNet;

namespace VK.Music.Service.Models.Auth;

public interface IVkNetApiAuthService
{
    public Task<VkApi> AuthAsync(VkApi api, string login);
    public Task CreateAuthSessionAsync(string login, string password);
    public void AddTestToken(string login, AuthorizationParameters authParams);
}