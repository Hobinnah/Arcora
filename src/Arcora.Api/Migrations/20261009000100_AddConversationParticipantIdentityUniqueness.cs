using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261009000100_AddConversationParticipantIdentityUniqueness")]
public sealed class AddConversationParticipantIdentityUniqueness : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var identity in new[] { "UserID", "TenantID", "OrganizationMemberID" })
        {
            migrationBuilder.CreateIndex(
                name: $"IX_ConversationParticipants_ConversationID_{identity}",
                table: "ConversationParticipants",
                columns: new[] { "ConversationID", identity },
                unique: true,
                filter: $"[{identity}] IS NOT NULL");
        }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var identity in new[] { "UserID", "TenantID", "OrganizationMemberID" })
            migrationBuilder.DropIndex($"IX_ConversationParticipants_ConversationID_{identity}", "ConversationParticipants");
    }
}
