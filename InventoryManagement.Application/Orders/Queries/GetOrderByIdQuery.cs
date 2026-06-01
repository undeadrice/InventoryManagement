using InventoryManagement.Application.Orders.TransferObjects;
using MediatR;

namespace InventoryManagement.Application.Orders.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;
