using Synchronizer.DAL.Entities;

namespace Synchronizer.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SynchronizerDbContext dbContext;

    public UserRepository(SynchronizerDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<User> Items => dbContext.Users.AsQueryable();

    public Guid Insert(User value)
    {
        var inserted = dbContext.Add(value);
        dbContext.SaveChanges();
        return inserted.Entity.Id;
    }

    public async Task<Guid> InsertAsync(User value)
    {
        var inserted = await dbContext.AddAsync(value);
        await dbContext.SaveChangesAsync();
        return inserted.Entity.Id;
    }
}