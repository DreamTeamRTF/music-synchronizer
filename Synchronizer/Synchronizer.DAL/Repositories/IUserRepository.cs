using Synchronizer.DAL.Entities;

namespace Synchronizer.DAL.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
}