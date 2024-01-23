using MusicServices.Models;
using Yandex.Music.Api.Models.Common.Cover;
using Yandex.Music.Api.Models.Playlist;
using Yandex.Music.Api.Models.Track;

namespace Yandex.Music.Service.Helpers;

public static class AudioModelConverter
{
    public static Playlist FromYandexModel(this YPlaylist yPlaylist)
    {
        return new Playlist
        (
            long.Parse(yPlaylist.Kind),
            yPlaylist.Title,
            yPlaylist.GetImageFromPlaylist(),
            yPlaylist.FromYandexModelTracks()
        );
    }

    private static string GetImageFromPlaylist(this YPlaylist yPlaylist)
    {
        if (yPlaylist.Image is not null) return yPlaylist.Image;
        return yPlaylist.Cover switch
        {
            YCoverMosaic mosaic => mosaic.ItemsUri.First().GetMosaicImageUrl(),
            YCoverImage image => image.Uri.GetProperImageUrl(),
            YCoverPic pic => pic.Uri.GetProperImageUrl(),
            _ =>
                "https://static.vecteezy.com/system/resources/previews/000/533/338/original/lightning-bolt-icon-vector.jpg"
        };
    }

    private static string GetProperImageUrl(this string uri)
    {
        var n = uri.Length;
        return $@"https://{uri[..(n - 2)].Replace("%%", "m200x200")}";
    }

    private static string GetMosaicImageUrl(this string uri)
    {
        var n = uri.Length;
        return $@"https://{uri.Substring(0, n - 2)}200x200";
    }

    public static Track FromYandexModel(this YTrack yTrack)
    {
        return new Track(yTrack.Title,
            yTrack.Artists.First().Name,
            yTrack.Albums?.First().Title);
    }

    public static Track FromYandexModel(this YTrackContainer yTrack)
    {
        return FromYandexModel(yTrack.Track);
    }

    public static Track[] FromYandexModelTracks(this YPlaylist yPlaylist)
    {
        return yPlaylist.Tracks.Select(x => x.FromYandexModel()).ToArray();
    }
}