using InventoryManagement.Application.Products.Services;
using InventoryManagement.Application.Products.TransferObjects;
using MediatR;

namespace InventoryManagement.Application.Products.Queries;

internal class GetProductsQueryHandler(IProductReadRepository productReadRepository)
    : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductDto>>
{
    public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await productReadRepository.GetAll(cancellationToken);
    }
}

