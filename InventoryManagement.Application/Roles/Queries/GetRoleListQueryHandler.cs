using InventoryManagement.Application.Roles.Dtos;
using InventoryManagement.Application.Roles.Services;
using MediatR;

namespace InventoryManagement.Application.Roles.Queries;

internal class GetRoleListQueryHandler(IRoleService roleService)
    : IRequestHandler<GetRoleListQuery, IReadOnlyCollection<RoleSimpleDto>>
{
    public async Task<IReadOnlyCollection<RoleSimpleDto>> Handle(GetRoleListQuery request, CancellationToken cancellationToken)
    {
        return await roleService.GetAll();
    }
}