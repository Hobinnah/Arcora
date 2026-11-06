using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPetFriendlyToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'IsPetFriendly') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [IsPetFriendly] bit NOT NULL CONSTRAINT [DF_Listings_IsPetFriendly] DEFAULT (0);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'IsPetFriendly') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP CONSTRAINT IF EXISTS [DF_Listings_IsPetFriendly];
    ALTER TABLE [dbo].[Listings] DROP COLUMN [IsPetFriendly];
END
");
        }
    }
}
