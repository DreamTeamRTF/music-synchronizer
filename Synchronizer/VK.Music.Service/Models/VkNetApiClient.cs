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

    public async Task<(AudioPlaylist, Audio[])[]> GetOwnPlaylistsAsync(string username)
    {
        var api = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(username);
        var playlists = await api.Audio
            .GetPlaylistsAsync(api.UserId!.Value, 100)
            .ConfigureAwait(false);

        var ownPlaylists = playlists.Where(p => p.Original == null || p.Original.OwnerId == api.UserId);
        if (ownPlaylists is null) throw new TrackNotFoundException();
        var resulted = new List<(AudioPlaylist, Audio[])>();
        foreach (var audioPlaylist in ownPlaylists)
        {
            var audios = await api.Audio.GetAsync(new AudioGetParams { OwnerId = api.UserId, PlaylistId = audioPlaylist.Id });
            resulted.Add((audioPlaylist, audios.ToArray()));
        }

        return resulted.ToArray();
    }

    public async Task<(AudioPlaylist, Audio[])> SavePlaylistAsync(string username, Playlist playlist)
    {
        var api = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(username);
        var audiosIds = new List<string>();
        foreach (var track in playlist.Tracks!)
        {
            var foundAudios = await api.Audio.SearchAsync(new AudioSearchParams { Query = $"{track.Author} {track.Title}" });
            if (foundAudios!= null)
            {
                var audio = foundAudios.First();
                audiosIds.Add($"{audio.OwnerId}_{audio.Id}");
            }
        }
        audiosIds.Reverse();

        var createdPlaylist =  await api.Audio.CreatePlaylistAsync(api.UserId!.Value, playlist.Title, audioIds: audiosIds);
        var audios = await api.Audio.GetAsync(new AudioGetParams { OwnerId = api.UserId, PlaylistId = createdPlaylist.Id });
        return (createdPlaylist, audios.ToArray());
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

    public async Task<(AudioPlaylist, Audio[])> FindByIdAsync(string username, long playlistId)
    {
        var client = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(username);
        var playlist = await client.Audio.GetPlaylistByIdAsync(client.UserId!.Value, playlistId);
        var audios = await client.Audio.GetAsync(new AudioGetParams { OwnerId = client.UserId, PlaylistId = playlist.Id });
        return (playlist, audios.ToArray());
    }
}