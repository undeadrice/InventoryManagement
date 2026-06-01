using InventoryManagement.Domain.Products.Entities;
using InventoryManagement.Domain.Products.Services;
using MediatR;

namespace InventoryManagement.Application.Products.Commands
{
    public class CreateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<CreateProductCommand, Guid>
    {
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = Product.Create(request.Name, request.Description, request.Price, request.Stock);

            await productRepository.Add(product);

            return product.Id;
        }
    }
}