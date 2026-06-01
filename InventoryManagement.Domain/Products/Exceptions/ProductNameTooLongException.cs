using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Domain.Products.Exceptions;

public class ProductNameTooLongException() : DomainException("Product name cannot exceed 50 characters.")
{
}