using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using Yandex.Music.Service.Exceptions;
using Yandex.Music.Service.Models.Account;
using Yandex.Music.Service.Models.Auth;

namespace Yandex.Music.Service.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly YandexAccountService accountService;
    private readonly InMemoryYandexMusicAuthService authService;
    private readonly ILogger<AuthController> logger;

    public AuthController(
        InMemoryYandexMusicAuthService authService,
        YandexAccountService accountService,
        ILogger<AuthController> logger)
    {
        this.authService = authService;
        this.accountService = accountService;
        this.logger = logger;
    }

    [HttpPost]
    [Route("yandex/music/auth")]
    public async Task<ActionResult> Auth([FromBody] LoginModel loginModel)
    {
        await authService.CreateAuthSessionAsync(loginModel.Username, loginModel.Login, loginModel.Password);
        return Ok();
    }

    [HttpPost]
    [Route("yandex/music/auth/token")]
    public async Task<ActionResult> Auth([FromBody] TokenLoginModel loginModel)
    {
        await authService.CreateAuthWithTokenAsync(loginModel.Username, loginModel.Token);
        return Ok();
    }

    [HttpGet]
    [Route("yandex/music/account/info")]
    public async Task<ActionResult<AccountInfoModel>> AccountInfo(string username)
    {
        try
        {
            var account = await accountService.GetAccountInfoAsync(username);
            logger.LogInformation("Successed: {AccountName}", account.Name);
            return Ok(account);
        }
        catch (AuthApiException e)
        {
            logger.LogError("Failed with exception: {E}", e);
            return BadRequest(e.Message);
        }
    }
}