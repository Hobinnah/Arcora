using Microsoft.EntityFrameworkCore.Migrations;
using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Arcora.Api.Migrations
{
    [DbContext(typeof(ArcoraDbContext))]
    [Migration("20260917113000_AddListingCancellationPoliciesAndExpandAccessInstructions")]
    public partial class AddListingCancellationPoliciesAndExpandAccessInstructions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'ShortTermCancellationPolicy') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [ShortTermCancellationPolicy] nvarchar(50) NULL;
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'LongTermCancellationPolicy') IS NULL
BEGIN
    ALTER TABLE [dbo].[Listings] ADD [LongTermCancellationPolicy] nvarchar(50) NULL;
END
");

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "ListingAccessInstructions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "ListingAccessInstructions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'ShortTermCancellationPolicy') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP COLUMN [ShortTermCancellationPolicy];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'LongTermCancellationPolicy') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Listings] DROP COLUMN [LongTermCancellationPolicy];
END
");
        }
    }
}
