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

    public async Task<Track[]> GetPlaylistTracks(PlaylistRequest request)
    {
        var audios = await apiClient.GetTrackAsync("89527357839", request.PlaylistId);
        return audios.Select(a => a.FromVkModel()).ToArray();
    }

    public async Task<Playlist[]> GetOwnPlaylistsAsync()
    {
        var playlists = await apiClient.GetOwnPlaylistsAsync("89527357839");
        return playlists.Select(p => p.FromVkModel()).ToArray();
    }
}