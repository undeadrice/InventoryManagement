using Dapper;
using InventoryManagement.Application.Products.Services;
using InventoryManagement.Application.Products.TransferObjects;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Products;

public class ProductReadRepository(PersistenceDbContext dbContext) : IProductReadRepository
{
    public async Task<IReadOnlyCollection<ProductDto>> GetAll(CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();

        var rows = await connection.QueryAsync<ProductDto>(
            new CommandDefinition(
                commandText: """
                    SELECT Id, Name, Description, Price, Stock
                    FROM Products
                    ORDER BY Name
                    """,
                cancellationToken: cancellationToken));

        return rows.ToList();
    }
}