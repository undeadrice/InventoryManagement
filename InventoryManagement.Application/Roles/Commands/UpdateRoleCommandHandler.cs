using InventoryManagement.Application.Roles.Services;
using MediatR;

namespace InventoryManagement.Application.Roles.Commands;

public class UpdateRoleCommandHandler(IRoleService roleService) : IRequestHandler<UpdateRoleCommand>
{
    public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        await roleService.Update(request.Id, request.Name, request.Permissions);
    }
}