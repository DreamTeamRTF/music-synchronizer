using MusicServices.Models;
using MusicServices.Models.Contracts;
using VK.Music.Service.Helpers;

namespace VK.Music.Service.Models.Music;

public class VkMusicService : IMusicService
{
    private readonly IVkNetApiClient apiClient;

    public VkMusicService(IVkNetApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<Track[]> GetPlaylistTracksAsync(PlaylistRequest request)
    {
        var audios = await apiClient.GetTracksFromPlaylistAsync(request.Username, request.PlaylistId)
            .ConfigureAwait(false);
        return audios.Select(a => a.FromVkModel()).ToArray();
    }

    public async Task<Playlist[]> GetOwnPlaylistsAsync(OwnPlaylistsRequest request)
    {
        var playlists = await apiClient.GetOwnPlaylistsAsync(request.Username).ConfigureAwait(false);
        return playlists.Select(p => p.FromVkModel()).ToArray();
    }
}