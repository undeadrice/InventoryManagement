namespace InventoryManagement.Application.Users.Contracts;

public record UpdateUserBasicDataContract(Guid Id, string Email, string FirstName, string LastName, DateOnly DateOfBirth);