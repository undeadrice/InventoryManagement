namespace InventoryManagement.Application.Users.Contracts;

public record UserWithRolesContract(Guid Id, string Email, string FirstName, string LastName, DateOnly DateOfBirth, IReadOnlyCollection<Guid> RoleIds);