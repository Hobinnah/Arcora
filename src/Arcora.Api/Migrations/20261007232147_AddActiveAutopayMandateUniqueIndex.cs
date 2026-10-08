using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveAutopayMandateUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountSettings_AspNetUsers_UserID",
                table: "AccountSettings");

            migrationBuilder.DropIndex(
                name: "IX_AutopayMandates_TenantID",
                table: "AutopayMandates");

            migrationBuilder.CreateIndex(
                name: "IX_AutopayMandates_TenantID",
                table: "AutopayMandates",
                column: "TenantID",
                unique: true,
                filter: "[Status] = 'ACTIVE'");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountSettings_AspNetUsers_UserID",
                table: "AccountSettings",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountSettings_AspNetUsers_UserID",
                table: "AccountSettings");

            migrationBuilder.DropIndex(
                name: "IX_AutopayMandates_TenantID",
                table: "AutopayMandates");

            migrationBuilder.CreateIndex(
                name: "IX_AutopayMandates_TenantID",
                table: "AutopayMandates",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountSettings_AspNetUsers_UserID",
                table: "AccountSettings",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
