using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationMemberProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BirthDecade",
                table: "OrganizationMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeUniqueDescription",
                table: "OrganizationMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetsDescription",
                table: "OrganizationMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoUrl",
                table: "OrganizationMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolDescription",
                table: "OrganizationMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TravelDestination",
                table: "OrganizationMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkDescription",
                table: "OrganizationMembers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDecade",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "HomeUniqueDescription",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "PetsDescription",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoUrl",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "SchoolDescription",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "TravelDestination",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "WorkDescription",
                table: "OrganizationMembers");
        }
    }
}
