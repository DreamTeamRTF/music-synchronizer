using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using MusicServices.Models.Contracts;

namespace VK.Music.Service.Controllers;

[ApiController]
public class PlaylistsController : ControllerBase
{
    private readonly IMusicService vkMusicService;

    public PlaylistsController(IMusicService vkMusicService)
    {
        this.vkMusicService = vkMusicService;
    }

    [HttpGet]
    [Route("vk/music/my/playlists")]
    public async Task<ActionResult<Playlist[]>> Get([FromQuery] OwnTracksRequest ownTracksRequest)
    {
        var playlists = await vkMusicService.GetOwnPlaylistsAsync(ownTracksRequest).ConfigureAwait(false);
        return Ok(playlists);
    }
}