using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261008000200_AddTenantInvitationQuote")]
public sealed class AddTenantInvitationQuote : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[TenantInvitations]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'MonthlyRentAmount') IS NULL
        ALTER TABLE [dbo].[TenantInvitations] ADD [MonthlyRentAmount] decimal(18,2) NULL;

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'SecurityDepositAmount') IS NULL
        ALTER TABLE [dbo].[TenantInvitations] ADD [SecurityDepositAmount] decimal(18,2) NULL;

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'Currency') IS NULL
        ALTER TABLE [dbo].[TenantInvitations] ADD [Currency] nvarchar(3) NULL;

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'StartDate') IS NULL
        ALTER TABLE [dbo].[TenantInvitations] ADD [StartDate] datetime2 NULL;

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'EndDate') IS NULL
        ALTER TABLE [dbo].[TenantInvitations] ADD [EndDate] datetime2 NULL;

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'LeaseTermMonths') IS NULL
        ALTER TABLE [dbo].[TenantInvitations] ADD [LeaseTermMonths] smallint NULL;

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'ReservationHoldID') IS NULL
        ALTER TABLE [dbo].[TenantInvitations] ADD [ReservationHoldID] uniqueidentifier NULL;
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[TenantInvitations]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'MonthlyRentAmount') IS NOT NULL
        ALTER TABLE [dbo].[TenantInvitations] DROP COLUMN [MonthlyRentAmount];

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'SecurityDepositAmount') IS NOT NULL
        ALTER TABLE [dbo].[TenantInvitations] DROP COLUMN [SecurityDepositAmount];

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'Currency') IS NOT NULL
        ALTER TABLE [dbo].[TenantInvitations] DROP COLUMN [Currency];

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'StartDate') IS NOT NULL
        ALTER TABLE [dbo].[TenantInvitations] DROP COLUMN [StartDate];

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'EndDate') IS NOT NULL
        ALTER TABLE [dbo].[TenantInvitations] DROP COLUMN [EndDate];

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'LeaseTermMonths') IS NOT NULL
        ALTER TABLE [dbo].[TenantInvitations] DROP COLUMN [LeaseTermMonths];

    IF COL_LENGTH(N'[dbo].[TenantInvitations]', N'ReservationHoldID') IS NOT NULL
        ALTER TABLE [dbo].[TenantInvitations] DROP COLUMN [ReservationHoldID];
END
");
    }
}
