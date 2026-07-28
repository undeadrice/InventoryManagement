using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Pipeline
{
    public class CheckPermissionAttribute : Attribute
    {
        public Permission[] Permissions { get; set; }

        public CheckPermissionAttribute(params Permission[] permissions)
        {
            Permissions = permissions;
        }
    }
}