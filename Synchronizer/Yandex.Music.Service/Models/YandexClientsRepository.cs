using Yandex.Music.Client;
using Yandex.Music.Service.Models.Auth;

namespace Yandex.Music.Service.Models;

public class YandexClientsRepository
{
    private readonly RepositoryYandexMusicAuthService yandexMusicService;

    public YandexClientsRepository(RepositoryYandexMusicAuthService yandexMusicService)
    {
        this.yandexMusicService = yandexMusicService;
    }

    public async Task<YandexMusicClientAsync> GetAuthenticatedYandexApiAsync(string username)
    {

        return await yandexMusicService.AuthAsync(username);
    }
}