using InventoryManagement.API.Products.Responses;
using InventoryManagement.Application.Products.TransferObjects;

namespace InventoryManagement.API.Products.Mappings;

public static class ProductMappingExtensions
{
    public static ProductResponse MapToProductResponse(this ProductDto dto) =>
        new ProductResponse(dto.Id, dto.Name, dto.Description, dto.Price, dto.Stock);
}