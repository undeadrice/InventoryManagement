using InventoryManagement.Application.Orders.TransferObjects;

namespace InventoryManagement.Application.Orders.Services;

public interface IOrderReadRepository
{
    Task<IReadOnlyCollection<OrderDto>> GetAll(CancellationToken cancellationToken);

    Task<OrderDto?> FindById(Guid id, CancellationToken cancellationToken);
}
