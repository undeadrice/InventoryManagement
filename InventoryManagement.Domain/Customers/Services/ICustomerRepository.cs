using InventoryManagement.Domain.Customers;
using System.Linq.Expressions;

namespace InventoryManagement.Domain.Customers.Services;

public interface ICustomerRepository
{
    Task Add(Customer customer, CancellationToken token = default);

    Task Update(Customer customer, CancellationToken token = default);

    Task<Customer> GetById(Guid id, CancellationToken token = default);

    Task<Customer?> FindById(Guid id, CancellationToken token = default);

    Task<IReadOnlyCollection<Customer>> GetAll(CancellationToken token, Expression<Func<Customer, bool>>? filter = null);
}
