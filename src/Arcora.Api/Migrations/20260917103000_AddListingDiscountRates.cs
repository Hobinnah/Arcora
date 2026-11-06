using Microsoft.EntityFrameworkCore.Migrations;
using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Arcora.Api.Migrations
{
    [DbContext(typeof(ArcoraDbContext))]
    [Migration("20260917103000_AddListingDiscountRates")]
    public partial class AddListingDiscountRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Listings]', N'QuarterlyDiscountRate') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [QuarterlyDiscountRate] int NOT NULL CONSTRAINT [DF_Listings_QuarterlyDiscountRate] DEFAULT (0);
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Listings]', N'SemiAnnualDiscountRate') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [SemiAnnualDiscountRate] int NOT NULL CONSTRAINT [DF_Listings_SemiAnnualDiscountRate] DEFAULT (0);
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Listings]', N'YearlyDiscountRate') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [YearlyDiscountRate] int NOT NULL CONSTRAINT [DF_Listings_YearlyDiscountRate] DEFAULT (0);
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Listings]', N'QuarterlyDiscountRate') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP CONSTRAINT IF EXISTS [DF_Listings_QuarterlyDiscountRate];
    ALTER TABLE [dbo].[Listings] DROP COLUMN [QuarterlyDiscountRate];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Listings]', N'SemiAnnualDiscountRate') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP CONSTRAINT IF EXISTS [DF_Listings_SemiAnnualDiscountRate];
    ALTER TABLE [dbo].[Listings] DROP COLUMN [SemiAnnualDiscountRate];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL AND COL_LENGTH(N'[dbo].[Listings]', N'YearlyDiscountRate') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP CONSTRAINT IF EXISTS [DF_Listings_YearlyDiscountRate];
    ALTER TABLE [dbo].[Listings] DROP COLUMN [YearlyDiscountRate];
END
");
        }
    }
}
