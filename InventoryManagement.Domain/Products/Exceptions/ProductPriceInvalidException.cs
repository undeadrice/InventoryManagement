using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Domain.Products.Exceptions;

public class ProductPriceInvalidException() : DomainException("Product price must be greater than zero.")
{ 
}