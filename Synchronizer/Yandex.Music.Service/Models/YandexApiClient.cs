using Yandex.Music.Api.Models.Playlist;
using Yandex.Music.Api.Models.Track;
using MusicServices.Models.Exceptions;

namespace Yandex.Music.Service.Models
{
    public class YandexApiClient
    {
        private readonly YandexClientsRepository _clientRepository;

        public YandexApiClient(YandexClientsRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public async Task<YTrackContainer[]> GetTracksFromPlaylistAsync(string login, int playlistId)
        {
            var api = await _clientRepository.GetAuthentificatedYandexApi(login);
            var loginInfo = await api.GetLoginInfo();
            var playlist = await api.GetPlaylist(loginInfo.Id, playlistId.ToString());
            var tracks = playlist.Tracks;

            if (tracks is null) throw new TrackNotFoundException();

            return tracks.ToArray();
        }

        public async Task<YPlaylist[]> GetOwnPlaylistsAsync(string login)
        {
            var api = await _clientRepository.GetAuthentificatedYandexApi(login);
            var ownPlaylists = await api.GetFavorites();

            if (ownPlaylists is null) throw new TrackNotFoundException();
            return ownPlaylists.ToArray();
        }
    }
}
