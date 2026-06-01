namespace InventoryManagement.Domain.Customers
{
    public class Customer
    {
        public Guid Id { get; set; }

        public CustomerLocation Location { get; set; }
    }
}
