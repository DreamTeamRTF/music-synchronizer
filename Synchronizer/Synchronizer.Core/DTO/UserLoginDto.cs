using System.ComponentModel.DataAnnotations;

namespace Synchronizer.Core.DTO;

public class UserLoginDto
{
    [Required(ErrorMessage = "Введите имя пользователя")]
    public string Username { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Введите ваш пароль")]
    [MinLength(6, ErrorMessage = "Введите минимум 6 символов")]
    public string Password { get; set; }
}