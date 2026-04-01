using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IplMerchStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedFranchises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = DateTime.UtcNow;

            migrationBuilder.InsertData(
                table: "Franchises",
                columns: new[] { "Name", "ShortCode", "PrimaryColor", "SecondaryColor", "LogoUrl", "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { "Chennai Super Kings", "CSK", "#FFCC00", "#000080", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Mumbai Indians", "MI", "#004687", "#0066ff", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Royal Challengers Bangalore", "RCB", "#EC1C24", "#000000", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Kolkata Knight Riders", "KKR", "#3A225D", "#FFD700", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Sunrisers Hyderabad", "SRH", "#FF6B00", "#000000", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Rajasthan Royals", "RR", "#EC1C24", "#3A4697", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Delhi Capitals", "DC", "#003582", "#0066ff", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Punjab Kings", "PBKS", "#AE1230", "#FFD700", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Gujarat Titans", "GT", "#37474F", "#FFD700", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now },
                    { "Lucknow Super Giants", "LSG", "#0066ff", "#FFD700", "https://images.unsplash.com/photo-1639952048313-e0c7c30f3567", now, now }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Franchises",
                keyColumn: "Id",
                keyValues: new object[] { new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } });
        }
    }
}
