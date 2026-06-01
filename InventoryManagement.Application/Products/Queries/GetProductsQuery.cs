using InventoryManagement.Application.Products.TransferObjects;
using MediatR;

namespace InventoryManagement.Application.Products.Queries;

public record GetProductsQuery() : IRequest<IReadOnlyCollection<ProductDto>>;

