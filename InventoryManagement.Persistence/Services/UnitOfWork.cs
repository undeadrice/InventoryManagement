using InventoryManagement.Domain.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManagement.Persistence.Services;

public class UnitOfWork(PersistenceDbContext dbContext) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task StartTransaction()
    {
        // For integration test purposes
        if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        _transaction = await dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await dbContext.SaveChangesAsync();

        if (_transaction != null)
        {
            await _transaction.CommitAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
        }
    }
}
