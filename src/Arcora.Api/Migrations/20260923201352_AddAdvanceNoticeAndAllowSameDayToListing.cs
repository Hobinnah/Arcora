using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvanceNoticeAndAllowSameDayToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'AdvanceNotice') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [AdvanceNotice] nvarchar(100) NULL;
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'AllowSameDay') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [AllowSameDay] bit NOT NULL CONSTRAINT [DF_Listings_AllowSameDay] DEFAULT (0);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'AdvanceNotice') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP COLUMN [AdvanceNotice];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'AllowSameDay') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP CONSTRAINT IF EXISTS [DF_Listings_AllowSameDay];
    ALTER TABLE [dbo].[Listings] DROP COLUMN [AllowSameDay];
END
");
        }
    }
}
