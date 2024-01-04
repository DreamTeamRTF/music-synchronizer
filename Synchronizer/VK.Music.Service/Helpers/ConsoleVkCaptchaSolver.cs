using VkNet.Utils.AntiCaptcha;

namespace VK.Music.Service.Helpers;

public class ConsoleVkCaptchaSolver : ICaptchaSolver
{
    public string? Solve(string url)
    {
        Console.WriteLine($"Капча по урлу {url}, Введи ответ:");
        return Console.ReadLine();
    }

    public void CaptchaIsFalse() // хуй бы с ним
    {
        throw new NotImplementedException();
    }
}