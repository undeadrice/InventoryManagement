namespace InventoryManagement.Application.Roles.Dtos;

public record RoleDto(Guid Id, string Name, IReadOnlyCollection<string> Permissions);