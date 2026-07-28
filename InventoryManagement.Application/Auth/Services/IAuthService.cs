namespace InventoryManagement.Application.Auth.Services;

public interface IAuthService
{
    Task<string> Login(string email, string password);
}