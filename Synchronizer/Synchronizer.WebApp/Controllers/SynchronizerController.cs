﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synchronizer.Core;
using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;
using Synchronizer.Models.Contracts.VK;
using Synchronizer.WebApp.Extensions;
using Synchronizer.WebApp.Helpers;
using Synchronizer.WebApp.Models.Synchronizer.Playlists;
using Synchronizer.WebApp.Services;

namespace Synchronizer.WebApp.Controllers;

[Authorize]
public class SynchronizerController : Controller
{
    private readonly ILogger<SynchronizerController> logger;
    private readonly VkMusicClient vkMusicClient;
    private readonly YandexMusicClient yandexMusicClient;
    private readonly SynchronizerClient synchronizerClient;

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

        return View(playlistsResult.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> SynchronizePlaylist(long playlistId, MusicServiceTypeModel musicService)
    {
        var playlistsResult = await synchronizerClient.SyncPlaylist(
            HttpContext.GetUsername(),
            playlistId,
            musicService.ToMusicServiceType());

        return RedirectToAction("Index", "Home");
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