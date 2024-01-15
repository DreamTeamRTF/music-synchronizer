using Yandex.Music.Client;

namespace Yandex.Music.Service.Models;

public static class YandexApiFactory
{
    public static YandexMusicClientAsync CreateApiClient()
    {
        return new YandexMusicClientAsync();
    }
}