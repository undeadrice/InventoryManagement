namespace InventoryManagement.Application.Roles.Contracts;

public record RoleContract(Guid Id, string Name, IReadOnlyCollection<string> Permissions);