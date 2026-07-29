using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Auth.Services;

public interface ICurrentUserService
{
    Guid? CurrentUserId { get; }

    bool IsAuthenticated { get; }

    Task<bool> IsInRole(UserRole role);

    Task<bool> HasPermissions(params Permission[] permissions);

    Task<bool> IsSuperAdmin();
}