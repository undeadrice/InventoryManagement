using InventoryManagement.Application.Pipeline;
using InventoryManagement.Domain.Customers;

namespace InventoryManagement.Application.Customers.Commands;

public record CreateCustomerCommand(CustomerLocation Location) : ICommand<Guid>;
