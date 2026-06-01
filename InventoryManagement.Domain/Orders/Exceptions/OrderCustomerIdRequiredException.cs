using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Domain.Orders.Exceptions;

public class OrderCustomerIdRequiredException : DomainException
{
    public OrderCustomerIdRequiredException()
        : base("Order customer id is required")
    {
    }
}
