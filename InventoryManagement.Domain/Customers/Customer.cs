namespace InventoryManagement.Domain.Customers;

public class Customer
{
    public Guid Id { get; private set; }

    public CustomerLocation Location { get; private set; }

    public Customer() { }

    private Customer(Guid id, CustomerLocation location)
    {
        Id = id;
        Location = location;
    }

    public static Customer Create(CustomerLocation location)
    {
        return new Customer(Guid.NewGuid(), location);
    }
}
