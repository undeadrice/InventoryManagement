using InventoryManagement.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.IntegrationTests.Infrastructure;

public class InventoryWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "InventoryTestDb_" + Guid.NewGuid().ToString("N");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(PersistenceDbContext) ||
                    d.ServiceType == typeof(DbContextOptions<PersistenceDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType.FullName?.StartsWith("Microsoft.EntityFrameworkCore") == true &&
                    d.ServiceType.FullName.Contains("PersistenceDbContext"))
                .ToList();

            foreach (var d in toRemove)
            {
                services.Remove(d);
            }

            var optionsConfigType = typeof(IDbContextOptionsConfiguration<PersistenceDbContext>);
            var optionsConfigs = services
                .Where(d => d.ServiceType == optionsConfigType)
                .ToList();

            foreach (var d in optionsConfigs)
            {
                services.Remove(d);
            }

            var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={_dbName};Trusted_Connection=True;MultipleActiveResultSets=true";

            services.AddDbContext<PersistenceDbContext>(options =>
                options.UseSqlServer(connectionString));
        });

        builder.UseEnvironment("Development");
    }

    public async Task CreateDatabase()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PersistenceDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DeleteDatabase()
    {
        try
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PersistenceDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}
