namespace InventoryManagement.Application.Users.Contracts;

public record CreateUserContract(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Password,
    IReadOnlyCollection<Guid> RoleIds);