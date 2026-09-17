using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    public partial class AddUserIdAndMakeListingIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make ListingID column nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "ListingID",
                table: "ListingPhoto",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // Add UserID column (nullable bigint)
            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "ListingPhoto",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove UserID column
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "ListingPhoto");

            // Revert ListingID to non-nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "ListingID",
                table: "ListingPhoto",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
