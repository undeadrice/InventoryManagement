using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Users.Commands;

[CheckPermission(Permission.UserCreate)]
public record CreateUserCommand(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Password,
    IReadOnlyCollection<Guid> RoleIds) : ICommand;