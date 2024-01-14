namespace VK.Music.Service.Models.Auth;

public class ConsoleTwoFactorVkProvider : ITwoFactorVkProvider
{
    public Task<string> GetAuthCodeAsync()
    {
        return Task.Run(() =>
        {
            Console.WriteLine("Введите код");
            var code = Console.ReadLine();
            Console.WriteLine($"Введенный код {code}");
            return code!;
        });
    }

    public string GetAuthCode(string username)
    {
        Console.WriteLine("Введите код");
        var code = Console.ReadLine();
        Console.WriteLine($"Введенный код {code}");
        return code!;
    }
}