using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Pipeline;

public class CheckRoleAttribute : Attribute
{
    public UserRole[] Roles { get; set; }

    public CheckRoleAttribute(params UserRole[] roles)
    {
        Roles = roles;
    }
}