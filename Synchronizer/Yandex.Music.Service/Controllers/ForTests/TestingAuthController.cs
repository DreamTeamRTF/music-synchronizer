using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using Yandex.Music.Service.Configuration;
using Yandex.Music.Service.Models;
using Yandex.Music.Service.Models.Auth;

namespace Yandex.Music.Service.Controllers.ForTests
{
    [ApiController]
    public class TestingAuthController : ControllerBase
    {
        private readonly InMemoryYandexMusicAuthService _authService;
        private readonly YandexApiFactory _apiFactory;
        private readonly YandexServiceConfig _config;

        public TestingAuthController(
            YandexServiceConfig config,
            InMemoryYandexMusicAuthService authService,
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
        //Авторизация через пароль идет в жопу, ибо не работает создание сессии.
        //Нужно или пользователю мануально вводить токен, или авторизоваться как-то по другому
        //Сейчас используй вместо пароля токен 
        public async Task<ActionResult<AuthorizationParameters>> CreateToken(LoginModel loginModel)
        {
            var yandexApi = _apiFactory.CreateApiClient();
            try
            {
                await yandexApi.Authorize(loginModel.Password);
            }
            catch (Exception ex)
            {
                return Problem("Cannot create AuthSession. Try again later");
            }
            //var token = await yandexApi.GetAccessToken();
            var model = new AuthorizationParameters { Token = loginModel.Password, UserId = long.Parse(yandexApi.Account.Uid) };

            return Ok(model);
        }
    }
}
