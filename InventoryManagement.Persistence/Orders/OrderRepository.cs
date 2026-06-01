using InventoryManagement.Domain.Orders;
using InventoryManagement.Domain.Orders.Services;
using InventoryManagement.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryManagement.Persistence.Orders;

public class OrderRepository(PersistenceDbContext persistenceDbContext) : IOrderRepository
{
    public async Task Add(Order order, CancellationToken token)
    {
        await persistenceDbContext.Orders.AddAsync(order, token);
    }

    public async Task Update(Order order, CancellationToken token)
    {
        persistenceDbContext.Orders.Update(order);
    }

    public async Task<Order> GetById(Guid id, CancellationToken token)
    {
        var result = await persistenceDbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);

        if (result == null)
        {
            throw new NotFoundException($"Order with id {id} doesn't exist");
        }

        return result;
    }

    public async Task<Order?> FindById(Guid id, CancellationToken token)
    {
        return await persistenceDbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);
    }

    public async Task<IReadOnlyCollection<Order>> GetAll(CancellationToken token, Expression<Func<Order, bool>>? filter = null)
    {
        if (filter == null)
        {
            return await persistenceDbContext.Orders
                .Include(o => o.OrderItems)
                .ToListAsync(cancellationToken: token);
        }

        return await persistenceDbContext.Orders
            .Include(o => o.OrderItems)
            .Where(filter)
            .ToListAsync(cancellationToken: token);
    }
}
