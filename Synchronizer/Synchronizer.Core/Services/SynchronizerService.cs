using Microsoft.Extensions.Logging;
using MusicServices.Models;
using Services.Infrastructure;
using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;
using Synchronizer.DAL.Entities;
using Synchronizer.DAL.Repositories;

namespace Synchronizer.Core.Services;

public class SynchronizerService : ISynchronizerService
{
    private readonly VkMusicClient vkMusicClient;
    private readonly YandexMusicClient yandexMusicClient;
    private readonly SynchronizedPlaylistsRepository synchronizedPlaylistsRepository;
    private readonly ILogger<SynchronizerService> logger;

    public SynchronizerService(
        VkMusicClient vkMusicClient,
        YandexMusicClient yandexMusicClient,
        SynchronizedPlaylistsRepository synchronizedPlaylistsRepository,
        ILogger<SynchronizerService> logger)
    {
        this.vkMusicClient = vkMusicClient;
        this.yandexMusicClient = yandexMusicClient;
        this.synchronizedPlaylistsRepository = synchronizedPlaylistsRepository;
        this.logger = logger;
    }
    
    public Task<Result<Playlist>> SyncPlaylistsAsync(string username, long playlistId, MusicServiceType mainServiceType)
    {
        return mainServiceType == MusicServiceType.Vk 
            ? SyncFromVkAsync(username, playlistId) 
            : SyncFromYandexAsync(username, playlistId);
    }

    public async Task<Result<PlaylistWithServiceType[]>> GetSynchronizedPlaylistsAsync(string username)
    {
        var links = synchronizedPlaylistsRepository.GetLinksForUser(username);
        logger.LogInformation("Founded links = {LinksLength}", links.Length);
        var playlists = new List<PlaylistWithServiceType>();
        foreach (var link in links)
        {
            var playlistResult = link.MainMusicService == MusicServiceType.Vk
                ? await vkMusicClient.GetPlaylistByIdAsync(username, link.VkPlaylistId)
                : await yandexMusicClient.GetPlaylistByIdAsync(username, link.YandexPlaylistId);
            if (playlistResult.IsSuccess)
            {
                playlists.Add(new PlaylistWithServiceType{Playlist = playlistResult.Value, ServiceType = link.MainMusicService});
                logger.LogInformation("Added playlist {ValueTitle}, {LinkMainMusicService}",
                    playlistResult.Value.Title, link.MainMusicService);
            }
        }
        
        return playlists.ToArray();
    }

    private async Task<Result<Playlist>> SyncFromVkAsync(string username, long playlistId)
    {
        var playlistResult = await vkMusicClient.GetPlaylistByIdAsync(username, playlistId);
        
        if (playlistResult.IsSuccess)
        {
            logger.LogInformation("Got playlist info from vk {ValueTitle}", playlistResult.Value.Title);
            var playlistAddResult = await yandexMusicClient.TryAddPlaylistAsync(username, playlistResult.Value);
            if (playlistAddResult.IsSuccess)
            {
                logger.LogInformation("Added playlist into yandex {ValueTitle}", playlistAddResult.Value.Title);
                try
                {
                    await synchronizedPlaylistsRepository.InsertAsync(new SynchronizedPlaylistLink
                    {
                        VkPlaylistId = playlistId,
                        YandexPlaylistId = playlistAddResult.Value.Id,
                        MainMusicService = MusicServiceType.Vk,
                        Username = username
                    });
                    logger.LogInformation(
                        "Inserted link with Username: {Username}, VkPlaylistId: {PlaylistId}, YandexPlaylistId {ValueId}",
                        username, playlistId, playlistAddResult.Value.Id);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }

            }

            return playlistAddResult;
        }

        return Result.Fail<Playlist>($"Failed to sync playlist for {username}, id: {playlistId}, {playlistResult.Error}");
    }
    
    private async Task<Result<Playlist>> SyncFromYandexAsync(string username, long playlistId)
    {
        var playlistResult = await yandexMusicClient.GetPlaylistByIdAsync(username, playlistId);
        if (playlistResult.IsSuccess)
        {
            var playlistAddResult = await vkMusicClient.TryAddPlaylistAsync(username, playlistResult.Value);
            if (playlistAddResult.IsSuccess)
            {
                await synchronizedPlaylistsRepository.InsertAsync(new SynchronizedPlaylistLink
                    { 
                        YandexPlaylistId = playlistId,
                        VkPlaylistId = playlistAddResult.Value.Id,
                        MainMusicService = MusicServiceType.Yandex,
                        Username = username
                        
                    });
                logger.LogInformation(
                    "Inserted link with Username: {Username}, VkPlaylistId: {PlaylistId}, YandexPlaylistId {ValueId}",
                    username, playlistAddResult.Value.Id, playlistId);
            }

            return playlistAddResult;
        }

        return Result.Fail<Playlist>($"Failed to sync playlist for {username}, id: {playlistId}");
    }

    /*
    public Task UpdateLinkedPlaylistAsync(long playlistId, MusicServiceType baseServiceType)
    {
        
    }*/
}