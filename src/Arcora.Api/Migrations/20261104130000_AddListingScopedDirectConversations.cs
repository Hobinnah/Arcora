using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261104130000_AddListingScopedDirectConversations")]
public sealed class AddListingScopedDirectConversations : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "ListingID",
            table: "Conversations",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Conversations_ListingID",
            table: "Conversations",
            column: "ListingID");

        migrationBuilder.CreateIndex(
            name: "IX_Conversations_DirectTenantOrganizationListing",
            table: "Conversations",
            columns: new[] { "TenantID", "OrganizationID", "ListingID" },
            unique: true,
            filter: "[ConversationType] = 'DIRECT' AND [TenantID] IS NOT NULL AND [OrganizationID] IS NOT NULL AND [ListingID] IS NOT NULL");

        migrationBuilder.AddForeignKey(
            name: "FK_Conversations_Listings_ListingID",
            table: "Conversations",
            column: "ListingID",
            principalTable: "Listings",
            principalColumn: "ListingID");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Conversations_Listings_ListingID",
            table: "Conversations");

        migrationBuilder.DropIndex(
            name: "IX_Conversations_DirectTenantOrganizationListing",
            table: "Conversations");

        migrationBuilder.DropIndex(
            name: "IX_Conversations_ListingID",
            table: "Conversations");

        migrationBuilder.DropColumn(
            name: "ListingID",
            table: "Conversations");
    }
}
