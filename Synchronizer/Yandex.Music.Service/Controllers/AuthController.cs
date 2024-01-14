using Microsoft.AspNetCore.Mvc;
using Yandex.Music.Service.Models.Auth;

namespace Yandex.Music.Service.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly YandexMusicAuthService _authService;

        public AuthController(YandexMusicAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("yandex/music/auth")]
        public async Task<ActionResult> Auth([FromBody] LoginModel loginModel)
        {
            await _authService.CreateAuthSessionAsync(loginModel.Login, loginModel.Password);
            return Ok();
        }
    }
}
