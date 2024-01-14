using MusicServices.Models;

namespace VK.Music.Service.Models.Account;

public interface IVkAccountService
{
    public Task<AccountInfoModel> GetAccountInfoAsync(string username);
}