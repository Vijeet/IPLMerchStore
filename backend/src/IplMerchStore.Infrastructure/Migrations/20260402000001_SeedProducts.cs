using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IplMerchStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = DateTime.UtcNow;

            // Array of products for all 10 franchises
            var products = new object[,]
            {
                // Chennai Super Kings (FranchiseId = 1)
                { "CSK Premium Jersey", "The official CSK cricket jersey with embroidered logo and player number", 3499m, "INR", 50, 1, 1, "https://example.com/images/csk-jersey.jpg", true, "CSK-JERSEY-001" },
                { "CSK Yellow Cap", "Official CSK yellow cap with embroidered logo", 799m, "INR", 100, 2, 1, "https://example.com/images/csk-cap.jpg", true, "CSK-CAP-001" },
                { "CSK Team Flag", "Official CSK team flag (3x2 feet)", 599m, "INR", 30, 3, 1, "https://example.com/images/csk-flag.jpg", true, "CSK-FLAG-001" },
                { "MS Dhoni Autographed Photo", "Autographed photo of MS Dhoni (8x10 inches)", 2999m, "INR", 10, 4, 1, "https://example.com/images/csk-dhoni-photo.jpg", true, "CSK-AUTO-001" },
                { "CSK Hoodie", "Official CSK hoodie with full front print", 1699m, "INR", 40, 6, 1, "https://example.com/images/csk-hoodie.jpg", true, "CSK-HOODIE-001" },
                { "CSK Coffee Mug", "CSK branded ceramic coffee mug 300ml", 399m, "INR", 80, 5, 1, "https://example.com/images/csk-mug.jpg", true, "CSK-MUG-001" },
                { "CSK Keychain", "CSK logo keychain with metal construction", 199m, "INR", 150, 7, 1, "https://example.com/images/csk-keychain.jpg", true, "CSK-KEY-001" },

                // Mumbai Indians (FranchiseId = 2)
                { "MI Premium Jersey", "Official MI cricket jersey with embroidered logo", 3499m, "INR", 60, 1, 2, "https://example.com/images/mi-jersey.jpg", true, "MI-JERSEY-001" },
                { "MI Blue Cap", "Official MI blue cap with logo", 799m, "INR", 120, 2, 2, "https://example.com/images/mi-cap.jpg", true, "MI-CAP-001" },
                { "MI Team Flag", "Official MI team flag (3x2 feet)", 599m, "INR", 35, 3, 2, "https://example.com/images/mi-flag.jpg", true, "MI-FLAG-001" },
                { "Rohit Sharma Autographed Photo", "Autographed photo of Rohit Sharma (8x10 inches)", 3499m, "INR", 8, 4, 2, "https://example.com/images/mi-rohit-photo.jpg", true, "MI-AUTO-001" },
                { "MI Hoodie", "Official MI hoodie with full front print", 1699m, "INR", 45, 6, 2, "https://example.com/images/mi-hoodie.jpg", true, "MI-HOODIE-001" },
                { "MI Coffee Mug", "MI branded ceramic coffee mug 300ml", 399m, "INR", 90, 5, 2, "https://example.com/images/mi-mug.jpg", true, "MI-MUG-001" },
                { "MI Keychain", "MI logo keychain with metal construction", 199m, "INR", 160, 7, 2, "https://example.com/images/mi-keychain.jpg", true, "MI-KEY-001" },

                // Royal Challengers Bangalore (FranchiseId = 3)
                { "RCB Premium Jersey", "Official RCB red cricket jersey", 3499m, "INR", 55, 1, 3, "https://example.com/images/rcb-jersey.jpg", true, "RCB-JERSEY-001" },
                { "RCB Red Cap", "Official RCB red cap with logo", 799m, "INR", 110, 2, 3, "https://example.com/images/rcb-cap.jpg", true, "RCB-CAP-001" },
                { "RCB Team Flag", "Official RCB team flag (3x2 feet)", 599m, "INR", 32, 3, 3, "https://example.com/images/rcb-flag.jpg", true, "RCB-FLAG-001" },
                { "Virat Kohli Autographed Photo", "Autographed photo of Virat Kohli (8x10 inches)", 4999m, "INR", 5, 4, 3, "https://example.com/images/rcb-virat-photo.jpg", true, "RCB-AUTO-001" },
                { "RCB Hoodie", "Official RCB hoodie with full front print", 1699m, "INR", 42, 6, 3, "https://example.com/images/rcb-hoodie.jpg", true, "RCB-HOODIE-001" },
                { "RCB Coffee Mug", "RCB branded ceramic coffee mug 300ml", 399m, "INR", 85, 5, 3, "https://example.com/images/rcb-mug.jpg", true, "RCB-MUG-001" },
                { "RCB Keychain", "RCB logo keychain with metal construction", 199m, "INR", 155, 7, 3, "https://example.com/images/rcb-keychain.jpg", true, "RCB-KEY-001" },

                // Kolkata Knight Riders (FranchiseId = 4)
                { "KKR Premium Jersey", "Official KKR purple cricket jersey", 3499m, "INR", 50, 1, 4, "https://example.com/images/kkr-jersey.jpg", true, "KKR-JERSEY-001" },
                { "KKR Purple Cap", "Official KKR purple cap with logo", 799m, "INR", 105, 2, 4, "https://example.com/images/kkr-cap.jpg", true, "KKR-CAP-001" },
                { "KKR Team Flag", "Official KKR team flag (3x2 feet)", 599m, "INR", 30, 3, 4, "https://example.com/images/kkr-flag.jpg", true, "KKR-FLAG-001" },
                { "Sunil Narine Autographed Photo", "Autographed photo of Sunil Narine (8x10 inches)", 2499m, "INR", 12, 4, 4, "https://example.com/images/kkr-narine-photo.jpg", true, "KKR-AUTO-001" },
                { "KKR Hoodie", "Official KKR hoodie with full front print", 1699m, "INR", 38, 6, 4, "https://example.com/images/kkr-hoodie.jpg", true, "KKR-HOODIE-001" },
                { "KKR Coffee Mug", "KKR branded ceramic coffee mug 300ml", 399m, "INR", 75, 5, 4, "https://example.com/images/kkr-mug.jpg", true, "KKR-MUG-001" },
                { "KKR Keychain", "KKR logo keychain with metal construction", 199m, "INR", 145, 7, 4, "https://example.com/images/kkr-keychain.jpg", true, "KKR-KEY-001" },

                // Sunrisers Hyderabad (FranchiseId = 5)
                { "SRH Premium Jersey", "Official SRH orange cricket jersey", 3499m, "INR", 52, 1, 5, "https://example.com/images/srh-jersey.jpg", true, "SRH-JERSEY-001" },
                { "SRH Orange Cap", "Official SRH orange cap with logo", 799m, "INR", 115, 2, 5, "https://example.com/images/srh-cap.jpg", true, "SRH-CAP-001" },
                { "SRH Team Flag", "Official SRH team flag (3x2 feet)", 599m, "INR", 33, 3, 5, "https://example.com/images/srh-flag.jpg", true, "SRH-FLAG-001" },
                { "Kane Williamson Autographed Photo", "Autographed photo of Kane Williamson (8x10 inches)", 3299m, "INR", 9, 4, 5, "https://example.com/images/srh-kane-photo.jpg", true, "SRH-AUTO-001" },
                { "SRH Hoodie", "Official SRH hoodie with full front print", 1699m, "INR", 41, 6, 5, "https://example.com/images/srh-hoodie.jpg", true, "SRH-HOODIE-001" },
                { "SRH Coffee Mug", "SRH branded ceramic coffee mug 300ml", 399m, "INR", 82, 5, 5, "https://example.com/images/srh-mug.jpg", true, "SRH-MUG-001" },
                { "SRH Keychain", "SRH logo keychain with metal construction", 199m, "INR", 150, 7, 5, "https://example.com/images/srh-keychain.jpg", true, "SRH-KEY-001" },

                // Rajasthan Royals (FranchiseId = 6)
                { "RR Premium Jersey", "Official RR pink cricket jersey", 3499m, "INR", 48, 1, 6, "https://example.com/images/rr-jersey.jpg", true, "RR-JERSEY-001" },
                { "RR Pink Cap", "Official RR pink cap with logo", 799m, "INR", 100, 2, 6, "https://example.com/images/rr-cap.jpg", true, "RR-CAP-001" },
                { "RR Team Flag", "Official RR team flag (3x2 feet)", 599m, "INR", 28, 3, 6, "https://example.com/images/rr-flag.jpg", true, "RR-FLAG-001" },
                { "Sanju Samson Autographed Photo", "Autographed photo of Sanju Samson (8x10 inches)", 2199m, "INR", 14, 4, 6, "https://example.com/images/rr-sanju-photo.jpg", true, "RR-AUTO-001" },
                { "RR Hoodie", "Official RR hoodie with full front print", 1699m, "INR", 36, 6, 6, "https://example.com/images/rr-hoodie.jpg", true, "RR-HOODIE-001" },
                { "RR Coffee Mug", "RR branded ceramic coffee mug 300ml", 399m, "INR", 70, 5, 6, "https://example.com/images/rr-mug.jpg", true, "RR-MUG-001" },
                { "RR Keychain", "RR logo keychain with metal construction", 199m, "INR", 140, 7, 6, "https://example.com/images/rr-keychain.jpg", true, "RR-KEY-001" },

                // Delhi Capitals (FranchiseId = 7)
                { "DC Premium Jersey", "Official DC blue cricket jersey", 3499m, "INR", 54, 1, 7, "https://example.com/images/dc-jersey.jpg", true, "DC-JERSEY-001" },
                { "DC Blue Cap", "Official DC blue cap with logo", 799m, "INR", 112, 2, 7, "https://example.com/images/dc-cap.jpg", true, "DC-CAP-001" },
                { "DC Team Flag", "Official DC team flag (3x2 feet)", 599m, "INR", 31, 3, 7, "https://example.com/images/dc-flag.jpg", true, "DC-FLAG-001" },
                { "Rishabh Pant Autographed Photo", "Autographed photo of Rishabh Pant (8x10 inches)", 2799m, "INR", 11, 4, 7, "https://example.com/images/dc-rishabh-photo.jpg", true, "DC-AUTO-001" },
                { "DC Hoodie", "Official DC hoodie with full front print", 1699m, "INR", 44, 6, 7, "https://example.com/images/dc-hoodie.jpg", true, "DC-HOODIE-001" },
                { "DC Coffee Mug", "DC branded ceramic coffee mug 300ml", 399m, "INR", 88, 5, 7, "https://example.com/images/dc-mug.jpg", true, "DC-MUG-001" },
                { "DC Keychain", "DC logo keychain with metal construction", 199m, "INR", 158, 7, 7, "https://example.com/images/dc-keychain.jpg", true, "DC-KEY-001" },

                // Punjab Kings (FranchiseId = 8)
                { "PBKS Premium Jersey", "Official Punjab Kings red cricket jersey", 3499m, "INR", 49, 1, 8, "https://example.com/images/pbks-jersey.jpg", true, "PBKS-JERSEY-001" },
                { "PBKS Red Cap", "Official Punjab Kings red cap with logo", 799m, "INR", 108, 2, 8, "https://example.com/images/pbks-cap.jpg", true, "PBKS-CAP-001" },
                { "PBKS Team Flag", "Official Punjab Kings team flag (3x2 feet)", 599m, "INR", 29, 3, 8, "https://example.com/images/pbks-flag.jpg", true, "PBKS-FLAG-001" },
                { "KL Rahul Autographed Photo", "Autographed photo of KL Rahul (8x10 inches)", 2399m, "INR", 13, 4, 8, "https://example.com/images/pbks-kl-photo.jpg", true, "PBKS-AUTO-001" },
                { "PBKS Hoodie", "Official Punjab Kings hoodie with full front print", 1699m, "INR", 40, 6, 8, "https://example.com/images/pbks-hoodie.jpg", true, "PBKS-HOODIE-001" },
                { "PBKS Coffee Mug", "PBKS branded ceramic coffee mug 300ml", 399m, "INR", 79, 5, 8, "https://example.com/images/pbks-mug.jpg", true, "PBKS-MUG-001" },
                { "PBKS Keychain", "PBKS logo keychain with metal construction", 199m, "INR", 148, 7, 8, "https://example.com/images/pbks-keychain.jpg", true, "PBKS-KEY-001" },

                // Gujarat Titans (FranchiseId = 9)
                { "GT Premium Jersey", "Official Gujarat Titans cricket jersey", 3499m, "INR", 51, 1, 9, "https://example.com/images/gt-jersey.jpg", true, "GT-JERSEY-001" },
                { "GT Gold Cap", "Official Gujarat Titans gold cap with logo", 799m, "INR", 111, 2, 9, "https://example.com/images/gt-cap.jpg", true, "GT-CAP-001" },
                { "GT Team Flag", "Official Gujarat Titans team flag (3x2 feet)", 599m, "INR", 32, 3, 9, "https://example.com/images/gt-flag.jpg", true, "GT-FLAG-001" },
                { "Hardik Pandya Autographed Photo", "Autographed photo of Hardik Pandya (8x10 inches)", 2899m, "INR", 10, 4, 9, "https://example.com/images/gt-hardik-photo.jpg", true, "GT-AUTO-001" },
                { "GT Hoodie", "Official Gujarat Titans hoodie with full front print", 1699m, "INR", 43, 6, 9, "https://example.com/images/gt-hoodie.jpg", true, "GT-HOODIE-001" },
                { "GT Coffee Mug", "GT branded ceramic coffee mug 300ml", 399m, "INR", 86, 5, 9, "https://example.com/images/gt-mug.jpg", true, "GT-MUG-001" },
                { "GT Keychain", "GT logo keychain with metal construction", 199m, "INR", 153, 7, 9, "https://example.com/images/gt-keychain.jpg", true, "GT-KEY-001" },

                // Lucknow Super Giants (FranchiseId = 10)
                { "LSG Premium Jersey", "Official Lucknow Super Giants cricket jersey", 3499m, "INR", 53, 1, 10, "https://example.com/images/lsg-jersey.jpg", true, "LSG-JERSEY-001" },
                { "LSG Blue Cap", "Official Lucknow Super Giants blue cap", 799m, "INR", 113, 2, 10, "https://example.com/images/lsg-cap.jpg", true, "LSG-CAP-001" },
                { "LSG Team Flag", "Official Lucknow Super Giants team flag (3x2 feet)", 599m, "INR", 34, 3, 10, "https://example.com/images/lsg-flag.jpg", true, "LSG-FLAG-001" },
                { "KL Rahul LSG Autographed Photo", "Autographed photo of KL Rahul with LSG (8x10 inches)", 2399m, "INR", 13, 4, 10, "https://example.com/images/lsg-kl-photo.jpg", true, "LSG-AUTO-001" },
                { "LSG Hoodie", "Official Lucknow Super Giants hoodie", 1699m, "INR", 42, 6, 10, "https://example.com/images/lsg-hoodie.jpg", true, "LSG-HOODIE-001" },
                { "LSG Coffee Mug", "LSG branded ceramic coffee mug 300ml", 399m, "INR", 84, 5, 10, "https://example.com/images/lsg-mug.jpg", true, "LSG-MUG-001" },
                { "LSG Keychain", "LSG logo keychain with metal construction", 199m, "INR", 151, 7, 10, "https://example.com/images/lsg-keychain.jpg", true, "LSG-KEY-001" }
            };

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Name", "Description", "Price", "Currency", "InventoryCount", "ProductType", "FranchiseId", "ImageUrl", "IsActive", "SKU", "CreatedAtUtc", "UpdatedAtUtc" },
                values: products.Cast<object[]>().Select((row, index) => new object[]
                {
                    row[0], // Name
                    row[1], // Description
                    row[2], // Price
                    row[3], // Currency
                    row[4], // InventoryCount
                    row[5], // ProductType
                    row[6], // FranchiseId
                    row[7], // ImageUrl
                    row[8], // IsActive
                    row[9], // SKU
                    now,    // CreatedAtUtc
                    now     // UpdatedAtUtc
                }).ToArray());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete all products created by this migration
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: null);
        }
    }
}
