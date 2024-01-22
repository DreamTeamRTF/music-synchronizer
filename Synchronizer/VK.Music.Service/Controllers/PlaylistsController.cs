using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using MusicServices.Models.Contracts;
using VK.Music.Service.Exceptions;

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
    public async Task<ActionResult<Playlist[]>> GetOwnPlaylists([FromQuery] OwnPlaylistsRequest ownPlaylistsRequest)
    {
        try
        {
            var playlists = await vkMusicService.GetOwnPlaylistsAsync(ownPlaylistsRequest).ConfigureAwait(false);
            return Ok(playlists);
        }
        catch (AuthApiException e)
        {
            return Unauthorized();
        }
    }

    [HttpPost]
    [Route("vk/music/add/playlist")]
    public async Task<ActionResult<Playlist>> AddPlaylist([FromBody] PlaylistToAddRequest playlistToAddRequest)
    {
        return await vkMusicService.AddPlaylistAsync(playlistToAddRequest);
    }

    [HttpGet]
    [Route("vk/music/playlist/findById")]
    public async Task<ActionResult<Playlist?>> AddPlaylist([FromQuery] FindPlaylistByIdRequest findPlaylistByIdRequest)
    {
        return await vkMusicService.FindPlaylistByIdAsync(findPlaylistByIdRequest);
    }

    [HttpPost]
    [Route("vk/music/playlist/smart-update")]
    public async Task<ActionResult<Playlist?>> SmartUpdatePlaylist(
        [FromBody] SmartPlaylistUpdateModel smartPlaylistUpdateModel)
    {
        return await vkMusicService.SmartPlaylistUpdateAsync(smartPlaylistUpdateModel);
    }

    [HttpPost]
    [Route("vk/music/playlist/update")]
    public async Task<ActionResult<Playlist?>> UpdatePlaylist([FromBody] PlaylistUpdateModel playlistUpdateModel)
    {
        return await vkMusicService.UpdatePlaylistAsync(playlistUpdateModel);
    }
}