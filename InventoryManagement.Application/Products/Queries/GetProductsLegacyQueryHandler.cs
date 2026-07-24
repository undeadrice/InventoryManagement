using InventoryManagement.Application.Products.Mapping;
using InventoryManagement.Application.Products.TransferObjects;
using InventoryManagement.Domain.Products.Services;
using MediatR;

namespace InventoryManagement.Application.Products.Queries;

public class GetProductsLegacyQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsLegacyQuery, IReadOnlyCollection<ProductDto>>
{
    public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsLegacyQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAll(cancellationToken, x => true);
        return products.Select(x => x.MapToProductDto()).ToList();
    }
}