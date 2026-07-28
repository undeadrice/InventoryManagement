using InventoryManagement.Application.Orders.Services;
using InventoryManagement.Application.Orders.TransferObjects;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Orders;

public class OrderReadRepository(PersistenceDbContext dbContext) : IOrderReadRepository
{
    public async Task<IReadOnlyCollection<OrderDto>> GetAll(CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(
                o.Id,
                o.CustomerId,
                o.OrderItems.Select(oi => new OrderItemDto(oi.ProductId, oi.Quantity, oi.UnitPrice)).ToList(),
                o.FinalPrice,
                o.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderDto?> FindById(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrderDto(
                o.Id,
                o.CustomerId,
                o.OrderItems.Select(oi => new OrderItemDto(oi.ProductId, oi.Quantity, oi.UnitPrice)).ToList(),
                o.FinalPrice,
                o.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
