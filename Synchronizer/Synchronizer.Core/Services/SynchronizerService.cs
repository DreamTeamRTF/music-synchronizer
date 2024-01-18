using MusicServices.Models;
using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;

namespace Synchronizer.Core.Services;

public class SynchronizerService : ISynchronizerService
{
    private readonly IVkMusicClient vkMusicClient;
    private readonly IYandexMusicClient yandexMusicClient;

    public SynchronizerService(IVkMusicClient vkMusicClient, IYandexMusicClient yandexMusicClient)
    {
        this.vkMusicClient = vkMusicClient;
        this.yandexMusicClient = yandexMusicClient;
    }

    public Task<Playlist> SyncPlaylistAsync()
    {
        throw new NotImplementedException();
    }
}