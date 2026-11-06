using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    [Migration("20260912090000_AddPostalLookupSuggestion")]
    public partial class AddPostalLookupSuggestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[PostalLookupSuggestions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[PostalLookupSuggestions] (
        [Id] uniqueidentifier NOT NULL,
        [Text] nvarchar(255) NULL,
        [Line1] nvarchar(255) NULL,
        [Line2] nvarchar(255) NULL,
        [City] nvarchar(120) NULL,
        [ProvinceCode] nvarchar(10) NULL,
        [PostalCode] nvarchar(20) NULL,
        [CountryCode] nvarchar(2) NULL,
        [PlaceProvider] nvarchar(50) NULL,
        [PlaceProviderReferenceID] nvarchar(255) NULL,
        [LookupKey] nvarchar(50) NULL,
        [CapturedDate] datetime2 NULL,
        CONSTRAINT [PK_PostalLookupSuggestions] PRIMARY KEY ([Id])
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PostalLookupSuggestions_LookupKey' AND object_id = OBJECT_ID(N'[dbo].[PostalLookupSuggestions]'))
BEGIN
    CREATE INDEX [IX_PostalLookupSuggestions_LookupKey] ON [dbo].[PostalLookupSuggestions] ([LookupKey]);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PostalLookupSuggestions_PlaceProviderReferenceID' AND object_id = OBJECT_ID(N'[dbo].[PostalLookupSuggestions]'))
BEGIN
    CREATE INDEX [IX_PostalLookupSuggestions_PlaceProviderReferenceID] ON [dbo].[PostalLookupSuggestions] ([PlaceProviderReferenceID]);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PostalLookupSuggestions_Line1_City_PostalCode' AND object_id = OBJECT_ID(N'[dbo].[PostalLookupSuggestions]'))
BEGIN
    CREATE INDEX [IX_PostalLookupSuggestions_Line1_City_PostalCode] ON [dbo].[PostalLookupSuggestions] ([Line1], [City], [PostalCode]);
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[PostalLookupSuggestions]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[PostalLookupSuggestions];
END
");
        }
    }
}
