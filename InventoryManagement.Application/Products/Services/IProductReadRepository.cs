using InventoryManagement.Application.Products.TransferObjects;

namespace InventoryManagement.Application.Products.Services;

public interface IProductReadRepository
{
    Task<IReadOnlyCollection<ProductDto>> GetAll(CancellationToken cancellationToken);
}