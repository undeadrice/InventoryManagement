using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Domain.Orders.Exceptions;

public class OrderFinalPriceInvalidException : DomainException
{
    public OrderFinalPriceInvalidException()
        : base("Order final price cannot be negative")
    {
    }
}
