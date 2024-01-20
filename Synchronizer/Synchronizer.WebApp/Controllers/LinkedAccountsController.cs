using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synchronizer.Core.DTO;
using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;
using Synchronizer.Models.Contracts.VK;
using Synchronizer.WebApp.Extensions;
using Synchronizer.WebApp.Models.Synchronizer.LinkedAccounts;

namespace Synchronizer.WebApp.Controllers;

[Authorize]
public class LinkedAccountsController : Controller
{
    private readonly VkMusicClient vkMusicClient;
    private readonly YandexMusicClient yandexMusicClient;

    public LinkedAccountsController(VkMusicClient vkMusicClient, YandexMusicClient yandexMusicClient)
    {
        this.vkMusicClient = vkMusicClient;
        this.yandexMusicClient = yandexMusicClient;
    }

    [HttpGet]
    public async Task<IActionResult> YandexAccount()
    {
        var accountResult = await yandexMusicClient.GetAccountInfoAsync(Request.HttpContext.GetUsername());
        if (!accountResult.IsSuccess) return RedirectToAction("YandexAccountForm");

        var model = new LinkedYandexAccount { Name = accountResult.Value.Name, ImageUrl = accountResult.Value.ImageUrl };
        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> VkMusicAccount()
    {
        var accountResult = await vkMusicClient.GetAccountInfoAsync(Request.HttpContext.GetUsername());
        if (!accountResult.IsSuccess) return RedirectToAction("VkAccountForm");

        var model = new LinkedVkAccount { Name = accountResult.Value.Name, ImageUrl = accountResult.Value.ImageUrl };
        return View(model);
    }

    [HttpGet]
    public IActionResult VkAccountForm()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VkAccountForm(VkLoginDto loginDto)
    {
        if (!ModelState.IsValid) return View(loginDto);

        var result = await vkMusicClient.AddLinkedAccountAsync(
            Request.HttpContext.GetUsername(),
            loginDto.Login,
            loginDto.Password,
            loginDto.Code2FA);

        if (result.IsSuccess) return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", result.Error);
        return View(loginDto);
    }

    [HttpGet]
    public IActionResult YandexAccountForm()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> YandexAccountForm(YandexLoginDto loginDto)
    {
        if (!ModelState.IsValid) return View(loginDto);

        var result = await yandexMusicClient.AddLinkedAccountAsync(
            Request.HttpContext.GetUsername(),
            loginDto.Login,
            loginDto.Password,
            null);

        if (result.IsSuccess) return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", result.Error);
        return View(loginDto);
    }
}