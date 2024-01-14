namespace Synchronizer.Core.DTO;

public class UserAuthDto
{
    public Guid RequestId { get; set; }
    public string Username { get; set; }
    public DateTime Date { get; set; }
}