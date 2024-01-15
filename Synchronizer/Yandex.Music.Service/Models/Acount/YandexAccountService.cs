using MusicServices.Models;

namespace Yandex.Music.Service.Models.Acount
{
    public class YandexAccountService
    {
        private readonly YandexApiClient _apiClient;

        public YandexAccountService(YandexApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<AccountInfoModel> GetAccountInfoAsync(string username)
        {
            return _apiClient.GetAccountInfoAsync(username);
        }
    }
}
