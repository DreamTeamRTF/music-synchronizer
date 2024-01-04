using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using MusicServices.Models.Contracts;

namespace VK.Music.Service.Controllers;

[ApiController]
public class TracksController : ControllerBase
{
    private readonly IMusicService vkMusicService;

    public TracksController(IMusicService vkMusicService)
    {
        this.vkMusicService = vkMusicService;
    }

    [HttpGet]
    [Route("vk/music/tracks")]
    public async Task<ActionResult<Track[]>> GetTracksFromPlaylist([FromQuery] int playlistId)
    {
        return await vkMusicService
            .GetPlaylistTracks(new PlaylistRequest { PlaylistId = playlistId })
            .ConfigureAwait(false);
    }
}