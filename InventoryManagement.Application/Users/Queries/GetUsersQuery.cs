using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Users.Contracts;
using InventoryManagement.Application.Users.Enums;
using MediatR;

namespace InventoryManagement.Application.Users.Queries;

[CheckPermission(Permission.UserView)]
public record GetUsersQuery() : IRequest<IReadOnlyCollection<UserContract>>;
