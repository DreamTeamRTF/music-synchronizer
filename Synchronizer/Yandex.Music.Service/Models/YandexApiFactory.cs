using Yandex.Music.Api;
using Yandex.Music.Client;

namespace Yandex.Music.Service.Models
{
    public class YandexApiFactory
    {
        public YandexMusicClientAsync CreateApiClient()
        {
            return new YandexMusicClientAsync();
        }
    }
}
