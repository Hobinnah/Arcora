using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    [DbContext(typeof(ArcoraDbContext))]
    [Migration("20260923103000_RenameBaseMonthlyRentAmountEndToMax")]
    public partial class RenameBaseMonthlyRentAmountEndToMax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountEnd') IS NOT NULL
                   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountMax') IS NULL
                BEGIN
                    EXEC sp_rename N'[dbo].[Listings].[BaseMonthlyRentAmountEnd]', N'BaseMonthlyRentAmountMax', N'COLUMN';
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountMax') IS NOT NULL
                   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountEnd') IS NULL
                BEGIN
                    EXEC sp_rename N'[dbo].[Listings].[BaseMonthlyRentAmountMax]', N'BaseMonthlyRentAmountEnd', N'COLUMN';
                END
                """);
        }
    }
}
