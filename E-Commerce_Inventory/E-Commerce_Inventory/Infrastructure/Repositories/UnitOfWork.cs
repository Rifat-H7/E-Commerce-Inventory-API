using E_Commerce_Inventory.Domain.Entities;
using E_Commerce_Inventory.Domain.Interfaces;
using E_Commerce_Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce_Inventory.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            RefreshTokens = new Repository<RefreshToken>(_context);
            Categories = new Repository<Category>(_context);
            Products = new Repository<Product>(_context);
        }

        public IRepository<User> Users { get; private set; }
        public IRepository<RefreshToken> RefreshTokens { get; private set; }
        public IRepository<Category> Categories { get; private set; }
        public IRepository<Product> Products { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
