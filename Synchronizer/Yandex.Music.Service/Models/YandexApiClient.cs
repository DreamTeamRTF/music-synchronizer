using MusicServices.Models;
using MusicServices.Models.Exceptions;
using Yandex.Music.Api.Models.Common;
using Yandex.Music.Api.Models.Playlist;
using Yandex.Music.Api.Models.Track;
using Yandex.Music.Client;
using Yandex.Music.Client.Extensions;

namespace Yandex.Music.Service.Models;

public class YandexApiClient
{
    private readonly YandexClientsRepository clientRepository;
    private readonly ILogger<YandexApiClient> logger;

    public YandexApiClient(YandexClientsRepository clientRepository, ILogger<YandexApiClient> logger)
    {
        this.clientRepository = clientRepository;
        this.logger = logger;
    }

    public async Task<YTrackContainer[]> GetTracksFromPlaylistAsync(string username, int playlistId)
    {
        var api = await clientRepository.GetAuthenticatedYandexApiAsync(username);
        var loginInfo = await api.GetLoginInfo().ConfigureAwait(false);
        var playlist = await api.GetPlaylist(loginInfo.Id, playlistId.ToString()).ConfigureAwait(false);
        var tracks = playlist.Tracks;

        if (tracks is null) throw new TrackNotFoundException();

        return tracks.ToArray();
    }

    public async Task<YPlaylist[]> GetOwnPlaylistsAsync(string username)
    {
        var api = await clientRepository.GetAuthenticatedYandexApiAsync(username);
        var loginInfo = await api.GetLoginInfo().ConfigureAwait(false);
        var fav = await api.GetFavorites();
        var ownPlaylistsShells = fav.Where(p => p.Owner.Login == loginInfo.Login);
        var ownPlaylists = new List<YPlaylist>();

        foreach (var p in ownPlaylistsShells) ownPlaylists.Add(await api.GetPlaylist(loginInfo.Id, p.Kind));

        if (ownPlaylists is null) throw new TrackNotFoundException();
        return ownPlaylists.ToArray();
    }

    public async Task<AccountInfoModel> GetAccountInfoAsync(string username)
    {
        var client = await clientRepository.GetAuthenticatedYandexApiAsync(username);
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
        var api = await clientRepository.GetAuthenticatedYandexApiAsync(username);
        var foundedTracks = await GetTracksToSaveAsync(api, playlist);
        var createdPlaylist = await api.CreatePlaylist(playlist.Title);
        createdPlaylist.Image = playlist.ImageUrl ?? "";
        return await createdPlaylist.InsertTracksAsync(foundedTracks.ToArray());
    }

    public async Task<YPlaylist?> FindByIdAsync(string username, long playlistId)
    {
        var api = await clientRepository.GetAuthenticatedYandexApiAsync(username);
        var favourites = await api.GetFavorites();
        logger.LogInformation("Found favorites {FavCount}, playlistId: {PlaylistId}", favourites?.Count, playlistId);
        foreach (var yPlaylist in favourites!) logger.LogInformation("found fav : {YPlaylistKind}", yPlaylist.Kind);
        var fav = await favourites.FirstOrDefault(x => x.Kind == playlistId.ToString()).WithTracksAsync();
        return fav;
    }

    public async Task<YPlaylist> SmartUpdatePlaylistAsync(string username, long playlistId, string[]? tracksToAdd,
        string[]? tracksToRemove)
    {
        var api = await clientRepository.GetAuthenticatedYandexApiAsync(username);
        var fav = await api.GetFavorites();
        logger.LogInformation("Found favorites {FavCount}, playlistId: {PlaylistId}", fav?.Count, playlistId);
        foreach (var yPlaylist in fav!) logger.LogInformation("found fav : {YPlaylistKind}", yPlaylist.Kind);
        var playlist = await fav.FirstOrDefault(x => x.Kind == playlistId.ToString())?
            .WithTracksAsync()!;
        YPlaylist updatedPlaylist;
        if (tracksToRemove != null && tracksToRemove.Length != 0)
        {
            var removeTracks = playlist.Tracks
                .Where(x => tracksToRemove.Any(t => x.Track.Title == t))
                .Select(x => x.Track)
                .ToArray();
            updatedPlaylist = await playlist.RemoveTracksAsync(removeTracks);
        }
        else
        {
            updatedPlaylist = playlist;
        }

        var foundedTracks = new List<YTrack>();
        foreach (var track in tracksToAdd!)
        {
            var searchResult = await api.Search(track, YSearchType.Track);
            var foundedTrack = searchResult?.Tracks?.Results?.First();
            if (foundedTrack != null) foundedTracks.Add(foundedTrack);
        }

        foreach (var track in foundedTracks) logger.LogInformation("found track to add {TrackTitle}", track.Title);

        var insertedPlaylist = await updatedPlaylist.InsertTracksAsync(foundedTracks.ToArray());
        foreach (var track in insertedPlaylist.Tracks)
            logger.LogInformation("INSERTED TRACKS CONTAINS: {TrackTitle}", track.Track.Title);
        return insertedPlaylist;
    }

    public async Task<YPlaylist> PlaylistUpdateAsync(string username, long playlistId, Playlist source)
    {
        var api = await clientRepository.GetAuthenticatedYandexApiAsync(username);
        var playlist = await (await api.GetFavorites()).First(x => x.Kind == playlistId.ToString()).WithTracksAsync();
        var tracksToRemove = playlist.Tracks.Select(t => t.Track).ToArray();
        var clearedPlaylist = await playlist.RemoveTracksAsync(tracksToRemove);
        var tracksToSave = await GetTracksToSaveAsync(api, source);
        clearedPlaylist.Image = source.ImageUrl ?? "";
        return await clearedPlaylist.InsertTracksAsync(tracksToSave.ToArray());
    }

    private async Task<List<YTrack>> GetTracksToSaveAsync(YandexMusicClientAsync musicClient, Playlist source)
    {
        var foundedTracks = new List<YTrack>();
        foreach (var track in source.Tracks!)
        {
            var searchResult = await musicClient.Search($"{track.Title} {track.Author}", YSearchType.Track);
            var foundedTrack = searchResult?.Tracks?.Results?.First();
            if (foundedTrack != null) foundedTracks.Add(foundedTrack);
        }

        foundedTracks.Reverse();
        return foundedTracks;
    }
}