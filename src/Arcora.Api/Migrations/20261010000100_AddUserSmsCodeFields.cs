using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261010000100_AddUserSmsCodeFields")]
public sealed class AddUserSmsCodeFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[AspNetUsers]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[AspNetUsers]', N'SmsCode') IS NULL
        ALTER TABLE [dbo].[AspNetUsers] ADD [SmsCode] nvarchar(10) NULL;

    IF COL_LENGTH(N'[dbo].[AspNetUsers]', N'SmsCodeExpiresAt') IS NULL
        ALTER TABLE [dbo].[AspNetUsers] ADD [SmsCodeExpiresAt] datetimeoffset NULL;
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[AspNetUsers]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[AspNetUsers]', N'SmsCode') IS NOT NULL
        ALTER TABLE [dbo].[AspNetUsers] DROP COLUMN [SmsCode];

    IF COL_LENGTH(N'[dbo].[AspNetUsers]', N'SmsCodeExpiresAt') IS NOT NULL
        ALTER TABLE [dbo].[AspNetUsers] DROP COLUMN [SmsCodeExpiresAt];
END
");
    }
}
