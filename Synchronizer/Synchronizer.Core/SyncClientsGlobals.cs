using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;
using Synchronizer.DAL.Entities;

namespace Synchronizer.Core;

public static class SyncClientsGlobals
{
    public static Dictionary<MusicServiceType, Type> ActiveSyncClients = new()
    {
        { MusicServiceType.Vk, typeof(VkMusicClient) },
        { MusicServiceType.Yandex, typeof(YandexMusicClient) }
    };
}