using InventoryManagement.Application.Users.Contracts;

namespace InventoryManagement.Application.Users.Services;

public interface IUserService
{
    Task Create(CreateUserContract user);

    Task<IReadOnlyCollection<UserContract>> GetAll();

    Task<UserWithRolesContract> GetById(Guid id);

    Task UpdateBasicData(UpdateUserBasicDataContract contract);

    Task UpdateRoles(Guid id, IReadOnlyCollection<Guid> roleIds);

    Task ChangePassword(Guid id, string password);
}