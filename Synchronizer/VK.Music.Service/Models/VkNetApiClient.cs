using MusicServices.Models;
using MusicServices.Models.Exceptions;
using VK.Music.Service.Exceptions;
using VkNet;
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
            var audios = await api.Audio.GetAsync(new AudioGetParams
                { OwnerId = api.UserId, PlaylistId = audioPlaylist.Id });
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
            var foundAudios = await api.Audio.SearchAsync(new AudioSearchParams
                { Query = $"{track.Author} {track.Title}" });
            if (foundAudios != null)
            {
                var audio = foundAudios.First();
                audiosIds.Add($"{audio.OwnerId}_{audio.Id}");
            }
        }

        audiosIds.Reverse();

        var createdPlaylist =
            await api.Audio.CreatePlaylistAsync(api.UserId!.Value, playlist.Title, audioIds: audiosIds);
        var audios = await api.Audio.GetAsync(new AudioGetParams
            { OwnerId = api.UserId, PlaylistId = createdPlaylist.Id });
        return (createdPlaylist, audios.ToArray());
    }

    public async Task<(AudioPlaylist, Audio[])> SmartUpdateAsync(
        string username,
        long playlistId,
        string[]? tracksToAdd,
        string[]? tracksToRemove)
    {
        var client = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(username);

        var audiosIds = new List<string>();
        foreach (var track in tracksToAdd!)
        {
            var foundAudios = await client.Audio.SearchAsync(new AudioSearchParams { Query = track });
            if (foundAudios != null)
            {
                var audio = foundAudios.First();
                audiosIds.Add($"{audio.OwnerId}_{audio.Id}");
            }
        }

        await client.Audio.AddToPlaylistAsync(client.UserId!.Value, playlistId, audiosIds);
        var playlist = await client.Audio.GetPlaylistByIdAsync(client.UserId!.Value, playlistId);
        var audios =
            await client.Audio.GetAsync(new AudioGetParams { OwnerId = client.UserId, PlaylistId = playlistId });
        return (playlist, audios.ToArray());
    }

    public async Task<(AudioPlaylist, Audio[])> UpdateAsync(string username, long playlistId, Playlist source)
    {
        var client = await vkNetClientsRepository.GetAuthenticatedVkNetApiAsync(username);
        var audioIds = await GetAudioIdsFromSourceAsync(client, source);
        await client.Audio.DeletePlaylistAsync(client.UserId!.Value, playlistId);
        var playlist = await client.Audio.CreatePlaylistAsync(client.UserId!.Value, source.Title, audioIds: audioIds);
        var audios = await client.Audio.GetAsync(new AudioGetParams
            { OwnerId = client.UserId, PlaylistId = playlist.Id });
        return (playlist, audios.ToArray());
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
        var audios = await client.Audio.GetAsync(new AudioGetParams
            { OwnerId = client.UserId, PlaylistId = playlist.Id });
        return (playlist, audios.ToArray());
    }

    private async Task<List<string>> GetAudioIdsFromSourceAsync(VkApi api, Playlist source)
    {
        var audiosIds = new List<string>();
        foreach (var track in source.Tracks!)
        {
            var foundAudios = await api.Audio.SearchAsync(new AudioSearchParams { Query = track.Title });
            if (foundAudios != null)
            {
                var audio = foundAudios.First();
                audiosIds.Add($"{audio.OwnerId}_{audio.Id}");
            }
        }

        return audiosIds;
    }
}