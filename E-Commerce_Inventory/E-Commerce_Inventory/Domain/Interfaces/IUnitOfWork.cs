using E_Commerce_Inventory.Domain.Entities;

namespace E_Commerce_Inventory.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<RefreshToken> RefreshTokens { get; }
        IRepository<Category> Categories { get; }
        IRepository<Product> Products { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
