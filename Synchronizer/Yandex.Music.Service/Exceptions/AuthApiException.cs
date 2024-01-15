namespace Yandex.Music.Service.Exceptions;

public class AuthApiException : Exception
{
    public AuthApiException(string message) : base(message)
    {
    }
}