using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261008000100_AddTenantInvitationEmailOutbox")]
public sealed class AddTenantInvitationEmailOutbox : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TenantInvitationEmails",
            columns: table => new
            {
                TenantInvitationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                ProtectedMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Attempts = table.Column<int>(type: "int", nullable: false),
                NextAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                SentAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantInvitationEmails", email => email.TenantInvitationID);
                table.ForeignKey("FK_TenantInvitationEmails_TenantInvitations_TenantInvitationID",
                    email => email.TenantInvitationID, "TenantInvitations", "TenantInvitationID");
            });
        migrationBuilder.CreateIndex("IX_TenantInvitationEmails_Status_NextAttemptAt",
            "TenantInvitationEmails", new[] { "Status", "NextAttemptAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable("TenantInvitationEmails");
}
