namespace InventoryManagement.Application.Roles.Dtos;

public record PermissionGroupDto(string GroupName, IReadOnlyCollection<string> Permissions);