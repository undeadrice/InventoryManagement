using InventoryManagement.Domain.Products.Entities;
using System.Linq.Expressions;

namespace InventoryManagement.Domain.Products.Services;

public interface IProductRepository
{
    Task Add(Product product, CancellationToken token = default);

    Task Update(Product product, CancellationToken token = default);

    Task<Product> GetById(Guid id, CancellationToken token = default);

    Task<Product?> FindById(Guid id, CancellationToken token = default);

    Task<IReadOnlyCollection<Product>> GetAll(CancellationToken token, Expression<Func<Product, bool>>? filter = null);
}

