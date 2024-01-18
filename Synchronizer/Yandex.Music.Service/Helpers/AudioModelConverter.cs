﻿using MusicServices.Models;
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
            yPlaylist.Owner.Uid!,
            yPlaylist.FromYandexModelTracks()
        );
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