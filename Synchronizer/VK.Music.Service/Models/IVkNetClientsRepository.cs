using VkNet;

namespace VK.Music.Service.Models;

public interface IVkNetClientsRepository
{
    public Task<VkApi> GetAuthenticatedVkNetApiAsync(string username);
}