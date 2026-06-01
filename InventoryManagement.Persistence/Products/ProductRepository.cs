using InventoryManagement.Domain.Products.Entities;
using InventoryManagement.Domain.Products.Services;
using InventoryManagement.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryManagement.Persistence.Products
{
    public class RestaurantRepository(PersistenceDbContext persistenceDbContext) : IProductRepository
    {
        public async Task Add(Product product, CancellationToken token)
        {
            await persistenceDbContext.Products.AddAsync(product);
        }

        public async Task Update(Product product, CancellationToken token)
        {
            persistenceDbContext.Products.Update(product);
        }

        public async Task<Product> GetById(Guid id, CancellationToken token)
        {
            var result = await persistenceDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                throw new NotFoundException($"Product with id {id} doesn't exist");
            }

            return result;
        }

        public async Task<Product?> FindById(Guid id, CancellationToken token)
        {
            return await persistenceDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyCollection<Product>> GetAll(Expression<Func<Product, bool>>? filter = null, CancellationToken token)
        {
            if (filter == null)
            {
                return await persistenceDbContext.Products.ToListAsync();
            }

            return await persistenceDbContext.Products.Where(filter).ToListAsync();
        }
    }
}
