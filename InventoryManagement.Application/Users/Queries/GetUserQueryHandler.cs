using InventoryManagement.Application.Users.Contracts;
using InventoryManagement.Application.Users.Services;
using MediatR;

namespace InventoryManagement.Application.Users.Queries;

internal class GetUserQueryHandler(IUserService userService)
    : IRequestHandler<GetUserQuery, UserWithRolesContract>
{
    public async Task<UserWithRolesContract> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetById(request.Id);
    }
}