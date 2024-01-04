using Microsoft.AspNetCore.Mvc;
using VK.Music.Service.Models.Auth;

namespace VK.Music.Service.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IVkNetApiAuthService authService;

    public AuthController(IVkNetApiAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost]
    [Route("vk/music/auth")]
    public async Task<ActionResult> Auth([FromBody] LoginModel loginModel)
    {
        await authService.CreateAuthSessionAsync(loginModel.Login, loginModel.Password);
        return Ok();
    }
}