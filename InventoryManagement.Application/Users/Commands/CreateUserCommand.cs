using InventoryManagement.Application.Pipeline;

namespace InventoryManagement.Application.Users.Commands;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Password,
    IReadOnlyCollection<Guid> RoleIds) : ICommand;