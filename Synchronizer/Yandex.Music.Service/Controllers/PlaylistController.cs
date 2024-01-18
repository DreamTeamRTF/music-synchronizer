using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using Yandex.Music.Service.Exceptions;
using Yandex.Music.Service.Models.Music;

namespace Yandex.Music.Service.Controllers;

[ApiController]
public class PlaylistsController : ControllerBase
{
    private readonly YandexMusicService yandexMusicService;
    private readonly ILogger<PlaylistsController> logger;

    public PlaylistsController(YandexMusicService yandexMusicService, ILogger<PlaylistsController> logger)
    {
        this.yandexMusicService = yandexMusicService;
        this.logger = logger;
    }

    [HttpGet]
    [Route("yandex/music/my/playlists")]
    public async Task<ActionResult<Playlist[]>> Get([FromQuery] OwnPlaylistsRequest ownTracksRequest)
    {
        try
        {
            var playlists = await yandexMusicService
                .GetOwnPlaylistsAsync(ownTracksRequest)
                .ConfigureAwait(false);
            logger.LogInformation("Found own playlists for user: {Username}, Count: {PlaylistsLength}", ownTracksRequest.Username, playlists.Length);
            return Ok(playlists);
        }
        catch (AuthApiException e)
        {
            return Unauthorized();
        }

    }
}