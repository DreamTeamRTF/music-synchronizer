using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Synchronizer.Core.DTO;
using Synchronizer.Core.Services;

namespace Synchronizer.WebApp.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> logger;
    private readonly IUserService userService;

    public AccountController(ILogger<AccountController> logger, IUserService userService)
    {
        this.logger = logger;
        this.userService = userService;
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserCredentialsDto userCredentialsDto)
    {
        if (!ModelState.IsValid) return View(userCredentialsDto);

        var register = await userService.Register(userCredentialsDto);
        if (register.IsSuccess)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(register.Value));
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", register.Error);
        return View(userCredentialsDto);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {
        if (!ModelState.IsValid) return View(loginDto);

        var login = await userService.Login(loginDto);
        if (login.IsSuccess)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(login.Value));
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", login.Error);
        return View(loginDto);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}