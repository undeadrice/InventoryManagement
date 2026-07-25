using InventoryManagement.Application.Products.TransferObjects;

namespace InventoryManagement.Application.Products.Services;

public interface IProductDapperReadRepository
{
    Task<IReadOnlyCollection<ProductDto>> GetAll(CancellationToken cancellationToken);
}