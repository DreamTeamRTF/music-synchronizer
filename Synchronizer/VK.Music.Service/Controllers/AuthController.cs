using Microsoft.AspNetCore.Mvc;
using MusicServices.Models;
using VK.Music.Service.Exceptions;
using VK.Music.Service.Models.Account;
using VK.Music.Service.Models.Auth;

namespace VK.Music.Service.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IVkAccountService accountService;
    private readonly IVkNetApiAuthService authService;
    private readonly ILogger<AuthController> logger;

    public AuthController(IVkNetApiAuthService authService, IVkAccountService accountService,
        ILogger<AuthController> logger)
    {
        this.authService = authService;
        this.accountService = accountService;
        this.logger = logger;
    }

    [HttpPost]
    [Route("vk/music/auth")]
    public async Task<ActionResult> Auth([FromBody] LoginModel loginModel)
    {
        await authService.CreateAuthSessionAsync(loginModel.Username, loginModel.Login, loginModel.Password,
            loginModel.SecondFactorCode);
        return Ok();
    }

    [HttpGet]
    [Route("vk/music/account/info")]
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