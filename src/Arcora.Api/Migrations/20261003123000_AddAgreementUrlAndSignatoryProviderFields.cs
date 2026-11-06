using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    public partial class AddAgreementUrlAndSignatoryProviderFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Lease]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Lease]', N'AgreementUrl') IS NULL
BEGIN
    ALTER TABLE [dbo].[Lease] ADD [AgreementUrl] nvarchar(1000) NULL;
END

IF OBJECT_ID(N'[dbo].[LeaseSignatory]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[LeaseSignatory]', N'ProviderRequestID') IS NULL
        ALTER TABLE [dbo].[LeaseSignatory] ADD [ProviderRequestID] nvarchar(255) NULL;

    IF COL_LENGTH(N'[dbo].[LeaseSignatory]', N'ProviderDocumentID') IS NULL
        ALTER TABLE [dbo].[LeaseSignatory] ADD [ProviderDocumentID] nvarchar(255) NULL;

    IF COL_LENGTH(N'[dbo].[LeaseSignatory]', N'ProviderDocumentUrl') IS NULL
        ALTER TABLE [dbo].[LeaseSignatory] ADD [ProviderDocumentUrl] nvarchar(1000) NULL;

    IF COL_LENGTH(N'[dbo].[LeaseSignatory]', N'SignatureImageUrl') IS NULL
        ALTER TABLE [dbo].[LeaseSignatory] ADD [SignatureImageUrl] nvarchar(1000) NULL;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Lease]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Lease]', N'AgreementUrl') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Lease] DROP COLUMN [AgreementUrl];
END

IF OBJECT_ID(N'[dbo].[LeaseSignatory]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[LeaseSignatory]', N'ProviderRequestID') IS NOT NULL
        ALTER TABLE [dbo].[LeaseSignatory] DROP COLUMN [ProviderRequestID];

    IF COL_LENGTH(N'[dbo].[LeaseSignatory]', N'ProviderDocumentID') IS NOT NULL
        ALTER TABLE [dbo].[LeaseSignatory] DROP COLUMN [ProviderDocumentID];

    IF COL_LENGTH(N'[dbo].[LeaseSignatory]', N'ProviderDocumentUrl') IS NOT NULL
        ALTER TABLE [dbo].[LeaseSignatory] DROP COLUMN [ProviderDocumentUrl];

    IF COL_LENGTH(N'[dbo].[LeaseSignatory]', N'SignatureImageUrl') IS NOT NULL
        ALTER TABLE [dbo].[LeaseSignatory] DROP COLUMN [SignatureImageUrl];
END
");
        }
    }
}
