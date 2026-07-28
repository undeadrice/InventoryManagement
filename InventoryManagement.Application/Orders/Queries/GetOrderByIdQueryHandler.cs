using InventoryManagement.Application.Orders.Services;
using InventoryManagement.Application.Orders.TransferObjects;
using InventoryManagement.Shared.Exceptions;
using MediatR;

namespace InventoryManagement.Application.Orders.Queries;

public class GetOrderByIdQueryHandler(IOrderReadRepository orderReadRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderReadRepository.FindById(request.OrderId, cancellationToken);
        return order ?? throw new NotFoundException($"Order with id {request.OrderId} doesn't exist");
    }
}
