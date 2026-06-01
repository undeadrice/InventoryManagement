using InventoryManagement.Domain.Products.Exceptions;

namespace InventoryManagement.Domain.Products.Entities;

public class Product
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    private Product(Guid id, string name, string description, decimal price, int stock)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }

    public static Product Create(string name, string description, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ProductNameRequiredException();
        }
     
        if (name.Length > 50)
        {
            throw new ProductNameTooLongException();
        }
          
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ProductDescriptionRequiredException();
        }
           
        if (description.Length > 50)
        {
            throw new ProductDescriptionTooLongException();
        }
         
        if (price <= 0)
        {
            throw new ProductPriceInvalidException();
        }
         
        if (stock < 0)
        {
            throw new ProductStockInvalidException();
        }

        return new Product(Guid.NewGuid(), name, description, price, stock);
    }
}