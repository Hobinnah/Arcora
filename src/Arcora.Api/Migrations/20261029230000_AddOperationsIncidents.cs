using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261029230000_AddOperationsIncidents")]
public sealed class AddOperationsIncidents : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("OperationsIncidents", table => new
        {
            Id = table.Column<Guid>(nullable: false),
            InstanceId = table.Column<string>(maxLength: 128, nullable: false),
            CheckId = table.Column<string>(maxLength: 80, nullable: false),
            Status = table.Column<string>(maxLength: 32, nullable: false),
            Message = table.Column<string>(maxLength: 512, nullable: false),
            OpenedAt = table.Column<DateTime>(nullable: false),
            ResolvedAt = table.Column<DateTime>(nullable: true),
            EmailSentAt = table.Column<DateTime>(nullable: true),
            RecoveryEmailSentAt = table.Column<DateTime>(nullable: true),
            NextEmailAttemptAt = table.Column<DateTime>(nullable: true),
            EmailAttempts = table.Column<int>(nullable: false)
        }, constraints: table => table.PrimaryKey("PK_OperationsIncidents", x => x.Id));
        migrationBuilder.CreateIndex("IX_OperationsIncidents_InstanceId_CheckId", "OperationsIncidents",
            ["InstanceId", "CheckId"], unique: true, filter: "[ResolvedAt] IS NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("OperationsIncidents");
}
