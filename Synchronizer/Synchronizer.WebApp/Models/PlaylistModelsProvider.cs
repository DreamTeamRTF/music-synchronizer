using MusicServices.Models;
using Synchronizer.DAL.Entities;
using Synchronizer.DAL.Repositories;
using Synchronizer.WebApp.Helpers;
using Synchronizer.WebApp.Models.Synchronizer.Playlists;

namespace Synchronizer.WebApp.Models;

public class PlaylistModelsProvider
{
    private readonly SynchronizedPlaylistsRepository playlistsRepository;
    private readonly ILogger<PlaylistModelsProvider> logger;

    public PlaylistModelsProvider(SynchronizedPlaylistsRepository playlistsRepository, ILogger<PlaylistModelsProvider> logger)
    {
        this.playlistsRepository = playlistsRepository;
        this.logger = logger;
    }

    public OwnPlaylistViewModel[] FindSyncPlaylists(string username, Playlist[] playlists, MusicServiceTypeModel musicService)
    {
        var linksForUser = playlistsRepository.Items.Where(x => x.Username == username);
        return playlists
            .Select(x => new OwnPlaylistViewModel
            {
                Playlist = x,
                IsSynchronized = IsPlaylistSync(linksForUser, x, musicService.ToMusicServiceType())
            })
            .ToArray();
    }

    private bool IsPlaylistSync(IQueryable<SynchronizedPlaylistLink> links, Playlist playlist, MusicServiceType mainService)
    {
        foreach (var link in links)
        {
            var ids = GetMainId(link, mainService);
            logger.LogInformation($"For link {link.Id}, MainId is {ids.MainId} Side is {ids.SideId}, YandexId={link.YandexPlaylistId}, VkId={link.VkPlaylistId}." +
                                  $" MainService for link {link.MainMusicService}, called main service {mainService}, PLAYLIST ID: {playlist.Id}");
            if ((link.MainMusicService == mainService && ids.MainId == playlist.Id)
                || (link.MainMusicService != mainService && ids.MainId == playlist.Id))
            {
                return true;
            }
        }

        return false;
    }

    private static (long MainId, long SideId) GetMainId(SynchronizedPlaylistLink link, MusicServiceType mainService) => mainService switch
    {
        MusicServiceType.Vk => (link.VkPlaylistId, link.YandexPlaylistId),
        MusicServiceType.Yandex => (link.YandexPlaylistId, link.VkPlaylistId),
        _ => throw new ArgumentException()
    };
}