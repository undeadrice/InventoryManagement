using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Auth.Services;

public interface ICurrentUserService
{
    Guid? CurrentUserId { get; }

    Task<bool> IsInRole(UserRole role);

    Task<bool> HasPermissions(params Permission[] permissions);
}