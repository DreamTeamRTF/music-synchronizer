using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicServices.Models;
using Yandex.Music.Service.Models.Acount;
using Yandex.Music.Service.Models.Auth;
using Yandex.Music.Service.Exceptions;

namespace Yandex.Music.Service.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly InMemoryYandexMusicAuthService _authService;
        private readonly YandexAccountService _accountService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(InMemoryYandexMusicAuthService authService,
            YandexAccountService accountService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost]
        [Route("yandex/music/auth")]
        //Авторизация через пароль идет в жопу, ибо не работает создание сессии.
        //Нужно или пользователю мануально вводить токен, или авторизоваться как-то по другому
        //Сейчас используй вместо пароля токен 
        public async Task<ActionResult> Auth([FromBody] LoginModel loginModel)
        {
            await _authService.CreateAuthSessionAsync(loginModel.Login, loginModel.Password);
            return Ok();
        }

        [HttpGet]
        [Route("yandex/music/account/info")]
        public async Task<ActionResult<AccountInfoModel>> AccountInfo(string username)
        {
            try
            {
                var account = await _accountService.GetAccountInfoAsync(username);
                _logger.LogInformation("Successed: {AccountName}", account.Name);
                return Ok(account);
            }
            catch (AuthApiException e)
            {
                _logger.LogError("Failed with exception: {E}", e);
                return BadRequest(e.Message);
            }
        }
    }
}
