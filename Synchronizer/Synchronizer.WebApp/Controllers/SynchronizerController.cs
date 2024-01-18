using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synchronizer.Core;
using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;
using Synchronizer.WebApp.Extensions;
using Synchronizer.WebApp.Models.Synchronizer.Playlists;

namespace Synchronizer.WebApp.Controllers;

[Authorize]
public class SynchronizerController : Controller
{
    private readonly ILogger<SynchronizerController> logger;
    private readonly VkMusicClient vkMusicClient;
    private readonly YandexMusicClient yandexMusicClient;

    public SynchronizerController(VkMusicClient vkMusicClient, YandexMusicClient yandexMusicClient,
        ILogger<SynchronizerController> logger)
    {
        this.vkMusicClient = vkMusicClient;
        this.yandexMusicClient = yandexMusicClient;
        this.logger = logger;
    }

    [HttpGet] // TODO: Заглушка нужно получать из апишек
    public IActionResult SynchronizedPlaylists()
    {
        var vkAccount = vkMusicClient;
        var yandexAccount = yandexMusicClient;
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

        return View(playlistsResult.Value);
    }

    /*[HttpPost]
    public async Task<IActionResult> SyncPlaylist(MusicServiceTypeModel musicService)
    {
        IMusicClient client = musicService == MusicServiceTypeModel.VkMusic ? yandexMusicClient : vkMusicClient;
        logger.LogInformation("Using service: {MusicService}", musicService);
        var username = Request.HttpContext.GetUsername();
        var playlists = await client.GetUsersOwnPlaylistsAsync(username);

        return View(playlists);
    }*/
}