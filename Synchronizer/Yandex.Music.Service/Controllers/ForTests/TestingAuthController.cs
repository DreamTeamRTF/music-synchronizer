using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yandex.Music.Service.Configuration;
using Yandex.Music.Service.Models;
using Yandex.Music.Service.Models.Auth;

namespace Yandex.Music.Service.Controllers.ForTests
{
    [ApiController]
    public class TestingAuthController : ControllerBase
    {
        private readonly YandexMusicAuthService _authService;
        private readonly YandexApiFactory _apiFactory;
        private readonly YandexServiceConfig _config;

        public TestingAuthController(
            YandexServiceConfig config,
            YandexMusicAuthService authService,
            YandexApiFactory apiFactory)
        {
            _config = config;
            _authService = authService;
            _apiFactory = apiFactory;
        }

        [HttpPost]
        [Route("test/yandex/music/auth")]
        public ActionResult TestAuth()
        {
            _authService.AddTestToken(_config.TestLogin, new AuthorizationParameters
            {
                Token = _config.TestToken,
                UserId = _config.TestUserId
            });

            return Ok();
        }

        [HttpPost]
        [Route("test/yandex/music/auth/token")]
        public async Task<ActionResult<AuthorizationParameters>> CreateToken(LoginModel loginModel)
        {
            var yandexApi = _apiFactory.CreateApiClient();
            try
            {
                await yandexApi.CreateAuthSession(loginModel.Login);
            }
            catch (Exception ex)
            {
                return Problem("Cannot create AuthSession. Try again later");
            }
            await yandexApi.AuthorizeByAppPassword(loginModel.Password);
            var token = await yandexApi.GetAccessToken();
            var model = new AuthorizationParameters { Token = token.AccessToken, UserId = long.Parse(yandexApi.Account.Uid) };

            return Ok(model);
        }
    }
}
