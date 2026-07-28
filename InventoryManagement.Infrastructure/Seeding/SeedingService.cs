using InventoryManagement.Application.Seeding;
using InventoryManagement.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Seeding;

public class SeedingService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    InfraIdentityDbContext dbContext)
    : ISeedingService
{
    public async Task SeedAsync()
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        string[] roles = { "Admin", "User", "Super admin" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }

        var adminUser = new ApplicationUser
        {
            FirstName = "Kamil",
            LastName = "Adminowski",
            DateOfBirth = new DateOnly(1994, 7, 18),
            UserName = "a@a.pl",
            Email = "a@a.pl",
            EmailConfirmed = true
        };

        var adminResult = await userManager.CreateAsync(adminUser, "Admin123!");
        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        var normalUser = new ApplicationUser
        {
            FirstName = "Kamil",
            LastName = "Userski",
            DateOfBirth = new DateOnly(1994, 7, 18),
            UserName = "u@u.pl",
            Email = "u@u.pl",
            EmailConfirmed = true
        };

        var userResult = await userManager.CreateAsync(normalUser, "User123!");
        if (userResult.Succeeded)
        {
            await userManager.AddToRoleAsync(normalUser, "User");
        }

        var superAdminUser = new ApplicationUser
        {
            FirstName = "Super",
            LastName = "Admin",
            DateOfBirth = new DateOnly(1994, 7, 18),
            UserName = "sa@sa.pl",
            Email = "sa@sa.pl",
            EmailConfirmed = true
        };

        var superAdminResult = await userManager.CreateAsync(superAdminUser, "Admin123!");
        if (superAdminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(superAdminUser, "Super admin");
        }
    }
}