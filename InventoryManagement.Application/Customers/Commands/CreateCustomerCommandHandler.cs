using InventoryManagement.Domain.Customers;
using InventoryManagement.Domain.Customers.Services;
using MediatR;

namespace InventoryManagement.Application.Customers.Commands;

public class CreateCustomerCommandHandler(ICustomerRepository customerRepository) : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(request.Location);

        await customerRepository.Add(customer, cancellationToken);

        return customer.Id;
    }
}
