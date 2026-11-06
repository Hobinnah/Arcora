using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261022020000_AddConversationMessageReactions")]
public sealed class AddConversationMessageReactions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ConversationMessageReactions",
            columns: table => new
            {
                ConversationMessageReactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ConversationMessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReactorTenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ReactorOrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Emoji = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConversationMessageReactions", x => x.ConversationMessageReactionID);
                table.CheckConstraint("CK_ConversationMessageReactions_SingleReactor",
                    "([ReactorTenantID] IS NULL AND [ReactorOrganizationMemberID] IS NOT NULL) OR ([ReactorTenantID] IS NOT NULL AND [ReactorOrganizationMemberID] IS NULL)");
                table.ForeignKey("FK_ConversationMessageReactions_ConversationMessages_ConversationMessageID",
                    x => x.ConversationMessageID, "ConversationMessages", "ConversationMessageID");
                table.ForeignKey("FK_ConversationMessageReactions_Tenants_ReactorTenantID",
                    x => x.ReactorTenantID, "Tenants", "TenantID");
                table.ForeignKey("FK_ConversationMessageReactions_OrganizationMembers_ReactorOrganizationMemberID",
                    x => x.ReactorOrganizationMemberID, "OrganizationMembers", "OrganizationMemberID");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ConversationMessageReactions_ConversationMessageID_ReactorTenantID",
            table: "ConversationMessageReactions",
            columns: new[] { "ConversationMessageID", "ReactorTenantID" },
            unique: true,
            filter: "[ReactorTenantID] IS NOT NULL");
        migrationBuilder.CreateIndex(
            name: "IX_ConversationMessageReactions_ConversationMessageID_ReactorOrganizationMemberID",
            table: "ConversationMessageReactions",
            columns: new[] { "ConversationMessageID", "ReactorOrganizationMemberID" },
            unique: true,
            filter: "[ReactorOrganizationMemberID] IS NOT NULL");
        migrationBuilder.CreateIndex(
            name: "IX_ConversationMessageReactions_ReactorTenantID",
            table: "ConversationMessageReactions",
            column: "ReactorTenantID");
        migrationBuilder.CreateIndex(
            name: "IX_ConversationMessageReactions_ReactorOrganizationMemberID",
            table: "ConversationMessageReactions",
            column: "ReactorOrganizationMemberID");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable("ConversationMessageReactions");
}
