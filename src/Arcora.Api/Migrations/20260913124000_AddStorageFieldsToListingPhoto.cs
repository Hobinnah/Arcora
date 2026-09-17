using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    public partial class AddStorageFieldsToListingPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageProvider",
                table: "ListingPhoto",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "AZURE_BLOB");

            migrationBuilder.AddColumn<string>(
                name: "StorageContainer",
                table: "ListingPhoto",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageReference",
                table: "ListingPhoto",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageProvider",
                table: "ListingPhoto");

            migrationBuilder.DropColumn(
                name: "StorageContainer",
                table: "ListingPhoto");

            migrationBuilder.DropColumn(
                name: "StorageReference",
                table: "ListingPhoto");
        }
    }
}
