using InventoryManagement.Domain.Customers.Services;
using InventoryManagement.Domain.Orders.Services;
using InventoryManagement.Domain.Persistence;
using InventoryManagement.Domain.Products.Services;
using InventoryManagement.Persistence.Customers;
using InventoryManagement.Persistence.Orders;
using InventoryManagement.Persistence.Products;
using InventoryManagement.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Persistence;

public static class DIRegistrations
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PersistenceDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PersistenceConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }
}
