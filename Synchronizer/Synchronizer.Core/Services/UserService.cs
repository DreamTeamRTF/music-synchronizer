using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Services.Infrastructure;
using Synchronizer.Core.DTO;
using Synchronizer.Core.Helpers;
using Synchronizer.DAL.Entities;
using Synchronizer.DAL.Repositories;

namespace Synchronizer.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<Result<ClaimsIdentity>> Register(UserCredentialsDto userCredentialsDto)
    {
        if (userRepository.Items.Any(user => user.Email == userCredentialsDto.Email))
            return Result.Fail<ClaimsIdentity>("Данный email уже зарегистрирован");

        if (userRepository.Items.Any(user => user.Username == userCredentialsDto.Username))
            return Result.Fail<ClaimsIdentity>("Данный username уже используется кем-то");

        var user = new User
        {
            Username = userCredentialsDto.Username,
            Email = userCredentialsDto.Email,
            Password = PasswordEncrypter.EncryptPassword(userCredentialsDto.Password),
            RoleId = 1
        };

        await userRepository.InsertAsync(user);
        var claims = Authenticate(user);
        return Result.Ok(claims);
    }

    public async Task<Result<ClaimsIdentity>> Login(UserLoginDto userLoginDto)
    {
        var user = await userRepository.Items
            .FirstOrDefaultAsync(user => user.Username == userLoginDto.Username);
        if (user is null) return Result.Fail<ClaimsIdentity>("Пользователь не найден");

        if (user.Password != PasswordEncrypter.EncryptPassword(userLoginDto.Password))
            return Result.Fail<ClaimsIdentity>("Неправильный пароль");

        var claims = Authenticate(user);
        return Result.Ok(claims);
    }

    public async Task<Result<User>> FindUserByUsername(string? username)
    {
        if (username is null) return Result.Fail<User>("Ошибка пользователя");
        var user = await userRepository.Items.FirstOrDefaultAsync(u => u.Username == username);
        return user is null ? Result.Fail<User>("user not found") : Result.Ok(user);
    }

    private static ClaimsIdentity Authenticate(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.Username),
            new(ClaimsIdentity.DefaultRoleClaimType, user?.Role?.Name ?? "SiteUser")
        };
        return new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
    }
}