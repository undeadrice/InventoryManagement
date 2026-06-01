using InventoryManagement.Application.Products.TransferObjects;
using InventoryManagement.Domain.Products.Entities;

namespace InventoryManagement.Application.Products.Mapping;

public static class ProductMappingExtensions
{
    public static ProductDto MapToProductDto(this Product model) =>
        new ProductDto(model.Id, model.Name, model.Description, model.Price, model.Stock);
}
