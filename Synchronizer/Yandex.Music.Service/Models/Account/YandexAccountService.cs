using MusicServices.Models;

namespace Yandex.Music.Service.Models.Account;

public class YandexAccountService
{
    private readonly YandexApiClient apiClient;

    public YandexAccountService(YandexApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public Task<AccountInfoModel> GetAccountInfoAsync(string username)
    {
        return apiClient.GetAccountInfoAsync(username);
    }
}