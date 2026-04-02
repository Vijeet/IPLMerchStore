using IplMerchStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IplMerchStore.Infrastructure.Configurations;

/// <summary>
/// Fluent API configuration for Product entity
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("INR");

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.ProductType)
            .IsRequired();

        // Unique constraint on SKU
        builder.HasIndex(p => p.SKU)
            .IsUnique();

        // Indexes for filtering and querying
        builder.HasIndex(p => p.FranchiseId);
        builder.HasIndex(p => p.ProductType);
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => new { p.FranchiseId, p.IsActive });

        // Foreign keys and relationships
        builder.HasOne(p => p.Franchise)
            .WithMany(f => f.Products)
            .HasForeignKey(p => p.FranchiseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.CartItems)
            .WithOne(ci => ci.Product)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Products");
    }
}
