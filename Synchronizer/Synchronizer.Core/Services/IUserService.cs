using System.Security.Claims;
using Services.Infrastructure;
using Synchronizer.Core.DTO;
using Synchronizer.DAL.Entities;

namespace Synchronizer.Core.Services;

public interface IUserService
{
    public Task<Result<ClaimsIdentity>> Register(UserCredentialsDto userCredentialsDto);
    public Task<Result<ClaimsIdentity>> Login(UserLoginDto userLoginDto);
    public Task<Result<User>> FindUserByUsername(string? username);
}