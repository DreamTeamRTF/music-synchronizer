using Yandex.Music.Api.Models.Playlist;
using Yandex.Music.Api.Models.Track;
using MusicServices.Models.Exceptions;
using MusicServices.Models;

namespace Yandex.Music.Service.Models
{
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
            var api = await clientRepository.GetAuthenticatedYandexApi(username)
                .ConfigureAwait(false);
            var loginInfo = await api.GetLoginInfo().ConfigureAwait(false);
            var fav = await api.GetFavorites()
                .ConfigureAwait(false);
            var ownPlaylistsShells = fav.Where(p => p.Owner.Login == username);
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
    }
}
