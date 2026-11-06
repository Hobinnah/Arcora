using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261010000200_AddActiveIdentityVerificationUniqueIndex")]
public sealed class AddActiveIdentityVerificationUniqueIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_IdentityVerifications_UserID_ProviderName_VerificationType_Active",
            table: "IdentityVerifications",
            columns: new[] { "UserID", "ProviderName", "VerificationType" },
            unique: true,
            filter: "[ProviderName] IS NOT NULL AND [VerificationType] IS NOT NULL AND [Status] IN ('NOT_STARTED','PENDING','SUBMITTED','UNDER_REVIEW')");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_IdentityVerifications_UserID_ProviderName_VerificationType_Active",
            table: "IdentityVerifications");
    }
}
