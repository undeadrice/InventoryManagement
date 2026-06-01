namespace InventoryManagement.Domain.Orders.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public OrderItem() { }
    private OrderItem(Guid id, Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static OrderItem Create(Guid productId, int quantity, decimal unitPrice)
    {
        return new OrderItem(Guid.NewGuid(), Guid.Empty, productId, quantity, unitPrice);
    }

    public void SetOrderId(Guid orderId)
    {
        OrderId = orderId;
    }
}
