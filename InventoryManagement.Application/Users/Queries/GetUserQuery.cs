using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Users.Contracts;
using InventoryManagement.Application.Users.Enums;
using MediatR;

namespace InventoryManagement.Application.Users.Queries;

[CheckRole(UserRole.Admin)]
public record GetUserQuery(Guid Id) : IRequest<UserWithRolesContract>;