using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    [DbContext(typeof(ArcoraDbContext))]
    [Migration("20260913124000_AddStorageFieldsToListingPhoto")]
    public partial class AddStorageFieldsToListingPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'StorageProvider') IS NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] ADD [StorageProvider] nvarchar(100) NOT NULL CONSTRAINT [DF_ListingPhotos_StorageProvider] DEFAULT N'AZURE_BLOB';
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'StorageContainer') IS NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] ADD [StorageContainer] nvarchar(255) NULL;
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'StorageReference') IS NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] ADD [StorageReference] nvarchar(500) NOT NULL CONSTRAINT [DF_ListingPhotos_StorageReference] DEFAULT N'';
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'StorageProvider') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] DROP CONSTRAINT IF EXISTS [DF_ListingPhotos_StorageProvider];
    ALTER TABLE [dbo].[ListingPhotos] DROP COLUMN [StorageProvider];
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'StorageContainer') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] DROP COLUMN [StorageContainer];
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'StorageReference') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] DROP CONSTRAINT IF EXISTS [DF_ListingPhotos_StorageReference];
    ALTER TABLE [dbo].[ListingPhotos] DROP COLUMN [StorageReference];
END
");
        }
    }
}
