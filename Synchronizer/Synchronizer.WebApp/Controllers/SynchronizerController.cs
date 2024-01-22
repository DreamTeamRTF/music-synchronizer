using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synchronizer.Core;
using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;
using Synchronizer.WebApp.Extensions;
using Synchronizer.WebApp.Helpers;
using Synchronizer.WebApp.Models;
using Synchronizer.WebApp.Models.Synchronizer.Playlists;
using Synchronizer.WebApp.Services;

namespace Synchronizer.WebApp.Controllers;

[Authorize]
public class SynchronizerController : Controller
{
    private readonly ILogger<SynchronizerController> logger;
    private readonly SynchronizerClient synchronizerClient;
    private readonly VkMusicClient vkMusicClient;
    private readonly YandexMusicClient yandexMusicClient;

    public SynchronizerController(
        VkMusicClient vkMusicClient,
        YandexMusicClient yandexMusicClient,
        ILogger<SynchronizerController> logger,
        SynchronizerClient synchronizerClient)
    {
        this.vkMusicClient = vkMusicClient;
        this.yandexMusicClient = yandexMusicClient;
        this.logger = logger;
        this.synchronizerClient = synchronizerClient;
    }

    [HttpGet] // TODO: Заглушка нужно получать из апишек
    public async Task<IActionResult> SynchronizedPlaylists()
    {
        var playlistsResult = await synchronizerClient.GetSynchronizedPlaylists(HttpContext.GetUsername());
        if (playlistsResult.IsSuccess)
        {
            logger.LogInformation("Sync Playlists Found");
            var model = new SynchronizedPlaylistsModel
            {
                Playlists = playlistsResult.Value
                    .Select(x => x.FromServiceModel())
                    .ToArray()
            };

            return View(model);
        }

        var synchronizedPlaylistsModel = new SynchronizedPlaylistsModel
        {
            Playlists = new[]
            {
                new SynchronizedPlaylistModel
                {
                    Title = "Aboba", ServiceType = MusicServiceTypeModel.YandexMusic,
                    CoverImage =
                        "https://avatars.mds.yandex.net/get-yapic/47747/637Scc7TQQW2BmFFiC1ZsnRA3E-1/islands-200"
                },
                new SynchronizedPlaylistModel
                {
                    Title = "VkPlaylist", ServiceType = MusicServiceTypeModel.VkMusic,
                    CoverImage =
                        "https://avatars.mds.yandex.net/get-yapic/47747/637Scc7TQQW2BmFFiC1ZsnRA3E-1/islands-200"
                },
                new SynchronizedPlaylistModel
                {
                    Title = "YandexPlaylist", ServiceType = MusicServiceTypeModel.YandexMusic,
                    CoverImage =
                        "https://avatars.mds.yandex.net/get-yapic/47747/637Scc7TQQW2BmFFiC1ZsnRA3E-1/islands-200"
                }
            }
        };
        return View(synchronizedPlaylistsModel);
    }

    [HttpGet]
    public async Task<IActionResult> OwnPlaylists(MusicServiceTypeModel musicService)
    {
        IMusicClient client = musicService == MusicServiceTypeModel.VkMusic ? vkMusicClient : yandexMusicClient;
        logger.LogInformation("Using service: {MusicService}", musicService);
        var username = Request.HttpContext.GetUsername();
        var playlistsResult = await client.GetUsersOwnPlaylistsAsync(username);
        if (!playlistsResult.IsSuccess)
        {
            var form = musicService == MusicServiceTypeModel.VkMusic
                ? "VkAccountForm"
                : "YandexAccountForm";
            return RedirectToAction(form, "LinkedAccounts");
        }

        var syncPlaylists = await synchronizerClient.GetSynchronizedPlaylists(username);
        var playlistWithSyncFlag = playlistsResult.Value.Select(x => new OwnPlaylistsViewModel
        {
            Playlist = x,
            IsSynchronized = syncPlaylists.Value.Any(p =>
                p.Playlist.Id == x.Id && p.ServiceType.ToMusicServiceType() == musicService)
        });

        return View((playlistWithSyncFlag.ToArray(), musicService));
    }

    [HttpPost] //Todo: сделать страницу с синхронизованным плейлистом
    public async Task<IActionResult> SynchronizePlaylist(long playlistId, MusicServiceTypeModel musicService)
    {
        var playlistsResult = await synchronizerClient.SyncPlaylist(
            HttpContext.GetUsername(),
            playlistId,
            musicService.ToMusicServiceType());

        return RedirectToAction("SynchronizedPlaylists");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSynchronizedPlaylist(long playlistId, MusicServiceTypeModel musicService)
    {
        logger.LogInformation("starting update for playlist: {PlaylistId}, service {MusicService}", playlistId,
            musicService);
        var playlistsResult = await synchronizerClient.UpdatePlaylist(
            HttpContext.GetUsername(),
            playlistId,
            musicService.ToMusicServiceType());

        return RedirectToAction("SynchronizedPlaylists");
    }
}