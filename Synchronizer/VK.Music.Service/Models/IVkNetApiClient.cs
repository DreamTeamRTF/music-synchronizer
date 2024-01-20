using MusicServices.Models;
using VkNet.Model;

namespace VK.Music.Service.Models;

public interface IVkNetApiClient
{
    public Task<Audio[]> GetTracksFromPlaylistAsync(string username, int playlistId);
    public Task<(AudioPlaylist, Audio[])[]> GetOwnPlaylistsAsync(string username);
    public Task<(AudioPlaylist, Audio[])> SavePlaylistAsync(string username, Playlist playlist);
    public Task<AccountInfoModel> GetAccountInfoAsync(string username);
    public Task<(AudioPlaylist, Audio[])> FindByIdAsync(string username, long playlistId);
}