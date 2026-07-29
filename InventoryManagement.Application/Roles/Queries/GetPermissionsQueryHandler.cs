using InventoryManagement.Application.Roles.Dtos;
using MediatR;

namespace InventoryManagement.Application.Roles.Queries;

internal class GetPermissionsQueryHandler
    : IRequestHandler<GetPermissionsQuery, IReadOnlyCollection<PermissionGroupDto>>
{
    private static readonly IReadOnlyCollection<PermissionGroupDto> _groups =
    [
        new PermissionGroupDto("Role", ["RoleCreate", "RoleEdit", "RoleDelete", "RoleView"]),
        new PermissionGroupDto("User", ["UserCreate", "UserEdit", "UserDelete", "UserView"]),
        new PermissionGroupDto("Permissions", ["PermissionView"]),
    ];

    public Task<IReadOnlyCollection<PermissionGroupDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_groups);
    }
}