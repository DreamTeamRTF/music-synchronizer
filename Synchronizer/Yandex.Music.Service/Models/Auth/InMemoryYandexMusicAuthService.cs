using System.Collections.Concurrent;
using Yandex.Music.Service.Exceptions;
using Yandex.Music.Client;
using Yandex.Music.Service.Configuration;

namespace Yandex.Music.Service.Models.Auth
{
    public class InMemoryYandexMusicAuthService
    {
        private readonly ConcurrentDictionary<string, AuthorizationParameters> authorizedSessions = new();

        public async Task CreateAuthSessionAsync(string username, string login, string password)
        {
            var yandex = YandexApiFactory.CreateApiClient();
            if (authorizedSessions.ContainsKey(username)) throw new ArgumentException();

            await yandex.CreateAuthSession(login);
            await yandex.AuthorizeByAppPassword(password);
            var token = await yandex.GetAccessToken();

            authorizedSessions.TryAdd(username,
                new AuthorizationParameters {Token = token.AccessToken, UserId = yandex.GetLoginInfo()!.Id});
        }
        
        public async Task CreateAuthWithTokenAsync(string username, string token)
        {
            var yandex = YandexApiFactory.CreateApiClient();
            if (authorizedSessions.ContainsKey(username)) throw new ArgumentException();

            await yandex.Authorize(token);

            authorizedSessions.TryAdd(username,
                new AuthorizationParameters {Token = token, UserId = yandex.GetLoginInfo()!.Id});
        }

        public async Task<YandexMusicClientAsync> AuthAsync(YandexMusicClientAsync api, string username)
        {
            if (authorizedSessions.TryGetValue(username, out var authParams))
            {
                try
                {
                    await api.Authorize(authParams.Token);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Сессия протухла для пользователя {authParams.UserId} with exception {e}");
                    authorizedSessions.Remove(username, out _);
                }

                if (api.IsAuthorized) return api;
            }

            throw new AuthApiException("Something went wrong in auth");
        }

        public void AddTestToken(string login, AuthorizationParameters authParams)
        {
            authorizedSessions.TryAdd(login, authParams);
        }
    }
}
