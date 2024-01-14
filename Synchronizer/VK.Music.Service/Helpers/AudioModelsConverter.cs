using MusicServices.Models;
using VkNet.Model;

namespace VK.Music.Service.Helpers;

public static class AudioModelsConverter
{
    public static Playlist FromVkModel(this AudioPlaylist playlist)
    {
        return new Playlist(
            playlist.Id!.Value,
            playlist.Title,
            playlist.Photo.Photo600,
            null);
    }


    public static Track FromVkModel(this Audio audio)
    {
        return new Track(audio.Title, audio.Artist, audio.Album?.Title);
    }
}