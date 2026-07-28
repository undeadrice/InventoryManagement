using InventoryManagement.Application.Users.Contracts;
using MediatR;

namespace InventoryManagement.Application.Users.Queries;

public record GetUsersQuery() : IRequest<IReadOnlyCollection<UserContract>>;