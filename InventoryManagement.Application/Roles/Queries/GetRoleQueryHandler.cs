using InventoryManagement.Application.Roles.Dtos;
using InventoryManagement.Application.Roles.Services;
using MediatR;

namespace InventoryManagement.Application.Roles.Queries;

internal class GetRoleQueryHandler(IRoleService roleService)
    : IRequestHandler<GetRoleQuery, RoleDto>
{
    public async Task<RoleDto> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        return await roleService.Get(request.Id);
    }
}