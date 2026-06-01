namespace InventoryManagement.Domain.Persistence;

public interface IUnitOfWork
{
    Task StartTransaction();

    Task CommitAsync();

    Task RollbackAsync();
}


