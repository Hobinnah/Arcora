using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261023000300_AddConversationOrganizationID")]
public sealed class AddConversationOrganizationID : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Conversations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Conversations]', N'OrganizationID') IS NULL
BEGIN
    ALTER TABLE [dbo].[Conversations] ADD [OrganizationID] uniqueidentifier NULL;
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Conversations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Conversations]', N'OrganizationID') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Conversations_OrganizationID' AND object_id = OBJECT_ID(N'[dbo].[Conversations]'))
BEGIN
    CREATE INDEX [IX_Conversations_OrganizationID] ON [dbo].[Conversations] ([OrganizationID]);
END
");

        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Conversations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Conversations]', N'OrganizationID') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[Organizations]', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM sys.foreign_keys
       WHERE name = N'FK_Conversations_Organizations_OrganizationID'
   )
BEGIN
    ALTER TABLE [dbo].[Conversations]
    ADD CONSTRAINT [FK_Conversations_Organizations_OrganizationID]
        FOREIGN KEY ([OrganizationID]) REFERENCES [dbo].[Organizations] ([OrganizationID]);
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Conversations]', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Conversations_Organizations_OrganizationID')
        ALTER TABLE [dbo].[Conversations] DROP CONSTRAINT [FK_Conversations_Organizations_OrganizationID];

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Conversations_OrganizationID' AND object_id = OBJECT_ID(N'[dbo].[Conversations]'))
        DROP INDEX [IX_Conversations_OrganizationID] ON [dbo].[Conversations];

    IF COL_LENGTH(N'[dbo].[Conversations]', N'OrganizationID') IS NOT NULL
        ALTER TABLE [dbo].[Conversations] DROP COLUMN [OrganizationID];
END
");
    }
}