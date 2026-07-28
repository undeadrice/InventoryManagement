using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Users.Enums;
using InventoryManagement.Domain.Customers;

namespace InventoryManagement.Application.Customers.Commands;

[CheckRole(UserRole.Admin)]
public record CreateCustomerCommand(CustomerLocation Location) : ICommand<Guid>;
