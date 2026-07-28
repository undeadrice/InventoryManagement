using InventoryManagement.Application.Roles.Services;
using MediatR;

namespace InventoryManagement.Application.Roles.Commands;

public class CreateRoleCommandHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await roleService.Create(request.Name, request.Permissions);
        return result;
    }
}