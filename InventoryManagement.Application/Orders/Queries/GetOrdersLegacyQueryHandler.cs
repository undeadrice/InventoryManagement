using InventoryManagement.Application.Orders.Mapping;
using InventoryManagement.Application.Orders.TransferObjects;
using InventoryManagement.Domain.Orders.Services;
using MediatR;

namespace InventoryManagement.Application.Orders.Queries;

public class GetOrdersLegacyQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrdersLegacyQuery, IReadOnlyCollection<OrderDto>>
{
    public async Task<IReadOnlyCollection<OrderDto>> Handle(GetOrdersLegacyQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetAll(cancellationToken, x => true);
        return orders.Select(x => x.MapToOrderDto()).ToList();
    }
}
