
using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Domain.Products.Exceptions;

public class ProductDescriptionRequiredException() : DomainException("Product description is required.")
{ 
}