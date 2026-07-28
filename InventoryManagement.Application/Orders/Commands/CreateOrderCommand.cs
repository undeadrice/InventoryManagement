using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Orders.Commands;

[CheckRole(UserRole.User)]
public record CreateOrderCommand(Guid CustomerId, List<OrderItemRequest> Items) : ICommand<Guid>;

public record OrderItemRequest(Guid ProductId, int Quantity);