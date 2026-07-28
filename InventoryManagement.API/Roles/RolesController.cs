using InventoryManagement.Application.Roles.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Roles;

[ApiController]
[Route("api/[controller]")]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, UpdateRoleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match command id.");
        }

        await mediator.Send(command);
        return Ok();
    }
}