using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantInvitationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[TenantInvitations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[TenantInvitations]', N'Name') IS NULL
BEGIN
    ALTER TABLE [dbo].[TenantInvitations] ADD [Name] nvarchar(200) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[TenantInvitations]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[TenantInvitations]', N'Name') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[TenantInvitations] DROP COLUMN [Name];
END
");
        }
    }
}
