using System;
using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    [DbContext(typeof(ArcoraDbContext))]
    [Migration("20260913120000_AddUserIdAndMakeListingIdNullable")]
    public partial class AddUserIdAndMakeListingIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make ListingID column nullable (safe to re-run)
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'ListingID') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] ALTER COLUMN [ListingID] uniqueidentifier NULL;
END
");

            // Add UserID column only if it does not already exist
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'UserID') IS NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] ADD [UserID] bigint NULL;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove UserID column only when present
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'UserID') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] DROP COLUMN [UserID];
END
");

            // Revert ListingID to non-nullable only when present
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[ListingPhotos]', N'ListingID') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[ListingPhotos] ALTER COLUMN [ListingID] uniqueidentifier NOT NULL;
END
");
        }
    }
}
