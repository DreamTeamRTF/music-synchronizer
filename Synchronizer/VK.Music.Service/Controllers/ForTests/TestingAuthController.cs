using Microsoft.AspNetCore.Mvc;
using VK.Music.Service.Configuration;
using VK.Music.Service.Models;
using VK.Music.Service.Models.Auth;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VK.Music.Service.Controllers.ForTests;

[ApiController]
public class TestingAuthController : ControllerBase
{
    private readonly IVkNetApiAuthService authService;
    private readonly VkServiceConfig config;
    private readonly VkApiFactory factory;
    private readonly ITwoFactorVkProvider twoFactorVkProvider;

    public TestingAuthController(
        VkServiceConfig config,
        IVkNetApiAuthService authService,
        ITwoFactorVkProvider twoFactorVkProvider,
        VkApiFactory factory)
    {
        this.config = config;
        this.authService = authService;
        this.twoFactorVkProvider = twoFactorVkProvider;
        this.factory = factory;
    }

    [HttpPost]
    [Route("test/vk/music/auth")]
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
    [Route("test/vk/music/auth/token")]
    public async Task<ActionResult<AuthorizationParameters>> CreateToken(LoginModel loginModel)
    {
        var vkNet = factory.CreateApiClient();
        await vkNet.AuthorizeAsync(new ApiAuthParams
        {
            ApplicationId = config.ApplicationId,
            Login = loginModel.Login,
            Password = loginModel.Password,
            Settings = Settings.Audio,
            TwoFactorAuthorization = twoFactorVkProvider.GetAuthCode,
            TwoFactorSupported = true
        });
        var model = new AuthorizationParameters { Token = vkNet.Token, UserId = vkNet.UserId!.Value };

        return Ok(model);
    }
}