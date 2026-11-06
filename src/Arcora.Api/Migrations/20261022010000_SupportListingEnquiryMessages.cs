using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261022010000_SupportListingEnquiryMessages")]
public sealed class SupportListingEnquiryMessages : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Message", table: "ConversationMessages", type: "nvarchar(max)", nullable: false,
            oldClrType: typeof(string), oldType: "nvarchar(256)", oldMaxLength: 256);
        migrationBuilder.AlterColumn<string>(
            name: "StorageProvider", table: "ListingPhotos", type: "nvarchar(100)", maxLength: 100, nullable: true,
            oldClrType: typeof(string), oldType: "nvarchar(100)", oldMaxLength: 100);
        migrationBuilder.AlterColumn<string>(
            name: "StorageReference", table: "ListingPhotos", type: "nvarchar(500)", maxLength: 500, nullable: true,
            oldClrType: typeof(string), oldType: "nvarchar(500)", oldMaxLength: 500);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Do not truncate saved enquiry cards or invent blob metadata during rollback.
        throw new NotSupportedException("This migration requires a reviewed data-preserving rollback.");
    }
}
