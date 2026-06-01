using InventoryManagement.Application.Orders.TransferObjects;
using InventoryManagement.Domain.Orders.Entities;

namespace InventoryManagement.Application.Orders.Mapping;

public static class OrderMappingExtensions
{
    public static OrderDto MapToOrderDto(this Order model) =>
        new OrderDto(
            model.Id,
            model.CustomerId,
            model.OrderItems.Select(oi => new OrderItemDto(oi.ProductId, oi.Quantity, oi.UnitPrice)).ToList(),
            model.FinalPrice,
            model.CreatedAt);
}
