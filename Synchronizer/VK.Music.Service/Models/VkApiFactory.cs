using VkNet;

namespace VK.Music.Service.Models;

public class VkApiFactory
{
    private readonly IServiceCollection services;

    public VkApiFactory(IServiceCollection services)
    {
        this.services = services;
    }

    public VkApi CreateApiClient()
    {
        return new VkApi(services);
    }
}