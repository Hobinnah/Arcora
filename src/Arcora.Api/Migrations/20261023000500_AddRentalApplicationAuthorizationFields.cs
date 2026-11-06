using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261023000500_AddRentalApplicationAuthorizationFields")]
public sealed class AddRentalApplicationAuthorizationFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalApplications]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalApplications]', N'LeaseContractReviewed') IS NULL
BEGIN
    ALTER TABLE [dbo].[RentalApplications]
    ADD [LeaseContractReviewed] bit NULL CONSTRAINT [DF_RentalApplications_LeaseContractReviewed] DEFAULT (0);
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalApplications]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalApplications]', N'VerificationAuthorization') IS NULL
BEGIN
    ALTER TABLE [dbo].[RentalApplications]
    ADD [VerificationAuthorization] bit NULL CONSTRAINT [DF_RentalApplications_VerificationAuthorization] DEFAULT (0);
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalApplications]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalApplications]', N'LeaseContractReviewed') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[RentalApplications] DROP CONSTRAINT IF EXISTS [DF_RentalApplications_LeaseContractReviewed];
    ALTER TABLE [dbo].[RentalApplications] DROP COLUMN [LeaseContractReviewed];
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalApplications]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalApplications]', N'VerificationAuthorization') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[RentalApplications] DROP CONSTRAINT IF EXISTS [DF_RentalApplications_VerificationAuthorization];
    ALTER TABLE [dbo].[RentalApplications] DROP COLUMN [VerificationAuthorization];
END
");
    }
}