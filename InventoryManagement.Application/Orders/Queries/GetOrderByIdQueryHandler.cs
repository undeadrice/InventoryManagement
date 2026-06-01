using InventoryManagement.Application.Orders.Mapping;
using InventoryManagement.Application.Orders.TransferObjects;
using InventoryManagement.Domain.Orders.Services;
using MediatR;

namespace InventoryManagement.Application.Orders.Queries;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetById(request.OrderId, cancellationToken);
        return order.MapToOrderDto();
    }
}
