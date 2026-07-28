using InventoryManagement.Application.Users.Commands;
using InventoryManagement.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var result = await mediator.Send(new GetUsersQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await mediator.Send(new GetUserQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}
