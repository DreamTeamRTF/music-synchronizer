using MusicServices.Models;
using System.Collections.Concurrent;
using Yandex.Music.Service.Exceptions;

namespace Yandex.Music.Service.Models.Storage
{
    public class InMemoryAuthRepository
    {
        private readonly ConcurrentDictionary<string, LoginModel> loginModels = new();

        public LoginModel GetYandexLoginByUsername(string username)
        {
            if (loginModels.TryGetValue(username, out var loginModel)) return loginModel;

            throw new AuthApiException("Логина нету");
        }

        public void AddLoginModelByUsername(string username, LoginModel loginModel)
        {
            loginModels.TryAdd(username, loginModel);
        }
    }
}
