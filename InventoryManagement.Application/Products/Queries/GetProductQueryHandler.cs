using InventoryManagement.Application.Products.Mapping;
using InventoryManagement.Application.Products.TransferObjects;
using InventoryManagement.Domain.Products.Services;
using MediatR;

namespace InventoryManagement.Application.Products.Queries;

    internal class GetRestaurantsQueryHandler(IProductRepository productRepository)
        : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductDto>>
    {
        public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var restaurants = await productRepository.GetAll(cancellationToken, x => true);
            return restaurants.Select(x => x.MapToProductDto()).ToList();
        }
    }

