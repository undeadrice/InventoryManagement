using InventoryManagement.Domain.Customers;
using InventoryManagement.Domain.Customers.Services;
using InventoryManagement.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryManagement.Persistence.Customers;

public class CustomerRepository(PersistenceDbContext persistenceDbContext) : ICustomerRepository
{
    public async Task Add(Customer customer, CancellationToken token)
    {
        await persistenceDbContext.Customers.AddAsync(customer, token);
    }

    public async Task Update(Customer customer, CancellationToken token)
    {
        persistenceDbContext.Customers.Update(customer);
    }

    public async Task<Customer> GetById(Guid id, CancellationToken token)
    {
        var result = await persistenceDbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);

        if (result == null)
        {
            throw new NotFoundException($"Customer with id {id} doesn't exist");
        }

        return result;
    }

    public async Task<Customer?> FindById(Guid id, CancellationToken token)
    {
        return await persistenceDbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);
    }

    public async Task<IReadOnlyCollection<Customer>> GetAll(CancellationToken token, Expression<Func<Customer, bool>>? filter = null)
    {
        if (filter == null)
        {
            return await persistenceDbContext.Customers.ToListAsync(cancellationToken: token);
        }

        return await persistenceDbContext.Customers.Where(filter).ToListAsync(cancellationToken: token);
    }
}
