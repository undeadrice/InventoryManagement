using InventoryManagement.Application.Pipeline;
using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Roles.Commands;

[CheckPermission(Permission.RoleCreate)]
public record CreateRoleCommand(string Name, IReadOnlyCollection<string> Permissions) : ICommand<Guid>;