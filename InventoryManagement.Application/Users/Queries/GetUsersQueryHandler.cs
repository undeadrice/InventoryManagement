using InventoryManagement.Application.Users.Contracts;
using InventoryManagement.Application.Users.Services;
using MediatR;

namespace InventoryManagement.Application.Users.Queries;

internal class GetUsersQueryHandler(IUserService userService)
    : IRequestHandler<GetUsersQuery, IReadOnlyCollection<UserContract>>
{
    public async Task<IReadOnlyCollection<UserContract>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetAll();
    }
}