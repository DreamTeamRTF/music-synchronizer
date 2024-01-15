using System.ComponentModel.DataAnnotations;

namespace Synchronizer.Core.DTO;

public class YandexLoginDto
{
    [Required(ErrorMessage = "Введите имя пользователя")]
    public string Login { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Введите ваш пароль")]
    public string Password { get; set; }
}