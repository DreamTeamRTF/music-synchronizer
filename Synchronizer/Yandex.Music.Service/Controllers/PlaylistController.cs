using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using MusicServices.Models.Contracts;
using Yandex.Music.Service.Exceptions;

namespace Yandex.Music.Service.Controllers;

[ApiController]
public class PlaylistsController : ControllerBase
{
    private readonly ILogger<PlaylistsController> logger;
    private readonly IMusicService yandexMusicService;

    public PlaylistsController(IMusicService yandexMusicService, ILogger<PlaylistsController> logger)
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
            logger.LogInformation("Found own playlists for user: {Username}, Count: {PlaylistsLength}",
                ownTracksRequest.Username, playlists.Length);
            return Ok(playlists);
        }
        catch (AuthApiException e)
        {
            return Unauthorized();
        }
    }

    [HttpPost]
    [Route("yandex/music/add/playlist")]
    public async Task<ActionResult<Playlist>> AddPlaylist([FromBody] PlaylistToAddRequest playlistToAddRequest)
    {
        return await yandexMusicService.AddPlaylistAsync(playlistToAddRequest);
    }

    [HttpGet]
    [Route("yandex/music/playlist/findById")]
    public async Task<ActionResult<Playlist?>> FindPlaylistById([FromQuery] FindPlaylistByIdRequest request)
    {
        return await yandexMusicService.FindPlaylistByIdAsync(request);
    }

    [HttpPost]
    [Route("yandex/music/playlist/smart-update")]
    public async Task<ActionResult<Playlist?>> SmartUpdatePlaylist(
        [FromBody] SmartPlaylistUpdateModel smartPlaylistUpdateModel)
    {
        return await yandexMusicService.SmartPlaylistUpdateAsync(smartPlaylistUpdateModel);
    }

    [HttpPost]
    [Route("yandex/music/playlist/update")]
    public async Task<ActionResult<Playlist>> UpdatePlaylist([FromBody] PlaylistUpdateModel playlistUpdateModel)
    {
        return await yandexMusicService.UpdatePlaylistAsync(playlistUpdateModel);
    }
}