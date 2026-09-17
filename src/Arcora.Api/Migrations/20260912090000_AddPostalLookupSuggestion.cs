using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    public partial class AddPostalLookupSuggestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostalLookupSuggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Line1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Line2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ProvinceCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    PlaceProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlaceProviderReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LookupKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalLookupSuggestions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostalLookupSuggestions_LookupKey",
                table: "PostalLookupSuggestions",
                column: "LookupKey");

            migrationBuilder.CreateIndex(
                name: "IX_PostalLookupSuggestions_PlaceProviderReferenceID",
                table: "PostalLookupSuggestions",
                column: "PlaceProviderReferenceID");

            migrationBuilder.CreateIndex(
                name: "IX_PostalLookupSuggestions_Line1_City_PostalCode",
                table: "PostalLookupSuggestions",
                columns: new[] { "Line1", "City", "PostalCode" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostalLookupSuggestions");
        }
    }
}
