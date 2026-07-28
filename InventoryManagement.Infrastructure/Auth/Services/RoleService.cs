using InventoryManagement.Application.Roles.Contracts;
using InventoryManagement.Application.Roles.Dtos;
using InventoryManagement.Application.Roles.Services;
using InventoryManagement.Infrastructure.Auth.Entities;
using InventoryManagement.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryManagement.Infrastructure.Auth.Services;

internal class RoleService(InfraIdentityDbContext dbContext, RoleManager<ApplicationRole> roleManager) : IRoleService
{
    public async Task<IReadOnlyCollection<RoleSimpleDto>> GetAll()
    {
        return await dbContext.Roles.Select(r => new RoleSimpleDto(r.Id, r.Name)).ToListAsync();
    }
    public async Task<RoleContract> Get(Guid id)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());

        if (role == null)
        {
            throw new NotFoundException($"Role with id {id} doesn't exist.");
        }

        var claims = await roleManager.GetClaimsAsync(role);
        var permissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

        return new RoleContract(role.Id, role.Name!, permissions);
    }

    public async Task Update(Guid id, string name, IReadOnlyCollection<string> permissions)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());

        if (role == null)
        {
            throw new NotFoundException($"Role with id {id} doesn't exist.");
        }

        role.Name = name;
        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        var existingClaims = await roleManager.GetClaimsAsync(role);
        var permissionClaims = existingClaims.Where(c => c.Type == "permission").ToList();

        foreach (var claim in permissionClaims)
        {
            await roleManager.RemoveClaimAsync(role, claim);
        }

        foreach (var permission in permissions)
        {
            await roleManager.AddClaimAsync(role, new Claim("permission", permission));
        }
    }

    public async Task<Guid> Create(string name, IReadOnlyCollection<string> permissions)
    {
        var role = new ApplicationRole { Name = name };
        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        foreach (var permission in permissions)
        {
            await roleManager.AddClaimAsync(role, new Claim("permission", permission));
        }

        return role.Id;
    }
}