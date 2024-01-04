using MusicServices.Models;
using MusicServices.Models.Contracts;
using VK.Music.Service.Helpers;

namespace VK.Music.Service.Models;

public class VkMusicService : IMusicService
{
    private readonly IVkNetApiClient apiClient;

    public VkMusicService(IVkNetApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<Track[]> GetPlaylistTracksAsync(PlaylistRequest request)
    {
        var audios = await apiClient.GetTracksFromPlaylist(request.Login, request.PlaylistId);
        return audios.Select(a => a.FromVkModel()).ToArray();
    }

    public async Task<Playlist[]> GetOwnPlaylistsAsync(OwnTracksRequest request)
    {
        var playlists = await apiClient.GetOwnPlaylistsAsync(request.Login);
        return playlists.Select(p => p.FromVkModel()).ToArray();
    }
}