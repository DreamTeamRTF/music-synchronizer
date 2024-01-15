using Yandex.Music.Client;
using Yandex.Music.Service.Models.Auth;

namespace Yandex.Music.Service.Models;

public class YandexClientsRepository
{
    private readonly InMemoryYandexMusicAuthService yandexMusicService;

    public YandexClientsRepository(InMemoryYandexMusicAuthService yandexMusicService)
    {
        this.yandexMusicService = yandexMusicService;
    }

    public async Task<YandexMusicClientAsync> GetAuthenticatedYandexApi(string login)
    {
        var api = YandexApiFactory.CreateApiClient();
        return await yandexMusicService.AuthAsync(api, login);
    }
}