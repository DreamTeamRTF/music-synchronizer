namespace Synchronizer.DAL.Entities;

public class BaseEntity<TId>
{
    public TId Id { get; set; }
}