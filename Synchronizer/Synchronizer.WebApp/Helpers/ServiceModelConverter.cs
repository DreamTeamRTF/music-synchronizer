using Synchronizer.Core;
using Synchronizer.WebApp.Models.Synchronizer.Playlists;

namespace Synchronizer.WebApp.Helpers;

public static class ServiceModelConverter
{
    public static SynchronizedPlaylistModel FromServiceModel(this PlaylistWithServiceType? playlist)
    {
        var model = new SynchronizedPlaylistModel();
        model.PlaylistId = playlist!.Playlist.Id;
        model.Title = playlist?.Playlist?.Title;
        model.CoverImage = playlist?.Playlist?.ImageUrl;
        model.ServiceType = playlist?.ServiceType.ToMusicServiceType();
        return model;
    }
}