using VkNet.Model;

namespace VK.Music.Service.Exceptions;

public static class VkAccountNameExtensions
{
    public static string GetAccountFullName(this AccountSaveProfileInfoParams profile)
    {
        return $"{profile.FirstName} {profile.LastName}";
    }
}