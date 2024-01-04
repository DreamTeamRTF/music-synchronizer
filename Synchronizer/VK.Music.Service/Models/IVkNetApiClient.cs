using VkNet.Model;

namespace VK.Music.Service.Models;

public interface IVkNetApiClient
{
    public Task<Audio[]> GetTrackAsync(string login, int playlistId);
    public Task<AudioPlaylist[]> GetOwnPlaylistsAsync(string login);
}