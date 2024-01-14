using System.Collections.Concurrent;
using MusicServices.Models;
using VK.Music.Service.Exceptions;

namespace VK.Music.Service.Models.Storage;

public class InMemoryAuthRepository : IAuthRepository
{
    private readonly ConcurrentDictionary<string, LoginModel> loginModels = new();

    public LoginModel GetVkLoginByUsername(string username)
    {
        if (loginModels.TryGetValue(username, out var loginModel)) return loginModel;

        throw new AuthApiException("Логина нету");
    }

    public void AddLoginModelByUsername(string username, LoginModel loginModel)
    {
        loginModels.TryAdd(username, loginModel);
    }
}