namespace Synchronizer.WebApp.Extensions;

public static class HttpContextExtensions
{
    public static string GetUsername(this HttpContext context)
    {
        return context.User.Identity?.Name!;
    }
}