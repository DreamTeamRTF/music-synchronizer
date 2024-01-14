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
    [Route("vk/music/own/playlists")]
    public async Task<ActionResult<Playlist[]>> Get([FromQuery] OwnPlaylistsRequest ownPlaylistsRequest)
    {
        var playlists = await vkMusicService.GetOwnPlaylistsAsync(ownPlaylistsRequest).ConfigureAwait(false);
        return Ok(playlists);
    }
}