using InventoryManagement.Application.Products.Services;
using InventoryManagement.Application.Products.TransferObjects;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Products;

public class ProductReadRepository(PersistenceDbContext dbContext) : IProductReadRepository
{
    public async Task<IReadOnlyCollection<ProductDto>> GetAll(CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock))
            .ToListAsync(cancellationToken);
    }
}