using Dapper;
using InventoryManagement.Application.Orders.Services;
using InventoryManagement.Application.Orders.TransferObjects;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Orders;

public class OrderReadRepository(PersistenceDbContext dbContext) : IOrderReadRepository
{
    private record OrderItemRow(Guid Id, Guid CustomerId, decimal FinalPrice, DateTime CreatedAt,
        Guid ProductId, int Quantity, decimal UnitPrice);

    public async Task<IReadOnlyCollection<OrderDto>> GetAll(CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();

        var rows = await connection.QueryAsync<OrderItemRow>(
            new CommandDefinition(
                commandText: """
                    SELECT o.Id, o.CustomerId, o.FinalPrice, o.CreatedAt,
                           oi.ProductId, oi.Quantity, oi.UnitPrice
                    FROM Orders o
                    INNER JOIN OrderItems oi ON o.Id = oi.OrderId
                    ORDER BY o.CreatedAt DESC
                    """,
                cancellationToken: cancellationToken));

        return rows
            .GroupBy(r => r.Id)
            .Select(g =>
            {
                var first = g.First();
                return new OrderDto(
                    first.Id,
                    first.CustomerId,
                    g.Select(r => new OrderItemDto(r.ProductId, r.Quantity, r.UnitPrice)).ToList(),
                    first.FinalPrice,
                    first.CreatedAt);
            })
            .ToList();
    }

    public async Task<OrderDto?> FindById(Guid id, CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();

        var rows = (await connection.QueryAsync<OrderItemRow>(
            new CommandDefinition(
                commandText: """
                    SELECT o.Id, o.CustomerId, o.FinalPrice, o.CreatedAt,
                           oi.ProductId, oi.Quantity, oi.UnitPrice
                    FROM Orders o
                    INNER JOIN OrderItems oi ON o.Id = oi.OrderId
                    WHERE o.Id = @Id
                    """,
                parameters: new { Id = id },
                cancellationToken: cancellationToken))).ToList();

        if (rows.Count == 0)
        {
            return null;
        }

        var first = rows.First();
        return new OrderDto(
            first.Id,
            first.CustomerId,
            rows.Select(r => new OrderItemDto(r.ProductId, r.Quantity, r.UnitPrice)).ToList(),
            first.FinalPrice,
            first.CreatedAt);
    }
}
