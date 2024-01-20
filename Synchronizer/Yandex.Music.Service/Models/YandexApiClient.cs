using MusicServices.Models;
using MusicServices.Models.Exceptions;
using Yandex.Music.Api.Models.Common;
using Yandex.Music.Api.Models.Playlist;
using Yandex.Music.Api.Models.Track;
using Yandex.Music.Client.Extensions;

namespace Yandex.Music.Service.Models;

public class YandexApiClient
{
    private readonly YandexClientsRepository clientRepository;

    public YandexApiClient(YandexClientsRepository clientRepository)
    {
        this.clientRepository = clientRepository;
    }

    public async Task<YTrackContainer[]> GetTracksFromPlaylistAsync(string username, int playlistId)
    {
        var api = await clientRepository.GetAuthenticatedYandexApi(username);
        var loginInfo = await api.GetLoginInfo().ConfigureAwait(false);
        var playlist = await api.GetPlaylist(loginInfo.Id, playlistId.ToString()).ConfigureAwait(false);
        var tracks = playlist.Tracks;

        if (tracks is null) throw new TrackNotFoundException();

        return tracks.ToArray();
    }

    public async Task<YPlaylist[]> GetOwnPlaylistsAsync(string username)
    {
        var api = await clientRepository.GetAuthenticatedYandexApi(username);
        var loginInfo = await api.GetLoginInfo().ConfigureAwait(false);
        var fav = await api.GetFavorites();
        var ownPlaylistsShells = fav.Where(p => p.Owner.Login == loginInfo.Login);
        var ownPlaylists = new List<YPlaylist>();

        foreach (var p in ownPlaylistsShells)
        {
            ownPlaylists.Add(await api.GetPlaylist(loginInfo.Id, p.Kind));
        }

        if (ownPlaylists is null) throw new TrackNotFoundException();
        return ownPlaylists.ToArray();
    }

    public async Task<AccountInfoModel> GetAccountInfoAsync(string username)
    {
        var client = await clientRepository.GetAuthenticatedYandexApi(username);
        var loginInfo = await client.GetLoginInfo();

        var photoUrl = loginInfo.IsAvatarEmpty ? "" : loginInfo.AvatarUrl;

        return new AccountInfoModel
        {
            Name = client.Account.FullName,
            ImageUrl = photoUrl
        };
    }
    
    public async Task<YPlaylist> SavePlaylistAsync(string username, Playlist playlist)
    {
        var api = await clientRepository.GetAuthenticatedYandexApi(username);
        var foundedTracks = new List<YTrack>();
        foreach (var track in playlist.Tracks!)
        {
            var searchResult = await api.Search($"{track.Title} {track.Author}", YSearchType.Track);
            var foundedTrack = searchResult?.Tracks?.Results?.First();
            if (foundedTrack != null)
            {
                foundedTracks.Add(foundedTrack);
            }
        }

        foundedTracks.Reverse();
        var createdPlaylist = await api.CreatePlaylist(playlist.Title);
        return await createdPlaylist.InsertTracksAsync(foundedTracks.ToArray());
    }
    
    public async Task<YPlaylist?> FindByIdAsync(string username, long playlistId)
    {
        var api = await clientRepository.GetAuthenticatedYandexApi(username);
        var fav = await (await api.GetFavorites()).FirstOrDefault(x => x.Kind  == playlistId.ToString()).WithTracksAsync();
        return fav;
    }
}