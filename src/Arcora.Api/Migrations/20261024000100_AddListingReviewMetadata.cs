using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261024000100_AddListingReviewMetadata")]
public sealed class AddListingReviewMetadata : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "ReviewSubmittedAt",
            table: "Listings",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "ReviewedAt",
            table: "Listings",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "ReviewedByUserID",
            table: "Listings",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ReviewMessage",
            table: "Listings",
            type: "nvarchar(2000)",
            maxLength: 2000,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "ReviewSubmittedAt", table: "Listings");
        migrationBuilder.DropColumn(name: "ReviewedAt", table: "Listings");
        migrationBuilder.DropColumn(name: "ReviewedByUserID", table: "Listings");
        migrationBuilder.DropColumn(name: "ReviewMessage", table: "Listings");
    }
}