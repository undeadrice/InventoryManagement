using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Domain.Products.Exceptions;

public class ProductStockInvalidException() : DomainException("Product stock cannot be negative.")
{ 
}