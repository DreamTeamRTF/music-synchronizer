using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using MusicServices.Models.Contracts;
using Yandex.Music.Service.Models.Music;


namespace Yandex.Music.Service.Controllers
{
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly YandexMusicService _yandexMusicService;

        public PlaylistsController(YandexMusicService yandexMusicService)
        {
            _yandexMusicService = yandexMusicService;
        }

        [HttpGet]
        [Route("yandex/music/my/playlists")]
        public async Task<ActionResult<Playlist[]>> Get([FromQuery] OwnPlaylistsRequest ownTracksRequest)
        {
            var playlists = await _yandexMusicService
                .GetOwnPlaylistsAsync(ownTracksRequest)
                .ConfigureAwait(false);
            return Ok(playlists);
        }
    }
}
