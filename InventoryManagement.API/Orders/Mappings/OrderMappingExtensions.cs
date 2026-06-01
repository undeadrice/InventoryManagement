using InventoryManagement.API.Orders.Responses;
using InventoryManagement.Application.Orders.TransferObjects;

namespace InventoryManagement.API.Orders.Mappings;

public static class OrderMappingExtensions
{
    public static OrderResponse MapToOrderResponse(this OrderDto dto) =>
        new OrderResponse(
            dto.Id,
            dto.CustomerId,
            dto.Items.Select(i => new OrderItemResponse(i.ProductId, i.Quantity, i.UnitPrice)).ToList(),
            dto.FinalPrice,
            dto.CreatedAt);
}
