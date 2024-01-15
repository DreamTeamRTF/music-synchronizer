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
        private readonly InMemoryYandexMusicAuthService authService;
        private readonly YandexServiceConfig config;

        public TestingAuthController(
            YandexServiceConfig config,
            InMemoryYandexMusicAuthService authService)
        {
            this.config = config;
            this.authService = authService;
        }

        [HttpPost]
        [Route("test/yandex/music/auth")]
        public ActionResult TestAuth()
        {
            authService.AddTestToken(config.TestLogin, new AuthorizationParameters
            {
                Token = config.TestToken,
                UserId = config.TestUserId
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
            var yandexApi = YandexApiFactory.CreateApiClient();
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
