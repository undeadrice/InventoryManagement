using InventoryManagement.Application.Auth.Models;
using InventoryManagement.Application.Auth.Services;
using InventoryManagement.Application.Roles.Services;
using InventoryManagement.Application.Seeding;
using InventoryManagement.Application.Users.Services;
using InventoryManagement.Infrastructure.Auth.Entities;
using InventoryManagement.Infrastructure.Auth.Services;
using InventoryManagement.Infrastructure.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace InventoryManagement.Infrastructure;

public static class DIRegistrations
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InfraIdentityDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["jwt:Issuer"],
                ValidAudience = configuration["jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:Secret"]!)),
                RoleClaimType = ClaimTypes.Role
            };
        });

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 3;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;
            options.SignIn.RequireConfirmedEmail = false;
        })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<InfraIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISeedingService, SeedingService>();
        services.AddScoped<IRoleService, RoleService>();

        var jwtSettings = new JwtSettings(
            configuration["jwt:Secret"]!,
            configuration["jwt:Issuer"]!,
            configuration["jwt:Audience"]!
        );
        services.AddSingleton(jwtSettings);

        return services;
    }
}