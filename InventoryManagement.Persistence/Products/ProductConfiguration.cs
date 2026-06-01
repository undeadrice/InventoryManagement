using InventoryManagement.Domain.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Persistence.Products;

public class RestaurantConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Price)
            .IsRequired();

        builder.Property(r => r.Stock)
            .IsRequired();
    }
}
