using InventoryManagement.Application.Auth.Services;
using InventoryManagement.Application.Users.Enums;
using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Infrastructure.Auth.Services;

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? CurrentUserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim is not null ? Guid.Parse(userIdClaim) : null;
        }
    }

    public async Task<bool> IsInRole(UserRole role)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null) return false;

        return await Task.FromResult(user.IsInRole(role.ToString()));
    }

    public async Task<bool> HasPermissions(params Permission[] permissions)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null) return false;

        foreach (var permission in permissions)
        {
            if (!user.HasClaim("permission", permission.ToString()))
            {
                return false;
            }
        }

        return true;
    }
}