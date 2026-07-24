using InventoryManagement.Application.Orders.TransferObjects;
using MediatR;

namespace InventoryManagement.Application.Orders.Queries;

public record GetOrdersLegacyQuery() : IRequest<IReadOnlyCollection<OrderDto>>;
