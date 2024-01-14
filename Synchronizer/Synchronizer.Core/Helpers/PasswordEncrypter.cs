using System.Security.Cryptography;
using System.Text;

namespace Synchronizer.Core.Helpers;

public static class PasswordEncrypter
{
    public static string EncryptPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var hash = BitConverter.ToString(bytes).Replace("_", "").ToLower();
        return hash;
    }
}