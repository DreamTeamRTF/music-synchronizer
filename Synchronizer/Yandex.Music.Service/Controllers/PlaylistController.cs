using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using MusicServices.Models.Contracts;


namespace Yandex.Music.Service.Controllers
{
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly IMusicService _yandexMusicService;

        public PlaylistsController(IMusicService yandexMusicService)
        {
            _yandexMusicService = yandexMusicService;
        }

        [HttpGet]
        [Route("yandex/music/my/playlists")]
        public async Task<ActionResult<Playlist[]>> Get([FromQuery] OwnTracksRequest ownTracksRequest)
        {
            var playlists = await _yandexMusicService
                .GetOwnPlaylistsAsync(ownTracksRequest)
                .ConfigureAwait(false);
            return Ok(playlists);
        }
    }
}
