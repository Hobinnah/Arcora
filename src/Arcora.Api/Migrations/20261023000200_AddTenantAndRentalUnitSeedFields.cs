using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261023000200_AddTenantAndRentalUnitSeedFields")]
public sealed class AddTenantAndRentalUnitSeedFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalUnits]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalUnits]', N'Name') IS NULL
BEGIN
    ALTER TABLE [dbo].[RentalUnits] ADD [Name] nvarchar(max) NULL;
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalUnits]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalUnits]', N'Beds') IS NULL
BEGIN
    ALTER TABLE [dbo].[RentalUnits] ADD [Beds] int NULL;
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Tenants]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Tenants]', N'LeaseContractReviewed') IS NULL
BEGIN
    ALTER TABLE [dbo].[Tenants] ADD [LeaseContractReviewed] bit NULL;
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Tenants]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Tenants]', N'VerificationAuthorization') IS NULL
BEGIN
    ALTER TABLE [dbo].[Tenants] ADD [VerificationAuthorization] bit NULL;
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalUnits]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalUnits]', N'Name') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[RentalUnits] DROP COLUMN [Name];
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalUnits]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalUnits]', N'Beds') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[RentalUnits] DROP COLUMN [Beds];
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Tenants]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Tenants]', N'LeaseContractReviewed') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Tenants] DROP COLUMN [LeaseContractReviewed];
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Tenants]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Tenants]', N'VerificationAuthorization') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Tenants] DROP COLUMN [VerificationAuthorization];
END
");
    }
}