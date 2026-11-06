using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgPayoutAccountStripeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[OrgPayoutAccounts]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'ChargesEnabled') IS NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] ADD [ChargesEnabled] bit NOT NULL CONSTRAINT [DF_OrgPayoutAccounts_ChargesEnabled] DEFAULT (0);

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'DetailsSubmitted') IS NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] ADD [DetailsSubmitted] bit NOT NULL CONSTRAINT [DF_OrgPayoutAccounts_DetailsSubmitted] DEFAULT (0);

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'LastStripeSyncAt') IS NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] ADD [LastStripeSyncAt] datetime2 NULL;

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'PayoutsEnabled') IS NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] ADD [PayoutsEnabled] bit NOT NULL CONSTRAINT [DF_OrgPayoutAccounts_PayoutsEnabled] DEFAULT (0);

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'RequirementsCurrentlyDue') IS NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] ADD [RequirementsCurrentlyDue] nvarchar(max) NULL;

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'RequirementsEventuallyDue') IS NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] ADD [RequirementsEventuallyDue] nvarchar(max) NULL;

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'StripeAccountID') IS NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] ADD [StripeAccountID] nvarchar(100) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[OrgPayoutAccounts]', N'U') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[OrgPayoutAccounts] DROP CONSTRAINT IF EXISTS [DF_OrgPayoutAccounts_ChargesEnabled];
    ALTER TABLE [dbo].[OrgPayoutAccounts] DROP CONSTRAINT IF EXISTS [DF_OrgPayoutAccounts_DetailsSubmitted];
    ALTER TABLE [dbo].[OrgPayoutAccounts] DROP CONSTRAINT IF EXISTS [DF_OrgPayoutAccounts_PayoutsEnabled];

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'ChargesEnabled') IS NOT NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] DROP COLUMN [ChargesEnabled];

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'DetailsSubmitted') IS NOT NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] DROP COLUMN [DetailsSubmitted];

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'LastStripeSyncAt') IS NOT NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] DROP COLUMN [LastStripeSyncAt];

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'PayoutsEnabled') IS NOT NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] DROP COLUMN [PayoutsEnabled];

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'RequirementsCurrentlyDue') IS NOT NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] DROP COLUMN [RequirementsCurrentlyDue];

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'RequirementsEventuallyDue') IS NOT NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] DROP COLUMN [RequirementsEventuallyDue];

    IF COL_LENGTH(N'[dbo].[OrgPayoutAccounts]', N'StripeAccountID') IS NOT NULL
        ALTER TABLE [dbo].[OrgPayoutAccounts] DROP COLUMN [StripeAccountID];
END
");
        }
    }
}
