using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    public partial class AddListingDiscountRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuarterlyDiscountRate",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SemiAnnualDiscountRate",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YearlyDiscountRate",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuarterlyDiscountRate",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "SemiAnnualDiscountRate",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "YearlyDiscountRate",
                table: "Listings");
        }
    }
}
