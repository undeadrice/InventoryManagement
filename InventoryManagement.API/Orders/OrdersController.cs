using InventoryManagement.API.Orders.Mappings;
using InventoryManagement.Application.Orders.Commands;
using InventoryManagement.Application.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Orders;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var result = await mediator.Send(new GetOrdersQuery());
        return Ok(result.Select(i => i.MapToOrderResponse()));
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(orderId));
        return Ok(result.MapToOrderResponse());
    }
}
