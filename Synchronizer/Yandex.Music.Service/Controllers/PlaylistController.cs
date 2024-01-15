using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using Yandex.Music.Service.Models.Music;


namespace Yandex.Music.Service.Controllers
{
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly YandexMusicService yandexMusicService;

        public PlaylistsController(YandexMusicService yandexMusicService)
        {
            this.yandexMusicService = yandexMusicService;
        }

        [HttpGet]
        [Route("yandex/music/my/playlists")]
        public async Task<ActionResult<Playlist[]>> Get([FromQuery] OwnPlaylistsRequest ownTracksRequest)
        {
            var playlists = await yandexMusicService
                .GetOwnPlaylistsAsync(ownTracksRequest)
                .ConfigureAwait(false);
            return Ok(playlists);
        }
    }
}
