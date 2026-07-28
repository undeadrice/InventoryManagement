using InventoryManagement.Application.Users.Contracts;
using InventoryManagement.Application.Users.Services;
using InventoryManagement.Infrastructure.Auth.Entities;
using InventoryManagement.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Auth.Services
{
    internal class UserService(InfraIdentityDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IUserService
    {
        public async Task Create(CreateUserContract user)
        {
            var identityUser = new ApplicationUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                UserName = user.Email,
                Email = user.Email
            };

            var result = await userManager.CreateAsync(identityUser, user.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var roles = await roleManager.Roles
                .Where(r => user.RoleIds.Contains(r.Id))
                .ToListAsync();

            foreach (var role in roles)
            {
                await userManager.AddToRoleAsync(identityUser, role.Name!);
            }
        }

        public async Task<IReadOnlyCollection<UserContract>> GetAll()
        {
            return await dbContext.Users.Select(u => new UserContract(u.Id, u.Email, u.FirstName, u.LastName, u.DateOfBirth)).ToListAsync();
        }

        public async Task<UserWithRolesContract> GetById(Guid Id)
        {
            var user = await userManager.FindByIdAsync(Id.ToString());

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var roleIds = await dbContext.UserRoles
                .Where(ur => ur.UserId == Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            return new UserWithRolesContract(user.Id, user.Email, user.FirstName, user.LastName, user.DateOfBirth, roleIds);
        }

        public async Task UpdateBasicData(UpdateUserBasicDataContract contract)
        {
            var user = await dbContext.Users.FindAsync(contract.Id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            user.Email = contract.Email;
            user.FirstName = contract.FirstName;
            user.LastName = contract.LastName;
            user.DateOfBirth = contract.DateOfBirth;

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRoles(Guid id, IReadOnlyCollection<Guid> roleIds)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user is null)
            {
                throw new NotFoundException($"User with id ${id} doesn't exist");
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                throw new InvalidOperationException(removeResult.Errors.First().Description);
            }

            var roleNames = await roleManager.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name!)
                .ToListAsync();

            if (roleNames.Count != roleIds.Count)
            {
                throw new NotFoundException("One or more roles were not found.");
            }

            var addResult = await userManager.AddToRolesAsync(user, roleNames);

            if (!addResult.Succeeded)
            {
                throw new InvalidOperationException(addResult.Errors.First().Description);
            }
        }

        public async Task ChangePassword(Guid id, string password)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                throw new NotFoundException($"User with id ${id} doesn't exist");
            }

            await userManager.RemovePasswordAsync(user);
            var result = await userManager.AddPasswordAsync(user, password);

            if (!result.Succeeded)
                throw new InvalidOperationException(result.Errors.First().Description);
        }
    }
}