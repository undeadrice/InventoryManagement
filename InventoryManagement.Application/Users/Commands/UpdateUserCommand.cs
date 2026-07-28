using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Users.Commands;

[CheckPermission(Permission.UserEdit)]
public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    IReadOnlyCollection<Guid> RoleIds) : ICommand;