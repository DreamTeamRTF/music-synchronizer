using MusicServices.Models.Exceptions;
using VkNet.Model;

namespace VK.Music.Service.Models;

public class VkNetApiClient : IVkNetApiClient
{
    private readonly IVkNetClientsRepository vkNetClientsRepository;

    public VkNetApiClient(IVkNetClientsRepository vkNetClientsRepository)
    {
        this.vkNetClientsRepository = vkNetClientsRepository;
    }

    public async Task<Audio[]> GetTrackAsync(string login, int playlistId)
    {
        var api = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(login);
        var audios = await api.Audio
            .GetAsync(new AudioGetParams { OwnerId = api.UserId, PlaylistId = playlistId })
            .ConfigureAwait(false);

        if (audios is null) throw new TrackNotFoundException();

        return audios.ToArray();
    }

    public async Task<AudioPlaylist[]> GetOwnPlaylistsAsync(string login)
    {
        var api = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(login);
        var playlists = await api.Audio
            .GetPlaylistsAsync(api.UserId!.Value, 100)
            .ConfigureAwait(false);
        
        var ownPlaylists = playlists.Where(p => p.Original == null || p.Original.OwnerId == api.UserId);
        if (ownPlaylists is null) throw new TrackNotFoundException();
        return ownPlaylists.ToArray();
    }
}