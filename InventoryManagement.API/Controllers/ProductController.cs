using InventoryManagement.Application.Products.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IMediator mediator) : ControllerBase
    {
        //[HttpGet]
        //public async Task<IActionResult> GetRestaurants()
        //{
        //    var result = await mediator.Send(new GetRestaurantsQuery());
        //    return Ok(result.Select(i => i.MapToRestaurantSimpleResponse()));
        //}

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
