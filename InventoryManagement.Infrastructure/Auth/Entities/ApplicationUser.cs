using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Infrastructure.Auth.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [Required]
    public override string Email
    {
        get => base.Email!;
        set => base.Email = value;
    }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required DateOnly DateOfBirth { get; set; }
}