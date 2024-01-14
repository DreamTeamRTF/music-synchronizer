using MusicServices.Models;
using Yandex.Music.Api.Models.Track;
using Yandex.Music.Service.Helpers;

namespace Yandex.Music.Service.Models
{
    public class YandexMusicService
    {
        private readonly YandexApiClient _apiClient;

        public YandexMusicService(YandexApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Track[]> GetPlaylistTracksAsync(PlaylistRequest request)
        {
            var yTracks = await _apiClient.GetTracksFromPlaylistAsync(request.Login, request.PlaylistId).ConfigureAwait(false);
            return yTracks.Select(t => t.FromYandexModel()).ToArray();
        }

        public async Task<Playlist[]> GetOwnPlaylistAsync(OwnTracksRequest request)
        {
            var playlist = await _apiClient.GetOwnPlaylistsAsync(request.Login);
            return playlist.Select(p => p.FromYandexModel()).ToArray();
        }
    }
}
