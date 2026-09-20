using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    public partial class AddListingCancellationPoliciesAndExpandAccessInstructions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortTermCancellationPolicy",
                table: "Listings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LongTermCancellationPolicy",
                table: "Listings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "ListingAccessInstructions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "ListingAccessInstructions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "ShortTermCancellationPolicy",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "LongTermCancellationPolicy",
                table: "Listings");
        }
    }
}
