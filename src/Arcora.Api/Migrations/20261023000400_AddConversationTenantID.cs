using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261023000400_AddConversationTenantID")]
public sealed class AddConversationTenantID : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Conversations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Conversations]', N'TenantID') IS NULL
BEGIN
    ALTER TABLE [dbo].[Conversations] ADD [TenantID] uniqueidentifier NULL;
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Conversations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Conversations]', N'TenantID') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Conversations_TenantID' AND object_id = OBJECT_ID(N'[dbo].[Conversations]'))
BEGIN
    CREATE INDEX [IX_Conversations_TenantID] ON [dbo].[Conversations] ([TenantID]);
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Conversations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Conversations]', N'TenantID') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[Tenants]', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM sys.foreign_keys
       WHERE name = N'FK_Conversations_Tenants_TenantID'
   )
BEGIN
    ALTER TABLE [dbo].[Conversations]
    ADD CONSTRAINT [FK_Conversations_Tenants_TenantID]
        FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Tenants] ([TenantID]);
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Conversations]', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Conversations_Tenants_TenantID')
        ALTER TABLE [dbo].[Conversations] DROP CONSTRAINT [FK_Conversations_Tenants_TenantID];

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Conversations_TenantID' AND object_id = OBJECT_ID(N'[dbo].[Conversations]'))
        DROP INDEX [IX_Conversations_TenantID] ON [dbo].[Conversations];

    IF COL_LENGTH(N'[dbo].[Conversations]', N'TenantID') IS NOT NULL
        ALTER TABLE [dbo].[Conversations] DROP COLUMN [TenantID];
END
");
    }
}