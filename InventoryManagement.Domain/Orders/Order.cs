namespace InventoryManagement.Domain.Orders;

public class Order
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public decimal FinalPrice { get; set; }
}

