using RestSharp;

namespace Synchronizer.Core.Helpers;

public static class RestClientFactory
{
    public static RestClient CreateRestClient(string url)
    {
        return new RestClient(url, op => { op.ThrowOnAnyError = false; });
    }
}