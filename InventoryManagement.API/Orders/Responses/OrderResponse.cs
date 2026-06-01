namespace InventoryManagement.API.Orders.Responses;

public record OrderItemResponse(Guid ProductId, int Quantity, decimal UnitPrice);

public record OrderResponse(Guid Id, Guid CustomerId, List<OrderItemResponse> Items, decimal FinalPrice, DateTime CreatedAt);
