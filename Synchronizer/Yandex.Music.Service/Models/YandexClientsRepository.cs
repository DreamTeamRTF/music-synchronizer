using Yandex.Music.Client;
using Yandex.Music.Service.Models.Auth;

namespace Yandex.Music.Service.Models
{
    public class YandexClientsRepository
    {
        private readonly YandexApiFactory _apiFactory;
        private readonly InMemoryYandexMusicAuthService _yandexMusicService;

        public YandexClientsRepository(InMemoryYandexMusicAuthService yandexMusicService, YandexApiFactory apiFactory)
        {
            _apiFactory = apiFactory;
            _yandexMusicService = yandexMusicService;
        }

        public async Task<YandexMusicClientAsync> GetAuthentificatedYandexApi(string login)
        {
            var api = _apiFactory.CreateApiClient();
            await _yandexMusicService.AuthAsync(api, login);
            return api;
        }
    }
}
