namespace InventoryManagement.Application.Orders.TransferObjects;

public record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
