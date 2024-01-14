using MusicServices.Models;

namespace VK.Music.Service.Models.Account;

public class VkAccountService : IVkAccountService
{
    private readonly IVkNetApiClient apiClient;

    public VkAccountService(IVkNetApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public Task<AccountInfoModel> GetAccountInfoAsync(string username)
    {
        return apiClient.GetAccountInfoAsync(username);
    }
}