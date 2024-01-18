using MusicServices.Models;
using Services.Infrastructure;

namespace Synchronizer.Core;

public interface IMusicClient
{
    public Task<Result<Playlist[]>> GetUsersOwnPlaylistsAsync(string username);
    public Task<Result<None>> AddLinkedAccountAsync(string username, string login, string password, string? code);
    public Task<Result<AccountInfoModel>> GetAccountInfoAsync(string username);
}