namespace Synchronizer.DAL.Entities;

public class AccountLink : BaseEntity<Guid>
{
    public string Username { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
}