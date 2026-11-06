using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStripePaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Payload",
                table: "PaymentProviderEvents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[PaymentMethods]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[PaymentMethods]', N'CardFunding') IS NULL
        ALTER TABLE [dbo].[PaymentMethods] ADD [CardFunding] nvarchar(20) NULL;

    IF COL_LENGTH(N'[dbo].[PaymentMethods]', N'MethodRole') IS NULL
        ALTER TABLE [dbo].[PaymentMethods] ADD [MethodRole] nvarchar(50) NULL;
END

IF OBJECT_ID(N'[dbo].[PaymentIntents]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[PaymentIntents]', N'FailureCategory') IS NULL
        ALTER TABLE [dbo].[PaymentIntents] ADD [FailureCategory] nvarchar(50) NULL;

    IF COL_LENGTH(N'[dbo].[PaymentIntents]', N'FailureCode') IS NULL
        ALTER TABLE [dbo].[PaymentIntents] ADD [FailureCode] nvarchar(100) NULL;
END

IF OBJECT_ID(N'[dbo].[PaymentAttempts]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[PaymentAttempts]', N'FailureCategory') IS NULL
        ALTER TABLE [dbo].[PaymentAttempts] ADD [FailureCategory] nvarchar(50) NULL;

    IF COL_LENGTH(N'[dbo].[PaymentAttempts]', N'MethodKind') IS NULL
        ALTER TABLE [dbo].[PaymentAttempts] ADD [MethodKind] nvarchar(50) NULL;

    IF COL_LENGTH(N'[dbo].[PaymentAttempts]', N'PaymentMethodID') IS NULL
        ALTER TABLE [dbo].[PaymentAttempts] ADD [PaymentMethodID] uniqueidentifier NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[PaymentMethods]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[PaymentMethods]', N'CardFunding') IS NOT NULL
        ALTER TABLE [dbo].[PaymentMethods] DROP COLUMN [CardFunding];

    IF COL_LENGTH(N'[dbo].[PaymentMethods]', N'MethodRole') IS NOT NULL
        ALTER TABLE [dbo].[PaymentMethods] DROP COLUMN [MethodRole];
END

IF OBJECT_ID(N'[dbo].[PaymentIntents]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[PaymentIntents]', N'FailureCategory') IS NOT NULL
        ALTER TABLE [dbo].[PaymentIntents] DROP COLUMN [FailureCategory];

    IF COL_LENGTH(N'[dbo].[PaymentIntents]', N'FailureCode') IS NOT NULL
        ALTER TABLE [dbo].[PaymentIntents] DROP COLUMN [FailureCode];
END

IF OBJECT_ID(N'[dbo].[PaymentAttempts]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[PaymentAttempts]', N'FailureCategory') IS NOT NULL
        ALTER TABLE [dbo].[PaymentAttempts] DROP COLUMN [FailureCategory];

    IF COL_LENGTH(N'[dbo].[PaymentAttempts]', N'MethodKind') IS NOT NULL
        ALTER TABLE [dbo].[PaymentAttempts] DROP COLUMN [MethodKind];

    IF COL_LENGTH(N'[dbo].[PaymentAttempts]', N'PaymentMethodID') IS NOT NULL
        ALTER TABLE [dbo].[PaymentAttempts] DROP COLUMN [PaymentMethodID];
END
");

            migrationBuilder.AlterColumn<string>(
                name: "Payload",
                table: "PaymentProviderEvents",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
