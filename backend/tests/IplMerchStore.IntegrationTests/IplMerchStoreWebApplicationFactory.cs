using IplMerchStore.Domain.Entities;
using IplMerchStore.Domain.Enums;
using IplMerchStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IplMerchStore.IntegrationTests;

/// <summary>
/// WebApplicationFactory for integration tests
/// </summary>
public class IplMerchStoreWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's DbContext registration
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test database with in-memory SQLite
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=:memory:");
            });

            // Build service provider and create and migrate the database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Ensure the database is deleted and recreated fresh for tests
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Seed test data
            SeedTestData(db);
        });
    }

    private static void SeedTestData(AppDbContext db)
    {
        // Don't check if data exists since we just created the database
        // Just seed the test data directly

        // Create test franchises
        var franchises = new List<Franchise>
        {
            new() { Id = 1, Name = "Chennai Super Kings", ShortCode = "CSK", PrimaryColor = "#FFCC00", SecondaryColor = "#000000", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
            new() { Id = 2, Name = "Mumbai Indians", ShortCode = "MI", PrimaryColor = "#004687", SecondaryColor = "#FFFFFF", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
            new() { Id = 3, Name = "Royal Challengers Bangalore", ShortCode = "RCB", PrimaryColor = "#EC1C24", SecondaryColor = "#000000", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow }
        };

        db.Franchises.AddRange(franchises);
        db.SaveChanges();

        // Create test products
        var now = DateTime.UtcNow;
        var products = new List<Product>
        {
            new() { Id = 1, Name = "CSK Premium Jersey", Description = "Official CSK cricket jersey", Price = 3499m, Currency = "INR", InventoryCount = 50, ProductType = ProductType.Jersey, FranchiseId = 1, ImageUrl = "https://example.com/csk-jersey.jpg", IsActive = true, SKU = "CSK-JERSEY-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 2, Name = "CSK Yellow Cap", Description = "Official CSK cap", Price = 799m, Currency = "INR", InventoryCount = 100, ProductType = ProductType.Cap, FranchiseId = 1, ImageUrl = "https://example.com/csk-cap.jpg", IsActive = true, SKU = "CSK-CAP-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 3, Name = "CSK Team Flag", Description = "Official CSK flag", Price = 599m, Currency = "INR", InventoryCount = 30, ProductType = ProductType.Flag, FranchiseId = 1, ImageUrl = "https://example.com/csk-flag.jpg", IsActive = true, SKU = "CSK-FLAG-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 4, Name = "MS Dhoni Autographed Photo", Description = "Signed photo of MS Dhoni", Price = 2999m, Currency = "INR", InventoryCount = 10, ProductType = ProductType.AutographedPhoto, FranchiseId = 1, ImageUrl = "https://example.com/dhoni-photo.jpg", IsActive = true, SKU = "CSK-AUTO-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 5, Name = "CSK Coffee Mug", Description = "CSK ceramic mug", Price = 399m, Currency = "INR", InventoryCount = 80, ProductType = ProductType.Mug, FranchiseId = 1, ImageUrl = "https://example.com/csk-mug.jpg", IsActive = true, SKU = "CSK-MUG-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 6, Name = "CSK Hoodie", Description = "Official CSK hoodie", Price = 1699m, Currency = "INR", InventoryCount = 40, ProductType = ProductType.Hoodie, FranchiseId = 1, ImageUrl = "https://example.com/csk-hoodie.jpg", IsActive = true, SKU = "CSK-HOODIE-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 7, Name = "CSK Keychain", Description = "CSK logo keychain", Price = 199m, Currency = "INR", InventoryCount = 150, ProductType = ProductType.Keychain, FranchiseId = 1, ImageUrl = "https://example.com/csk-keychain.jpg", IsActive = true, SKU = "CSK-KEY-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 8, Name = "MI Premium Jersey", Description = "Official MI jersey", Price = 3499m, Currency = "INR", InventoryCount = 60, ProductType = ProductType.Jersey, FranchiseId = 2, ImageUrl = "https://example.com/mi-jersey.jpg", IsActive = true, SKU = "MI-JERSEY-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 9, Name = "MI Blue Cap", Description = "Official MI cap", Price = 799m, Currency = "INR", InventoryCount = 120, ProductType = ProductType.Cap, FranchiseId = 2, ImageUrl = "https://example.com/mi-cap.jpg", IsActive = true, SKU = "MI-CAP-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 10, Name = "MI Team Flag", Description = "Official MI flag", Price = 599m, Currency = "INR", InventoryCount = 35, ProductType = ProductType.Flag, FranchiseId = 2, ImageUrl = "https://example.com/mi-flag.jpg", IsActive = true, SKU = "MI-FLAG-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 11, Name = "RCB Premium Jersey", Description = "Official RCB jersey", Price = 3499m, Currency = "INR", InventoryCount = 55, ProductType = ProductType.Jersey, FranchiseId = 3, ImageUrl = "https://example.com/rcb-jersey.jpg", IsActive = true, SKU = "RCB-JERSEY-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 12, Name = "RCB Red Cap", Description = "Official RCB cap", Price = 799m, Currency = "INR", InventoryCount = 110, ProductType = ProductType.Cap, FranchiseId = 3, ImageUrl = "https://example.com/rcb-cap.jpg", IsActive = true, SKU = "RCB-CAP-001", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { Id = 13, Name = "RCB Team Flag", Description = "Official RCB flag", Price = 599m, Currency = "INR", InventoryCount = 32, ProductType = ProductType.Flag, FranchiseId = 3, ImageUrl = "https://example.com/rcb-flag.jpg", IsActive = true, SKU = "RCB-FLAG-001", CreatedAtUtc = now, UpdatedAtUtc = now }
        };

        db.Products.AddRange(products);
        db.SaveChanges();
    }
}
