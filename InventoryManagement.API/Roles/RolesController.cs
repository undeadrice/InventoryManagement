using InventoryManagement.Application.Roles.Commands;
using InventoryManagement.Application.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Roles;

[ApiController]
[Route("api/[controller]")]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var result = await mediator.Send(new GetRoleListQuery());
        return Ok(result);
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await mediator.Send(new GetPermissionsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var result = await mediator.Send(new GetRoleQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateRole(UpdateRoleCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}