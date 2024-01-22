using MusicServices.Models;
using MusicServices.Models.Contracts;
using Yandex.Music.Service.Helpers;

namespace Yandex.Music.Service.Models.Music;

public class YandexMusicService : IMusicService
{
    private readonly YandexApiClient apiClient;

    public YandexMusicService(YandexApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<Track[]> GetPlaylistTracksAsync(PlaylistRequest request)
    {
        var yTracks = await apiClient.GetTracksFromPlaylistAsync(request.Username, request.PlaylistId)
            .ConfigureAwait(false);
        return yTracks.Select(t => t.FromYandexModel()).ToArray();
    }

    public async Task<Playlist[]> GetOwnPlaylistsAsync(OwnPlaylistsRequest request)
    {
        var playlists = await apiClient.GetOwnPlaylistsAsync(request.Username);
        return playlists.Select(p => p.FromYandexModel()).ToArray();
    }

    public async Task<Playlist> AddPlaylistAsync(PlaylistToAddRequest request)
    {
        var playlist = await apiClient.SavePlaylistAsync(request.Username, request.Playlist);
        return playlist.FromYandexModel();
    }

    public async Task<Playlist?> FindPlaylistByIdAsync(FindPlaylistByIdRequest request)
    {
        var playlist = await apiClient.FindByIdAsync(request.Username, request.PlaylistId);
        return playlist?.FromYandexModel();
    }

    public async Task<Playlist?> SmartPlaylistUpdateAsync(SmartPlaylistUpdateModel request)
    {
        var playlist = await apiClient.SmartUpdatePlaylistAsync(request.Username, request.PlaylistId,
            request.TracksToAdd, request.TracksToRemove);
        return playlist?.FromYandexModel();
    }

    public async Task<Playlist> UpdatePlaylistAsync(PlaylistUpdateModel request)
    {
        var playlist = await apiClient.PlaylistUpdateAsync(request.Username, request.PlaylistId, request.Source);
        return playlist.FromYandexModel();
    }
}