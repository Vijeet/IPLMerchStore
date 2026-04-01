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
            .HasMaxLength(10);

        builder.Property(f => f.City)
            .HasMaxLength(50);

        builder.Property(f => f.LogoUrl)
            .HasMaxLength(500);

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        builder.HasMany(f => f.Products)
            .WithOne(p => p.Franchise)
            .HasForeignKey(p => p.FranchiseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Franchises");
    }
}
