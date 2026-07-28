using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Roles.Commands;

[CheckPermission(Permission.RoleEdit)]
public record UpdateRoleCommand(Guid Id, string Name, IReadOnlyCollection<string> Permissions) : ICommand;