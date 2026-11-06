using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261022030000_AddConversationNotes")]
public sealed class AddConversationNotes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ConversationNotes",
            columns: table => new
            {
                ConversationNoteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ConversationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                UpdatedByOrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConversationNotes", x => x.ConversationNoteID);
                table.ForeignKey("FK_ConversationNotes_Conversations_ConversationID",
                    x => x.ConversationID, "Conversations", "ConversationID");
                table.ForeignKey("FK_ConversationNotes_OrganizationMembers_UpdatedByOrganizationMemberID",
                    x => x.UpdatedByOrganizationMemberID, "OrganizationMembers", "OrganizationMemberID");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ConversationNotes_ConversationID",
            table: "ConversationNotes",
            column: "ConversationID",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_ConversationNotes_UpdatedByOrganizationMemberID",
            table: "ConversationNotes",
            column: "UpdatedByOrganizationMemberID");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable("ConversationNotes");
}
