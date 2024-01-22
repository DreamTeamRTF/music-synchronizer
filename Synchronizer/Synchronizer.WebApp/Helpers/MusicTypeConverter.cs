using Synchronizer.DAL.Entities;
using Synchronizer.WebApp.Models.Synchronizer.Playlists;

namespace Synchronizer.WebApp.Helpers;

public static class MusicTypeConverter
{
    public static MusicServiceType ToMusicServiceType(this MusicServiceTypeModel typeModel)
    {
        return typeModel switch
        {
            MusicServiceTypeModel.VkMusic => MusicServiceType.Vk,
            MusicServiceTypeModel.YandexMusic => MusicServiceType.Yandex,
            _ => throw new ArgumentOutOfRangeException(nameof(typeModel))
        };
    }

    public static MusicServiceTypeModel ToMusicServiceType(this MusicServiceType typeModel)
    {
        return typeModel switch
        {
            MusicServiceType.Vk => MusicServiceTypeModel.VkMusic,
            MusicServiceType.Yandex => MusicServiceTypeModel.YandexMusic,
            _ => throw new ArgumentOutOfRangeException(nameof(typeModel))
        };
    }
}