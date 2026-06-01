using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Domain.Products.Exceptions;

public class ProductNameRequiredException() : DomainException("Product name is required.")
{
}