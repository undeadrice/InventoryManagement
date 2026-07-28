using InventoryManagement.Application.Auth.Services;
using InventoryManagement.Application.Users.Enums;
using InventoryManagement.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Auth.Services;

internal class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    InfraIdentityDbContext dbContext) : ICurrentUserService
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
        var userId = CurrentUserId;
        if (userId is null) return false;

        var user = await userManager.FindByIdAsync(userId.Value.ToString());
        if (user is null) return false;

        var roleNames = await userManager.GetRolesAsync(user);

        var roleIds = await roleManager.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        var rolePermissionClaims = await dbContext.RoleClaims
            .Where(rc => roleIds.Contains(rc.RoleId) && rc.ClaimType == "permission")
            .Select(rc => rc.ClaimValue)
            .ToListAsync();

        foreach (var permission in permissions)
        {
            if (!rolePermissionClaims.Contains(permission.ToString()))
            {
                return false;
            }
        }

        return true;
    }
}