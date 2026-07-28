using InventoryManagement.Application.Pipeline;

namespace InventoryManagement.Application.Products.Commands;

public record SeedProductsCommand(int Quantity) : ICommand<int>;
