using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using Synchronizer.Core;
using Synchronizer.Core.DTO;
using Synchronizer.Core.Services;

namespace Synchronizer.Service.Controllers;

[ApiController]
public class SynchronizerController : ControllerBase
{
    private readonly ISynchronizerService synchronizerService;
    private readonly ILogger<SynchronizerController> logger;

    public SynchronizerController(ISynchronizerService synchronizerService, ILogger<SynchronizerController> logger)
    {
        this.synchronizerService = synchronizerService;
        this.logger = logger;
    }

    [HttpPost]
    [Route("/sync/playlist")]
    public async Task<ActionResult<Playlist>> SyncPlaylist(PlaylistToSyncDto playlistToSyncDto)
    {
        var syncResult = await synchronizerService.SyncPlaylistsAsync(
            playlistToSyncDto.Username,
            playlistToSyncDto.PlaylistId,
            playlistToSyncDto.MusicService);
        
        if (syncResult.IsSuccess)
        {
            logger.LogError("Sync IsSuccess {@Result}", syncResult.Value.Title);
            return syncResult.Value;
        }
        
        logger.LogError("Sync failed with error {SyncResultError}", syncResult.Error);
        return BadRequest(syncResult.Error);
    }
    
    [HttpGet]
    [Route("/sync/playlists")]
    public async Task<ActionResult<PlaylistWithServiceType[]>> SyncPlaylists([FromQuery] string username)
    {
        var syncResult = await synchronizerService.GetSynchronizedPlaylistsAsync(username);
        
        if (syncResult.IsSuccess)
        {
            logger.LogError("Sync IsSuccess for user {User}", username);
            return syncResult.Value;
        }
        
        logger.LogError("Sync failed with error {SyncResultError}", syncResult.Error);
        return BadRequest(syncResult.Error);
    }
}