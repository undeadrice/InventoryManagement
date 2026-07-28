using InventoryManagement.Application.Auth.Models;
using InventoryManagement.Application.Auth.Services;
using InventoryManagement.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryManagement.Infrastructure.Auth.Services;

internal class AuthService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings) : IAuthService
{
    public async Task<string> Login(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null || !await userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var roles = await userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }
}