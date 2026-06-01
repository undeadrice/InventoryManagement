using InventoryManagement.Application.Pipeline;

namespace InventoryManagement.Application.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock)
    : ICommand<Guid>;

