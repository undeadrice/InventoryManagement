namespace InventoryManagement.API.Products.Responses;

public record ProductResponse(Guid Id, string Name, string Descrption, decimal Price, int Stock);