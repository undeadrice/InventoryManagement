using InventoryManagement.Domain.Customers;

namespace InventoryManagement.Domain.Orders.Services;

public interface IDiscountCalculator
{
    decimal CalculateDiscount(decimal basePrice, int totalQuantity, CustomerLocation location, DateTime orderDate);
}
