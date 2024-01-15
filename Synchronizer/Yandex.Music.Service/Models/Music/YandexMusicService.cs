using MusicServices.Models;
using Yandex.Music.Api.Models.Track;
using Yandex.Music.Service.Helpers;

namespace Yandex.Music.Service.Models.Music
{
    public class YandexMusicService
    {
        private readonly YandexApiClient apiClient;

        public YandexMusicService(YandexApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<Track[]> GetPlaylistTracksAsync(PlaylistRequest request)
        {
            var yTracks = await apiClient.GetTracksFromPlaylistAsync(request.Username, request.PlaylistId).ConfigureAwait(false);
            return yTracks.Select(t => t.FromYandexModel()).ToArray();
        }

        public async Task<Playlist[]> GetOwnPlaylistsAsync(OwnPlaylistsRequest request)
        {
            var playlists = await apiClient.GetOwnPlaylistsAsync(request.Username);
            return playlists.Select(p => p.FromYandexModel()).ToArray();
        }
    }
}
