using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Roles.Dtos;
using InventoryManagement.Application.Users.Enums;
using MediatR;

namespace InventoryManagement.Application.Roles.Queries;

[CheckRole(UserRole.Admin)]
public record GetRoleQuery(Guid Id) : IRequest<RoleDto>;
