using System.Collections.Concurrent;
using Yandex.Music.Service.Exceptions;
using Yandex.Music.Client;
using Yandex.Music.Service.Configuration;

namespace Yandex.Music.Service.Models.Auth
{
    public class InMemoryYandexMusicAuthService
    {
        private readonly ConcurrentDictionary<string, AuthorizationParameters> authorizedSessions = new();
        private readonly YandexApiFactory _apiFactory;
        private readonly YandexServiceConfig _config;

        public InMemoryYandexMusicAuthService(YandexApiFactory apiFactory, YandexServiceConfig config)
        {
            _apiFactory = apiFactory;
            _config = config;
        }

        public async Task CreateAuthSessionAsync(string login, string token)
        {
            var yandex = _apiFactory.CreateApiClient();
            if (authorizedSessions.ContainsKey(login)) throw new ArgumentException();

            await yandex.Authorize(token);

            authorizedSessions.TryAdd(login,
                new AuthorizationParameters {Token = token, UserId = yandex.GetLoginInfo()!.Id});
        }

        public async Task<YandexMusicClientAsync> AuthAsync(YandexMusicClientAsync api, string login)
        {
            if (authorizedSessions.TryGetValue(login, out var authParams))
            {
                try
                {
                    await api.Authorize(authParams.Token);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Сессия протухла для пользователя {authParams.UserId} with exception {e}");
                    authorizedSessions.Remove(login, out _);
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
