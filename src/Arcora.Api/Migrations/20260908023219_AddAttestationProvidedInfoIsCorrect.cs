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
            migrationBuilder.AddColumn<bool>(
                name: "AttestationProvidedInfoIsCorrect",
                table: "RentalApplications",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttestationProvidedInfoIsCorrect",
                table: "RentalApplications");
        }
    }
}
