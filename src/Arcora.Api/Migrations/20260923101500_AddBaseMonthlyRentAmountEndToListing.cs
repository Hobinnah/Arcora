using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    [DbContext(typeof(ArcoraDbContext))]
    [Migration("20260923101500_AddBaseMonthlyRentAmountEndToListing")]
    public partial class AddBaseMonthlyRentAmountEndToListing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BaseMonthlyRentAmountEnd",
                table: "Listings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseMonthlyRentAmountEnd",
                table: "Listings");
        }
    }
}
