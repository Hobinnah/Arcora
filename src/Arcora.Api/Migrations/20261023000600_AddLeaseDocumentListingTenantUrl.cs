using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261023000600_AddLeaseDocumentListingTenantUrl")]
public sealed class AddLeaseDocumentListingTenantUrl : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[LeaseDocuments]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[LeaseDocuments]', N'ListingID') IS NULL
        ALTER TABLE [dbo].[LeaseDocuments] ADD [ListingID] uniqueidentifier NULL;

    IF COL_LENGTH(N'[dbo].[LeaseDocuments]', N'TenantID') IS NULL
        ALTER TABLE [dbo].[LeaseDocuments] ADD [TenantID] uniqueidentifier NULL;

    IF COL_LENGTH(N'[dbo].[LeaseDocuments]', N'Url') IS NULL
        ALTER TABLE [dbo].[LeaseDocuments] ADD [Url] nvarchar(max) NULL;
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[LeaseDocuments]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[LeaseDocuments]', N'ListingID') IS NOT NULL
        ALTER TABLE [dbo].[LeaseDocuments] DROP COLUMN [ListingID];

    IF COL_LENGTH(N'[dbo].[LeaseDocuments]', N'TenantID') IS NOT NULL
        ALTER TABLE [dbo].[LeaseDocuments] DROP COLUMN [TenantID];

    IF COL_LENGTH(N'[dbo].[LeaseDocuments]', N'Url') IS NOT NULL
        ALTER TABLE [dbo].[LeaseDocuments] DROP COLUMN [Url];
END
");
    }
}