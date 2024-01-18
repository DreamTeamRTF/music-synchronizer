using RestSharp;

namespace Synchronizer.Core.Helpers;

public static class RestClientFactory
{
    public static RestClient CreateRestClient(string url) => new(url, op =>
    {
        op.ThrowOnAnyError = false;
    });
}