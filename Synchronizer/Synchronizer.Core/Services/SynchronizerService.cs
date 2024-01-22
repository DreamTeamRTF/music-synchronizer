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
    private readonly ILogger<SynchronizerService> logger;
    private readonly SynchronizedPlaylistsRepository synchronizedPlaylistsRepository;
    private readonly VkMusicClient vkMusicClient;
    private readonly YandexMusicClient yandexMusicClient;

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
        logger.LogInformation("Got sync request PlaylistId: {PlaylistId}, service: {MainServiceType}", playlistId,
            mainServiceType);
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
                playlists.Add(new PlaylistWithServiceType
                    { Playlist = playlistResult.Value, ServiceType = link.MainMusicService });
                logger.LogInformation("Added playlist {ValueTitle}, {LinkMainMusicService}",
                    playlistResult.Value.Title, link.MainMusicService);
            }
        }

        return playlists.ToArray();
    }

    public Task<Result<Playlist>> SmartUpdateSyncPlaylistAsync(string username, long id, MusicServiceType serviceType)
    {
        return serviceType == MusicServiceType.Vk
            ? SmartUpdateFromVkAsync(username, id)
            : SmartUpdateFromYandex(username, id);
    }

    public Task<Result<Playlist>> DefaultUpdateSyncPlaylistAsync(string username, long id, MusicServiceType serviceType)
    {
        return serviceType == MusicServiceType.Vk
            ? UpdateFromVkAsync(username, id)
            : UpdateFromYandexAsync(username, id);
    }

    private async Task<Result<Playlist>> UpdateFromYandexAsync(string username, long yandexPlaylistId)
    {
        var yandexFindResult = await yandexMusicClient.GetPlaylistByIdAsync(username, yandexPlaylistId);
        var link = synchronizedPlaylistsRepository
            .GetLinksForUser(username)
            .FirstOrDefault(
                x => x.YandexPlaylistId == yandexPlaylistId && x.MainMusicService == MusicServiceType.Yandex);
        if (yandexFindResult.IsSuccess)
        {
            logger.LogInformation("Got playlist info from vk {ValueTitle}", yandexFindResult.Value.Title);
            var updateModel = new PlaylistUpdateModel(username, link!.VkPlaylistId, yandexFindResult.Value);
            var vkUpdateResult = await vkMusicClient.UpdatePlaylistAsync(updateModel);
            if (vkUpdateResult.IsSuccess)
            {
                logger.LogInformation("Updated playlist yandex {ValueTitle}", vkUpdateResult.Value.Title);
                try
                {
                    link.VkPlaylistId = vkUpdateResult.Value.Id;
                    link.Tracks.Clear();
                    if (vkUpdateResult.Value.Tracks != null)
                    {
                        link.Tracks.AddRange(
                            vkUpdateResult.Value.Tracks.Select(x => new SyncTrack { Title = x.Title }));
                        synchronizedPlaylistsRepository.Update(link);
                        logger.LogInformation(
                            "Inserted link with Username: {Username}, YandexId: {PlaylistId}, VkId {ValueId}",
                            username, yandexPlaylistId, vkUpdateResult.Value.Id);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }
            }

            return vkUpdateResult;
        }

        return Result.Fail<Playlist>(
            $"Failed to sync playlist for {username}, id: {yandexPlaylistId}, {yandexFindResult.Error}");
    }

    private async Task<Result<Playlist>> UpdateFromVkAsync(string username, long vkPlaylistId)
    {
        var vkFoundResult = await vkMusicClient.GetPlaylistByIdAsync(username, vkPlaylistId);
        var link = synchronizedPlaylistsRepository
            .GetLinksForUser(username)
            .FirstOrDefault(x => x.VkPlaylistId == vkPlaylistId && x.MainMusicService == MusicServiceType.Vk);
        if (vkFoundResult.IsSuccess)
        {
            logger.LogInformation("Got playlist info from vk {ValueTitle}", vkFoundResult.Value.Title);
            var updateModel = new PlaylistUpdateModel(username, link!.YandexPlaylistId, vkFoundResult.Value);
            var yandexPlaylistUpdateResult = await yandexMusicClient.UpdatePlaylistAsync(updateModel);
            if (yandexPlaylistUpdateResult.IsSuccess)
            {
                logger.LogInformation("Updated playlist yandex {ValueTitle}", yandexPlaylistUpdateResult.Value.Title);
                try
                {
                    link.Tracks.Clear();
                    if (yandexPlaylistUpdateResult.Value.Tracks != null)
                    {
                        link.Tracks.AddRange(
                            yandexPlaylistUpdateResult.Value.Tracks.Select(x => new SyncTrack { Title = x.Title }));
                        synchronizedPlaylistsRepository.Update(link);
                        logger.LogInformation(
                            "Inserted link with Username: {Username}, VkPlaylistId: {PlaylistId}, YandexPlaylistId {ValueId}",
                            username, vkPlaylistId, yandexPlaylistUpdateResult.Value.Id);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }
            }

            return yandexPlaylistUpdateResult;
        }

        return Result.Fail<Playlist>(
            $"Failed to sync playlist for {username}, id: {vkPlaylistId}, {vkFoundResult.Error}");
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
                    var model = new SynchronizedPlaylistLink
                    {
                        VkPlaylistId = playlistId,
                        YandexPlaylistId = playlistAddResult.Value.Id,
                        MainMusicService = MusicServiceType.Vk,
                        Username = username
                    };
                    var syncTracks = playlistResult.Value
                        .Tracks?.Select(x => new SyncTrack { Title = x.Title }) ?? Array.Empty<SyncTrack>();
                    await synchronizedPlaylistsRepository.InsertAsync(model, syncTracks);

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

        return Result.Fail<Playlist>(
            $"Failed to sync playlist for {username}, id: {playlistId}, {playlistResult.Error}");
    }

    private async Task<Result<Playlist>> SmartUpdateFromVkAsync(string username, long playlistId)
    {
        var link = synchronizedPlaylistsRepository
            .GetLinksForUser(username)
            .FirstOrDefault(x => x.VkPlaylistId == playlistId && x.MainMusicService == MusicServiceType.Vk);
        if (link == null) return Result.Fail<Playlist>("Link for playlist not found");
        var vkPlaylistResult = await vkMusicClient.GetPlaylistByIdAsync(username, playlistId);

        if (vkPlaylistResult.IsSuccess)
        {
            logger.LogInformation("Got playlist info from vk {ValueTitle}", vkPlaylistResult.Value.Title);
            var updateTracks = CalculateUpdatedTracks(link.Tracks.ToArray(),
                vkPlaylistResult.Value.Tracks ?? Array.Empty<Track>());
            logger.LogInformation("Tracks to add count {Length}", updateTracks.TracksToAdd.Length);
            logger.LogInformation("Tracks to remove count {Length}", updateTracks.TracksToRemove.Length);
            if (updateTracks.TracksToAdd.Length == 0 && updateTracks.TracksToRemove.Length == 0)
                return Result.Ok(vkPlaylistResult.Value);
            var model = new SmartPlaylistUpdateModel(username, link.YandexPlaylistId, updateTracks.TracksToAdd,
                updateTracks.TracksToRemove);
            var playlistUpdateResult = await yandexMusicClient.SmartUpdatePlaylistAsync(model);
            if (playlistUpdateResult.IsSuccess)
            {
                logger.LogInformation("Added playlist into yandex {ValueTitle}", playlistUpdateResult.Value.Title);
                try
                {
                    foreach (var track in playlistUpdateResult.Value.Tracks!)
                        logger.LogInformation("Tracks from playlist to ADD TO DB {@LinkTrack}", track.Title);
                    link.Tracks = playlistUpdateResult.Value.Tracks?
                        .Select(x => new SyncTrack { Title = x.Title, LinkId = link.Id })
                        .ToList()!;
                    foreach (var linkTrack in link.Tracks)
                        logger.LogInformation("Sync tracks to ADD TO DB {@LinkTrack}", linkTrack);
                    synchronizedPlaylistsRepository.Update(link);
                    logger.LogInformation(
                        "Updated link with Username: {Username}, VkPlaylistId: {PlaylistId}, YandexPlaylistId {ValueId}",
                        username, playlistId, playlistUpdateResult.Value.Id);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }
            }

            return playlistUpdateResult;
        }

        return Result.Fail<Playlist>(
            $"Failed to sync playlist for {username}, id: {playlistId}, {vkPlaylistResult.Error}");
    }

    private async Task<Result<Playlist>> SmartUpdateFromYandex(string username, long playlistId)
    {
        var link = synchronizedPlaylistsRepository
            .GetLinksForUser(username)
            .FirstOrDefault(x => x.YandexPlaylistId == playlistId && x.MainMusicService == MusicServiceType.Yandex);

        if (link == null) return Result.Fail<Playlist>("Link for playlist not found");
        foreach (var syncTrack in link.Tracks) logger.LogInformation("Links: {ValueTitle}", syncTrack.Title);
        var yandexResult = await yandexMusicClient.GetPlaylistByIdAsync(username, playlistId);

        if (yandexResult.IsSuccess)
        {
            logger.LogInformation("Got playlist info from yandex {ValueTitle}, Count: {Count}",
                yandexResult.Value.Title, yandexResult.Value.Tracks?.Length);
            var updateTracks = CalculateUpdatedTracks(link.Tracks.ToArray(),
                yandexResult.Value.Tracks ?? Array.Empty<Track>());
            if (updateTracks.TracksToAdd.Length == 0 && updateTracks.TracksToRemove.Length == 0)
                return Result.Ok(yandexResult.Value);
            var model = new SmartPlaylistUpdateModel(username, link.VkPlaylistId, updateTracks.TracksToAdd,
                updateTracks.TracksToRemove);
            var playlistUpdateResult = await vkMusicClient.SmartUpdatePlaylistAsync(model);
            if (playlistUpdateResult.IsSuccess)
            {
                logger.LogInformation("Added playlist into yandex {ValueTitle}", playlistUpdateResult.Value.Title);
                try
                {
                    foreach (var track in playlistUpdateResult.Value.Tracks!)
                        logger.LogInformation("Tracks from playlist to ADD TO DB {@LinkTrack}", track);
                    link.Tracks = playlistUpdateResult.Value.Tracks?
                        .Select(x => new SyncTrack { Title = x.Title })
                        .ToList()!;
                    foreach (var linkTrack in link.Tracks)
                        logger.LogInformation("Sync tracks to ADD TO DB {@LinkTrack}", linkTrack);
                    synchronizedPlaylistsRepository.Update(link);
                    logger.LogInformation(
                        "Updated link with Username: {Username}, VkPlaylistId: {PlaylistId}, YandexPlaylistId {ValueId}",
                        username, playlistId, playlistUpdateResult.Value.Id);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }
            }

            return playlistUpdateResult;
        }

        return Result.Fail<Playlist>($"Failed to sync playlist for {username}, id: {playlistId}, {yandexResult.Error}");
    }

    private (string[] TracksToAdd, string[] TracksToRemove) CalculateUpdatedTracks(
        SyncTrack[] syncTracks,
        Track[] tracksFromApi)
    {
        var tracksToAdd = new List<string>();
        var tracksToRemove = new List<SyncTrack>();

        foreach (var track in tracksFromApi)
        {
            logger.LogInformation("STARTING trackToAdd: {TrackTitle},", track.Title);
            if (syncTracks.All(x => x.Title != track.Title))
            {
                logger.LogInformation("ADDED trackToAdd: {TrackTitle},", track.Title);
                tracksToAdd.Add($"{track.Title} {track.Author}");
            }
        }

        foreach (var syncTrack in syncTracks)
        {
            logger.LogInformation("STARTING track to REMOVE: {SyncTrackTitle},", syncTrack.Title);
            if (tracksFromApi.All(x => x.Title != syncTrack.Title))
            {
                logger.LogInformation("ADDED track to REMOVE: {SyncTrackTitle},", syncTrack.Title);
                tracksToRemove.Add(syncTrack);
            }
        }

        synchronizedPlaylistsRepository.RemoveSyncTracks(tracksToRemove.ToArray());

        return (tracksToAdd.ToArray(), tracksToRemove.Select(x => x.Title).ToArray());
    }

    private async Task<Result<Playlist>> SyncFromYandexAsync(string username, long playlistId)
    {
        var playlistResult = await yandexMusicClient.GetPlaylistByIdAsync(username, playlistId);
        if (playlistResult.IsSuccess)
        {
            var playlistAddResult = await vkMusicClient.TryAddPlaylistAsync(username, playlistResult.Value);
            if (playlistAddResult.IsSuccess)
            {
                var model = new SynchronizedPlaylistLink
                {
                    VkPlaylistId = playlistAddResult.Value.Id,
                    YandexPlaylistId = playlistId,
                    MainMusicService = MusicServiceType.Yandex,
                    Username = username
                };
                var syncTracks = playlistResult.Value
                    .Tracks?.Select(x => new SyncTrack { Title = x.Title }) ?? Array.Empty<SyncTrack>();
                await synchronizedPlaylistsRepository.InsertAsync(model, syncTracks);
                logger.LogInformation(
                    "Inserted link with Username: {Username}, VkPlaylistId: {PlaylistId}, YandexPlaylistId {ValueId}",
                    username, playlistAddResult.Value.Id, playlistId);
            }

            return playlistAddResult;
        }

        return Result.Fail<Playlist>($"Failed to sync playlist for {username}, id: {playlistId}");
    }
}