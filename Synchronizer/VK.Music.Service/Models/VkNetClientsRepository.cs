using VK.Music.Service.Models.Auth;
using VkNet;

namespace VK.Music.Service.Models;

public class VkNetClientsRepository : IVkNetClientsRepository
{
    private readonly VkApiFactory vkApiFactory;
    private readonly IVkNetApiAuthService vkNetAuthService;

    public VkNetClientsRepository(IVkNetApiAuthService vkNetAuthService, VkApiFactory vkApiFactory)
    {
        this.vkNetAuthService = vkNetAuthService;
        this.vkApiFactory = vkApiFactory;
    }

    public async Task<VkApi> GetAuthenticatedVkNetApiAsync(string username)
    {
        var api = vkApiFactory.CreateApiClient();
        await vkNetAuthService.AuthAsync(api, username);
        return api;
    }
}