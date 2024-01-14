using MusicServices.Models;

namespace VK.Music.Service.Models.Storage;

public interface IAuthRepository
{
    public LoginModel GetVkLoginByUsername(string username);
    public void AddLoginModelByUsername(string username, LoginModel loginModel);
}