using MusicServices.Models;
using MusicServices.Models.Exceptions;
using VK.Music.Service.Exceptions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;

namespace VK.Music.Service.Models;

public class VkNetApiClient : IVkNetApiClient
{
    private readonly IVkNetClientsRepository vkNetClientsRepository;

    public VkNetApiClient(IVkNetClientsRepository vkNetClientsRepository)
    {
        this.vkNetClientsRepository = vkNetClientsRepository;
    }

    public async Task<Audio[]> GetTracksFromPlaylistAsync(string username, int playlistId)
    {
        var api = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(username);
        var audios = await api.Audio
            .GetAsync(new AudioGetParams { OwnerId = api.UserId, PlaylistId = playlistId })
            .ConfigureAwait(false);

        if (audios is null) throw new TrackNotFoundException();

        return audios.ToArray();
    }

    public async Task<AudioPlaylist[]> GetOwnPlaylistsAsync(string username)
    {
        var api = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(username);
        var playlists = await api.Audio
            .GetPlaylistsAsync(api.UserId!.Value, 100)
            .ConfigureAwait(false);

        var ownPlaylists = playlists.Where(p => p.Original == null || p.Original.OwnerId == api.UserId);
        if (ownPlaylists is null) throw new TrackNotFoundException();
        return ownPlaylists.ToArray();
    }

    public async Task<AccountInfoModel> GetAccountInfoAsync(string username)
    {
        var client = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(username);
        var account = await client.Account.GetProfileInfoAsync();
        var photos = await client.Photo.GetAsync(
            new PhotoGetParams
            {
                OwnerId = client.UserId,
                AlbumId = PhotoAlbumType.Profile
            });

        var photoUrl = photos
            .OrderByDescending(x => x.CreateTime)
            .First()
            .Sizes
            .OrderByDescending(x => x.Height)
            .First()
            .Url;

        return new AccountInfoModel
        {
            Name = account.GetAccountFullName(),
            ImageUrl = photoUrl.ToString()
        };
    }
}