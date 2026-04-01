using IplMerchStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IplMerchStore.Infrastructure.Configurations;

public class FranchiseConfiguration : IEntityTypeConfiguration<Franchise>
{
    public void Configure(EntityTypeBuilder<Franchise> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.ShortCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(f => f.PrimaryColor)
            .IsRequired()
            .HasMaxLength(7); // Hex color

        builder.Property(f => f.SecondaryColor)
            .IsRequired()
            .HasMaxLength(7); // Hex color

        builder.Property(f => f.LogoUrl)
            .HasMaxLength(500);

        // Unique indexes
        builder.HasIndex(f => f.Name)
            .IsUnique()
            .HasDatabaseName("IX_Franchises_Name_Unique");

        builder.HasIndex(f => f.ShortCode)
            .IsUnique()
            .HasDatabaseName("IX_Franchises_ShortCode_Unique");

        builder.HasMany(f => f.Products)
            .WithOne(p => p.Franchise)
            .HasForeignKey(p => p.FranchiseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Franchises");
    }
}
