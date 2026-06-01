namespace InventoryManagement.Application.Orders.TransferObjects;

public record OrderDto(Guid Id, Guid CustomerId, List<OrderItemDto> Items, decimal FinalPrice, DateTime CreatedAt);
