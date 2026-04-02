using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IplMerchStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockQuantity",
                table: "Products",
                newName: "InventoryCount");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Products",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "INR");

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_FranchiseId_IsActive",
                table: "Products",
                columns: new[] { "FranchiseId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductType",
                table: "Products",
                column: "ProductType");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            // Seed 70 sample products across 10 franchises
            var now = DateTime.UtcNow;
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Name", "Description", "Price", "Currency", "InventoryCount", "ProductType", "FranchiseId", "ImageUrl", "IsActive", "SKU", "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[,]
                {
                    // CSK (1)
                    { "CSK Premium Jersey", "Official CSK cricket jersey", 3499m, "INR", 50, 1, 1, "https://example.com/csk-jersey.jpg", true, "CSK-JERSEY-001", now, now },
                    { "CSK Yellow Cap", "Official CSK yellow cap", 799m, "INR", 100, 2, 1, "https://example.com/csk-cap.jpg", true, "CSK-CAP-001", now, now },
                    { "CSK Team Flag", "Official CSK team flag", 599m, "INR", 30, 3, 1, "https://example.com/csk-flag.jpg", true, "CSK-FLAG-001", now, now },
                    { "MS Dhoni Autographed Photo", "Signed photo of MS Dhoni", 2999m, "INR", 10, 4, 1, "https://example.com/dhoni-photo.jpg", true, "CSK-AUTO-001", now, now },
                    { "CSK Hoodie", "Official CSK hoodie", 1699m, "INR", 40, 6, 1, "https://example.com/csk-hoodie.jpg", true, "CSK-HOODIE-001", now, now },
                    { "CSK Coffee Mug", "CSK ceramic mug", 399m, "INR", 80, 5, 1, "https://example.com/csk-mug.jpg", true, "CSK-MUG-001", now, now },
                    { "CSK Keychain", "CSK logo keychain", 199m, "INR", 150, 7, 1, "https://example.com/csk-keychain.jpg", true, "CSK-KEY-001", now, now },
                    // MI (2)
                    { "MI Premium Jersey", "Official MI jersey", 3499m, "INR", 60, 1, 2, "https://example.com/mi-jersey.jpg", true, "MI-JERSEY-001", now, now },
                    { "MI Blue Cap", "Official MI cap", 799m, "INR", 120, 2, 2, "https://example.com/mi-cap.jpg", true, "MI-CAP-001", now, now },
                    { "MI Team Flag", "Official MI flag", 599m, "INR", 35, 3, 2, "https://example.com/mi-flag.jpg", true, "MI-FLAG-001", now, now },
                    { "Rohit Sharma Autographed Photo", "Signed photo of Rohit", 3499m, "INR", 8, 4, 2, "https://example.com/rohit-photo.jpg", true, "MI-AUTO-001", now, now },
                    { "MI Hoodie", "Official MI hoodie", 1699m, "INR", 45, 6, 2, "https://example.com/mi-hoodie.jpg", true, "MI-HOODIE-001", now, now },
                    { "MI Coffee Mug", "MI ceramic mug", 399m, "INR", 90, 5, 2, "https://example.com/mi-mug.jpg", true, "MI-MUG-001", now, now },
                    { "MI Keychain", "MI logo keychain", 199m, "INR", 160, 7, 2, "https://example.com/mi-keychain.jpg", true, "MI-KEY-001", now, now },
                    // RCB (3)
                    { "RCB Premium Jersey", "Official RCB jersey", 3499m, "INR", 55, 1, 3, "https://example.com/rcb-jersey.jpg", true, "RCB-JERSEY-001", now, now },
                    { "RCB Red Cap", "Official RCB cap", 799m, "INR", 110, 2, 3, "https://example.com/rcb-cap.jpg", true, "RCB-CAP-001", now, now },
                    { "RCB Team Flag", "Official RCB flag", 599m, "INR", 32, 3, 3, "https://example.com/rcb-flag.jpg", true, "RCB-FLAG-001", now, now },
                    { "Virat Kohli Autographed Photo", "Signed photo of Virat", 4999m, "INR", 5, 4, 3, "https://example.com/virat-photo.jpg", true, "RCB-AUTO-001", now, now },
                    { "RCB Hoodie", "Official RCB hoodie", 1699m, "INR", 42, 6, 3, "https://example.com/rcb-hoodie.jpg", true, "RCB-HOODIE-001", now, now },
                    { "RCB Coffee Mug", "RCB ceramic mug", 399m, "INR", 85, 5, 3, "https://example.com/rcb-mug.jpg", true, "RCB-MUG-001", now, now },
                    { "RCB Keychain", "RCB logo keychain", 199m, "INR", 155, 7, 3, "https://example.com/rcb-keychain.jpg", true, "RCB-KEY-001", now, now },
                    // KKR (4)
                    { "KKR Premium Jersey", "Official KKR jersey", 3499m, "INR", 50, 1, 4, "https://example.com/kkr-jersey.jpg", true, "KKR-JERSEY-001", now, now },
                    { "KKR Purple Cap", "Official KKR cap", 799m, "INR", 105, 2, 4, "https://example.com/kkr-cap.jpg", true, "KKR-CAP-001", now, now },
                    { "KKR Team Flag", "Official KKR flag", 599m, "INR", 30, 3, 4, "https://example.com/kkr-flag.jpg", true, "KKR-FLAG-001", now, now },
                    { "Sunil Narine Autographed Photo", "Signed photo of Narine", 2499m, "INR", 12, 4, 4, "https://example.com/narine-photo.jpg", true, "KKR-AUTO-001", now, now },
                    { "KKR Hoodie", "Official KKR hoodie", 1699m, "INR", 38, 6, 4, "https://example.com/kkr-hoodie.jpg", true, "KKR-HOODIE-001", now, now },
                    { "KKR Coffee Mug", "KKR ceramic mug", 399m, "INR", 75, 5, 4, "https://example.com/kkr-mug.jpg", true, "KKR-MUG-001", now, now },
                    { "KKR Keychain", "KKR logo keychain", 199m, "INR", 145, 7, 4, "https://example.com/kkr-keychain.jpg", true, "KKR-KEY-001", now, now },
                    // SRH (5)
                    { "SRH Premium Jersey", "Official SRH jersey", 3499m, "INR", 52, 1, 5, "https://example.com/srh-jersey.jpg", true, "SRH-JERSEY-001", now, now },
                    { "SRH Orange Cap", "Official SRH cap", 799m, "INR", 115, 2, 5, "https://example.com/srh-cap.jpg", true, "SRH-CAP-001", now, now },
                    { "SRH Team Flag", "Official SRH flag", 599m, "INR", 33, 3, 5, "https://example.com/srh-flag.jpg", true, "SRH-FLAG-001", now, now },
                    { "Kane Williamson Autographed Photo", "Signed photo of Kane", 3299m, "INR", 9, 4, 5, "https://example.com/kane-photo.jpg", true, "SRH-AUTO-001", now, now },
                    { "SRH Hoodie", "Official SRH hoodie", 1699m, "INR", 41, 6, 5, "https://example.com/srh-hoodie.jpg", true, "SRH-HOODIE-001", now, now },
                    { "SRH Coffee Mug", "SRH ceramic mug", 399m, "INR", 82, 5, 5, "https://example.com/srh-mug.jpg", true, "SRH-MUG-001", now, now },
                    { "SRH Keychain", "SRH logo keychain", 199m, "INR", 150, 7, 5, "https://example.com/srh-keychain.jpg", true, "SRH-KEY-001", now, now },
                    // RR (6)
                    { "RR Premium Jersey", "Official RR jersey", 3499m, "INR", 48, 1, 6, "https://example.com/rr-jersey.jpg", true, "RR-JERSEY-001", now, now },
                    { "RR Pink Cap", "Official RR cap", 799m, "INR", 100, 2, 6, "https://example.com/rr-cap.jpg", true, "RR-CAP-001", now, now },
                    { "RR Team Flag", "Official RR flag", 599m, "INR", 28, 3, 6, "https://example.com/rr-flag.jpg", true, "RR-FLAG-001", now, now },
                    { "Sanju Samson Autographed Photo", "Signed photo of Sanju", 2199m, "INR", 14, 4, 6, "https://example.com/sanju-photo.jpg", true, "RR-AUTO-001", now, now },
                    { "RR Hoodie", "Official RR hoodie", 1699m, "INR", 36, 6, 6, "https://example.com/rr-hoodie.jpg", true, "RR-HOODIE-001", now, now },
                    { "RR Coffee Mug", "RR ceramic mug", 399m, "INR", 70, 5, 6, "https://example.com/rr-mug.jpg", true, "RR-MUG-001", now, now },
                    { "RR Keychain", "RR logo keychain", 199m, "INR", 140, 7, 6, "https://example.com/rr-keychain.jpg", true, "RR-KEY-001", now, now },
                    // DC (7)
                    { "DC Premium Jersey", "Official DC jersey", 3499m, "INR", 54, 1, 7, "https://example.com/dc-jersey.jpg", true, "DC-JERSEY-001", now, now },
                    { "DC Blue Cap", "Official DC cap", 799m, "INR", 112, 2, 7, "https://example.com/dc-cap.jpg", true, "DC-CAP-001", now, now },
                    { "DC Team Flag", "Official DC flag", 599m, "INR", 31, 3, 7, "https://example.com/dc-flag.jpg", true, "DC-FLAG-001", now, now },
                    { "Rishabh Pant Autographed Photo", "Signed photo of Pant", 2799m, "INR", 11, 4, 7, "https://example.com/pant-photo.jpg", true, "DC-AUTO-001", now, now },
                    { "DC Hoodie", "Official DC hoodie", 1699m, "INR", 44, 6, 7, "https://example.com/dc-hoodie.jpg", true, "DC-HOODIE-001", now, now },
                    { "DC Coffee Mug", "DC ceramic mug", 399m, "INR", 88, 5, 7, "https://example.com/dc-mug.jpg", true, "DC-MUG-001", now, now },
                    { "DC Keychain", "DC logo keychain", 199m, "INR", 158, 7, 7, "https://example.com/dc-keychain.jpg", true, "DC-KEY-001", now, now },
                    // PBKS (8)
                    { "PBKS Premium Jersey", "Official PBKS jersey", 3499m, "INR", 49, 1, 8, "https://example.com/pbks-jersey.jpg", true, "PBKS-JERSEY-001", now, now },
                    { "PBKS Red Cap", "Official PBKS cap", 799m, "INR", 108, 2, 8, "https://example.com/pbks-cap.jpg", true, "PBKS-CAP-001", now, now },
                    { "PBKS Team Flag", "Official PBKS flag", 599m, "INR", 29, 3, 8, "https://example.com/pbks-flag.jpg", true, "PBKS-FLAG-001", now, now },
                    { "KL Rahul Autographed Photo", "Signed photo of KL", 2399m, "INR", 13, 4, 8, "https://example.com/kl-photo.jpg", true, "PBKS-AUTO-001", now, now },
                    { "PBKS Hoodie", "Official PBKS hoodie", 1699m, "INR", 40, 6, 8, "https://example.com/pbks-hoodie.jpg", true, "PBKS-HOODIE-001", now, now },
                    { "PBKS Coffee Mug", "PBKS ceramic mug", 399m, "INR", 79, 5, 8, "https://example.com/pbks-mug.jpg", true, "PBKS-MUG-001", now, now },
                    { "PBKS Keychain", "PBKS logo keychain", 199m, "INR", 148, 7, 8, "https://example.com/pbks-keychain.jpg", true, "PBKS-KEY-001", now, now },
                    // GT (9)
                    { "GT Premium Jersey", "Official GT jersey", 3499m, "INR", 51, 1, 9, "https://example.com/gt-jersey.jpg", true, "GT-JERSEY-001", now, now },
                    { "GT Gold Cap", "Official GT cap", 799m, "INR", 111, 2, 9, "https://example.com/gt-cap.jpg", true, "GT-CAP-001", now, now },
                    { "GT Team Flag", "Official GT flag", 599m, "INR", 32, 3, 9, "https://example.com/gt-flag.jpg", true, "GT-FLAG-001", now, now },
                    { "Hardik Pandya Autographed Photo", "Signed photo of Hardik", 2899m, "INR", 10, 4, 9, "https://example.com/hardik-photo.jpg", true, "GT-AUTO-001", now, now },
                    { "GT Hoodie", "Official GT hoodie", 1699m, "INR", 43, 6, 9, "https://example.com/gt-hoodie.jpg", true, "GT-HOODIE-001", now, now },
                    { "GT Coffee Mug", "GT ceramic mug", 399m, "INR", 86, 5, 9, "https://example.com/gt-mug.jpg", true, "GT-MUG-001", now, now },
                    { "GT Keychain", "GT logo keychain", 199m, "INR", 153, 7, 9, "https://example.com/gt-keychain.jpg", true, "GT-KEY-001", now, now },
                    // LSG (10)
                    { "LSG Premium Jersey", "Official LSG jersey", 3499m, "INR", 53, 1, 10, "https://example.com/lsg-jersey.jpg", true, "LSG-JERSEY-001", now, now },
                    { "LSG Blue Cap", "Official LSG cap", 799m, "INR", 113, 2, 10, "https://example.com/lsg-cap.jpg", true, "LSG-CAP-001", now, now },
                    { "LSG Team Flag", "Official LSG flag", 599m, "INR", 34, 3, 10, "https://example.com/lsg-flag.jpg", true, "LSG-FLAG-001", now, now },
                    { "LSG Player Autographed Photo", "LSG player signed photo", 2399m, "INR", 13, 4, 10, "https://example.com/lsg-photo.jpg", true, "LSG-AUTO-001", now, now },
                    { "LSG Hoodie", "Official LSG hoodie", 1699m, "INR", 42, 6, 10, "https://example.com/lsg-hoodie.jpg", true, "LSG-HOODIE-001", now, now },
                    { "LSG Coffee Mug", "LSG ceramic mug", 399m, "INR", 84, 5, 10, "https://example.com/lsg-mug.jpg", true, "LSG-MUG-001", now, now },
                    { "LSG Keychain", "LSG logo keychain", 199m, "INR", 151, 7, 10, "https://example.com/lsg-keychain.jpg", true, "LSG-KEY-001", now, now }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_FranchiseId_IsActive",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Name",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductType",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SKU",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "InventoryCount",
                table: "Products",
                newName: "StockQuantity");
        }
    }
}
