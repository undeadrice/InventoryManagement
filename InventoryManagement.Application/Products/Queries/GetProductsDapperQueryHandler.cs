using InventoryManagement.Application.Products.Services;
using InventoryManagement.Application.Products.TransferObjects;
using MediatR;

namespace InventoryManagement.Application.Products.Queries;

internal class GetProductsDapperQueryHandler(IProductDapperReadRepository productDapperReadRepository)
    : IRequestHandler<GetProductsDapperQuery, IReadOnlyCollection<ProductDto>>
{
    public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsDapperQuery request, CancellationToken cancellationToken)
    {
        return await productDapperReadRepository.GetAll(cancellationToken);
    }
}