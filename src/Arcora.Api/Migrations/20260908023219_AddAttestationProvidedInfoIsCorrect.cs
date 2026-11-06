using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAttestationProvidedInfoIsCorrect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalApplications]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalApplications]', N'AttestationProvidedInfoIsCorrect') IS NULL
BEGIN
    ALTER TABLE [dbo].[RentalApplications] ADD [AttestationProvidedInfoIsCorrect] bit NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[RentalApplications]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[RentalApplications]', N'AttestationProvidedInfoIsCorrect') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[RentalApplications] DROP COLUMN [AttestationProvidedInfoIsCorrect];
END
");
        }
    }
}
