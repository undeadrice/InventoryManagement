namespace InventoryManagement.Application.Users.Contracts;

public record UserContract(Guid Id, string Email, string FirstName, string LastName, DateOnly DateOfBirth);