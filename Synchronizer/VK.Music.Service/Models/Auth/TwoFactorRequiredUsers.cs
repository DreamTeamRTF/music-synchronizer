using System.Collections.Concurrent;

namespace VK.Music.Service.Models.Auth;

public class TwoFactorRequiredUsers
{
    public static ConcurrentDictionary<string, string> RequiredSecondFactor = new ();
}