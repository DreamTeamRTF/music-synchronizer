using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using MusicServices.Models.Contracts;

namespace Yandex.Music.Service.Controllers;

[ApiController]
public class TracksController : ControllerBase
{
    private readonly IMusicService yandexMusicService;

    public TracksController(IMusicService yandexMusicService)
    {
        this.yandexMusicService = yandexMusicService;
    }

    [HttpGet]
    [Route("yandex/music/tracks")]
    public async Task<ActionResult<Track[]>> GetTracksFromPlaylist([FromQuery] PlaylistRequest playlistRequest)
    {
        return await yandexMusicService
            .GetPlaylistTracksAsync(playlistRequest)
            .ConfigureAwait(false);
    }
}