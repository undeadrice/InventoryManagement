using InventoryManagement.Domain.Customers;
using InventoryManagement.Domain.Orders;
using InventoryManagement.Domain.Products.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence;

public class PersistenceDbContext(DbContextOptions<PersistenceDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceDbContext).Assembly);
    }
}

