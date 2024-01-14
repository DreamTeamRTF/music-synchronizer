using MusicServices.Models;
using Services.Infrastructure;

namespace Synchronizer.Core.Yandex;

public class YandexMusicClient : IYandexMusicClient
{
    public Task<Playlist[]> GetUsersOwnPlaylistsAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<Result<None>> AddLinkedAccountAsync(string username, string login, string password, string? code)
    {
        throw new NotImplementedException();
    }
}