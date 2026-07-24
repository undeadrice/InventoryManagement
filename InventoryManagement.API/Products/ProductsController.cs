using InventoryManagement.API.Products.Mappings;
using InventoryManagement.Application.Products.Commands;
using InventoryManagement.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Products
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        [HttpGet] // Dapper - lightweight read
        public async Task<IActionResult> GetProducts()
        {
            var result = await mediator.Send(new GetProductsQuery());
            return Ok(result.Select(i => i.MapToProductResponse()));
        }

        [HttpGet("legacy")] // EF Core - loads full aggregate
        public async Task<IActionResult> GetProductsLegacy()
        {
            var result = await mediator.Send(new GetProductsLegacyQuery());
            return Ok(result.Select(i => i.MapToProductResponse()));
        }

        [HttpPost("seed/{quantity:int}")]
        public async Task<IActionResult> SeedProducts(int quantity)
        {
            var result = await mediator.Send(new SeedProductsCommand(quantity));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
