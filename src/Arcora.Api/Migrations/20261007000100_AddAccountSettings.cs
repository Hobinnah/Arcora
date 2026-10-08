using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261007000100_AddAccountSettings")]
public partial class AddAccountSettings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AccountSettings",
            columns: table => new
            {
                UserID = table.Column<long>(type: "bigint", nullable: false),
                EmailNotifications = table.Column<bool>(type: "bit", nullable: false),
                SmsNotifications = table.Column<bool>(type: "bit", nullable: false),
                Language = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Timezone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AccountSettings", item => item.UserID);
                table.ForeignKey(
                    name: "FK_AccountSettings_AspNetUsers_UserID",
                    column: item => item.UserID,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AccountSettings");
    }
}
