using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Orders.Exceptions;

namespace InventoryManagement.Domain.Orders.Entities;

public class Order : IUserOwnedEntity
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public Guid CustomerId { get; private set; }

    public List<OrderItem> OrderItems { get; private set; } = new();

    public decimal FinalPrice { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Order() { }

    private Order(Guid id, Guid userId, Guid customerId, List<OrderItem> orderItems, decimal finalPrice)
    {
        Id = id;
        UserId = userId;
        CustomerId = customerId;
        OrderItems = orderItems;
        FinalPrice = finalPrice;
        CreatedAt = DateTime.UtcNow;
    }

    public static Order Create(Guid userId, Guid customerId, List<OrderItem> orderItems, decimal finalPrice)
    {
        if (customerId == Guid.Empty)
        {
            throw new OrderCustomerIdRequiredException();
        }

        if (orderItems == null || orderItems.Count == 0)
        {
            throw new OrderItemsRequiredException();
        }

        if (finalPrice < 0)
        {
            throw new OrderFinalPriceInvalidException();
        }

        return new Order(Guid.NewGuid(), userId, customerId, orderItems, finalPrice);
    }
}