using System.ComponentModel.DataAnnotations;

namespace Synchronizer.Core.DTO;

public class UserCredentialsDto
{
    [Required(ErrorMessage = "Введите имя пользователя")]
    [MaxLength(20, ErrorMessage = "Имя пользователя болжно содержать не более 20 символов")]
    [MinLength(2, ErrorMessage = "Имя пользователя болжно содержать минимум 2 символа")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Введите ваш email")]
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Введите ваш пароль")]
    [MinLength(6, ErrorMessage = "Введите минимум 6 символов")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Подтвердите ваш пароль")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    public string PasswordConfirm { get; set; }
}