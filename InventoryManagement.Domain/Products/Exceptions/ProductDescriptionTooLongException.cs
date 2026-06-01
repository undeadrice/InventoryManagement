using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Domain.Products.Exceptions;

public class ProductDescriptionTooLongException() : DomainException("Product description cannot exceed 50 characters.")
{
}