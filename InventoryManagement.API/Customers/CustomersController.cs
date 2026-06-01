using InventoryManagement.Application.Customers.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Customers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
