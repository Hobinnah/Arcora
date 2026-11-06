using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    [DbContext(typeof(ArcoraDbContext))]
    [Migration("20260923101500_AddBaseMonthlyRentAmountEndToListing")]
    public partial class AddBaseMonthlyRentAmountEndToListing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountEnd') IS NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountMax') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [BaseMonthlyRentAmountEnd] decimal(18,2) NOT NULL CONSTRAINT [DF_Listings_BaseMonthlyRentAmountEnd] DEFAULT (0);
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountEnd') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP CONSTRAINT IF EXISTS [DF_Listings_BaseMonthlyRentAmountEnd];
    ALTER TABLE [dbo].[Listings] DROP COLUMN [BaseMonthlyRentAmountEnd];
END
");
        }
    }
}
