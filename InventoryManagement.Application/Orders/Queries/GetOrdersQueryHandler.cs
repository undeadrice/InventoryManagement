using InventoryManagement.Application.Orders.Services;
using InventoryManagement.Application.Orders.TransferObjects;
using MediatR;

namespace InventoryManagement.Application.Orders.Queries;

public class GetOrdersQueryHandler(IOrderReadRepository orderReadRepository)
    : IRequestHandler<GetOrdersQuery, IReadOnlyCollection<OrderDto>>
{
    public async Task<IReadOnlyCollection<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await orderReadRepository.GetAll(cancellationToken);
    }
}
