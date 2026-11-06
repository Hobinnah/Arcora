using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCohostNameToCohostInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[CohostInvitations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[CohostInvitations]', N'CohostName') IS NULL
BEGIN
    ALTER TABLE [dbo].[CohostInvitations] ADD [CohostName] nvarchar(150) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[CohostInvitations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[CohostInvitations]', N'CohostName') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[CohostInvitations] DROP COLUMN [CohostName];
END
");
        }
    }
}
