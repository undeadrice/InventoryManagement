using InventoryManagement.Application.Roles.Dtos;

namespace InventoryManagement.Application.Roles.Services;

public interface IRoleService
{
    Task<IReadOnlyCollection<RoleSimpleDto>> GetAll();

    Task<RoleDto> Get(Guid id);

    Task<Guid> Create(string name, IReadOnlyCollection<string> permissions);

    Task Update(Guid id, string name, IReadOnlyCollection<string> permissions);
}
