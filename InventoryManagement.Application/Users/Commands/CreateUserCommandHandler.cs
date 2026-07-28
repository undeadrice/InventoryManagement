using InventoryManagement.Application.Users.Contracts;
using InventoryManagement.Application.Users.Services;
using MediatR;

namespace InventoryManagement.Application.Users.Commands;

public class CreateUserCommandHandler(IUserService userService) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var contract = new CreateUserContract(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Email,
            request.Password,
            request.RoleIds);

        await userService.Create(contract);
    }
}