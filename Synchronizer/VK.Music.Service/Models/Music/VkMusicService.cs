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
        var audios = await apiClient.GetTracksFromPlaylistAsync(request.Username, request.PlaylistId);
        return audios.Select(a => a.FromVkModel()).ToArray();
    }

    public async Task<Playlist[]> GetOwnPlaylistsAsync(OwnPlaylistsRequest request)
    {
        var playlists = await apiClient.GetOwnPlaylistsAsync(request.Username);
        return playlists.Select(p => p.Item1.FromVkModel(p.Item2)).ToArray();
    }

    public async Task<Playlist> AddPlaylistAsync(PlaylistToAddRequest request)
    {
        var playlist = await apiClient.SavePlaylistAsync(request.Username, request.Playlist);
        return playlist.Item1.FromVkModel(playlist.Item2);
    }

    public async Task<Playlist?> FindPlaylistByIdAsync(FindPlaylistByIdRequest request)
    {
        var playlist = await apiClient.FindByIdAsync(request.Username, request.PlaylistId);
        return playlist.Item1.FromVkModel(playlist.Item2); 
    }
}