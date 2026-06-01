using InventoryManagement.Application.Pipeline;

namespace InventoryManagement.Application.Orders.Commands;

public record OrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderCommand(Guid CustomerId, List<OrderItemRequest> Items) : ICommand<Guid>;
