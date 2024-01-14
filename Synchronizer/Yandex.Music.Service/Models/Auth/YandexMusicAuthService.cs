using System.Collections.Concurrent;
using Yandex.Music.Service.Exceptions;
using Yandex.Music.Client;

namespace Yandex.Music.Service.Models.Auth
{
    public class YandexMusicAuthService
    {
        private readonly ConcurrentDictionary<string, AuthorizationParameters> authorizedSessions = new();
        private readonly YandexApiFactory apiFactory;

        public YandexMusicAuthService(YandexApiFactory apiFactory)
        {
            this.apiFactory = apiFactory;
        }

        public async Task CreateAuthSessionAsync(string login, string password)
        {
            var yandex = apiFactory.CreateApiClient();
            if (authorizedSessions.ContainsKey(login)) throw new ArgumentException();

            try
            {
                await yandex.CreateAuthSession(login);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create authSession, probably capcha. Try again later");
            }
            await yandex.AuthorizeByAppPassword(password);
            var token = await yandex.GetAccessToken();

            authorizedSessions.TryAdd(login,
                new AuthorizationParameters {Token = token.AccessToken, UserId = yandex.GetLoginInfo()!.Id});
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
