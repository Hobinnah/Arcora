using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261030010000_AddAttachmentSendIdempotency")]
public sealed class AddAttachmentSendIdempotency : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>("AttachmentSendKey", "ConversationMessages", type: "uniqueidentifier", nullable: true);
        migrationBuilder.AddColumn<string>("AttachmentSenderScope", "ConversationMessages", type: "nvarchar(48)", maxLength: 48, nullable: true);
        migrationBuilder.AddColumn<string>("AttachmentPayloadHash", "ConversationMessages", type: "nvarchar(64)", maxLength: 64, nullable: true);
        migrationBuilder.CreateIndex(
            name: "IX_ConversationMessages_ConversationID_AttachmentSenderScope_AttachmentSendKey",
            table: "ConversationMessages",
            columns: new[] { "ConversationID", "AttachmentSenderScope", "AttachmentSendKey" },
            unique: true,
            filter: "[AttachmentSenderScope] IS NOT NULL AND [AttachmentSendKey] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_ConversationMessages_ConversationID_AttachmentSenderScope_AttachmentSendKey", "ConversationMessages");
        migrationBuilder.DropColumn("AttachmentSendKey", "ConversationMessages");
        migrationBuilder.DropColumn("AttachmentSenderScope", "ConversationMessages");
        migrationBuilder.DropColumn("AttachmentPayloadHash", "ConversationMessages");
    }
}
