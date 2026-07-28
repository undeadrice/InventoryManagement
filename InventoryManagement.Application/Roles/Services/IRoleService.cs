using InventoryManagement.Application.Roles.Contracts;
using InventoryManagement.Application.Roles.Dtos;

namespace InventoryManagement.Application.Roles.Services;

public interface IRoleService
{
    Task<IReadOnlyCollection<RoleSimpleDto>> GetAll();

    Task<RoleContract> Get(Guid id);

    Task<Guid> Create(string name, IReadOnlyCollection<string> permissions);
}