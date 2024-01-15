using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synchronizer.Core.DTO;
using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;
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

    [HttpGet] // TODO: Заглушка нужно получать из апишек
    public IActionResult Index()
    {
        var vkAccount = vkMusicClient;
        var yandexAccount = yandexMusicClient;
        /*var linkedAccountsViewModel = new LinkedAccountsViewModel(
            new LinkedVkAccount { Name = "AbibosVk", ImageUrl  = "https://sun9-3.userapi.com/impg/c857332/v857332149/319c8/uAEyqabx_WI.jpg?size=1080x1080&quality=96&sign=43d6e86e39d8fbabfcb5277e666df631&type=album" },
            new LinkedYandexAccount { Name = "BaYanex", ImageUrl= "https://avatars.mds.yandex.net/get-yapic/47747/637Scc7TQQW2BmFFiC1ZsnRA3E-1/islands-200" });*/
        var linkedAccountsViewModel = new LinkedAccountsViewModel(null, null);
        return View(linkedAccountsViewModel);
    }

    [HttpGet] // TODO Заглушка
    public async Task<IActionResult> YandexAccount()
    {
        /*var linkedAccountsViewModel = new LinkedAccountsViewModel(
            new LinkedVkAccount { Name = "AbibosVk", ImageUrl  = "https://sun9-3.userapi.com/impg/c857332/v857332149/319c8/uAEyqabx_WI.jpg?size=1080x1080&quality=96&sign=43d6e86e39d8fbabfcb5277e666df631&type=album" },
            new LinkedYandexAccount { Name = "BaYanex", ImageUrl= "https://avatars.mds.yandex.net/get-yapic/47747/637Scc7TQQW2BmFFiC1ZsnRA3E-1/islands-200" });*/
        var yandexAccount = new LinkedYandexAccount
        {
            Name = "BaYanex",
            ImageUrl = "https://avatars.mds.yandex.net/get-yapic/47747/637Scc7TQQW2BmFFiC1ZsnRA3E-1/islands-200"
        };

        return View(yandexAccount);
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