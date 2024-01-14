namespace Synchronizer.WebApp.Models.Synchronizer.LinkedAccounts;

public class LinkedAccountsViewModel
{
    public LinkedAccountsViewModel(LinkedVkAccount? vkAccount, LinkedYandexAccount? yandexAccount)
    {
        VkAccount = vkAccount;
        YandexAccount = yandexAccount;
    }

    public LinkedVkAccount? VkAccount { get; private set; }
    public LinkedYandexAccount? YandexAccount { get; private set; }
}