using InventoryManagement.Domain.Orders.Entities;
using System.Linq.Expressions;

namespace InventoryManagement.Domain.Orders.Services;

public interface IOrderRepository
{
    Task Add(Order order, CancellationToken token);

    Task Update(Order order, CancellationToken token);

    Task<Order> GetById(Guid id, CancellationToken token);

    Task<Order?> FindById(Guid id, CancellationToken token);

    Task<IReadOnlyCollection<Order>> GetAll(CancellationToken token, Expression<Func<Order, bool>>? filter = null);
}
