using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaseContractTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaseContractTemplates",
                columns: table => new
                {
                    LeaseContractTemplateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HtmlContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseContractTemplates", x => x.LeaseContractTemplateID);
                    table.ForeignKey(
                        name: "FK_LeaseContractTemplates_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "LeaseContractTemplateVersions",
                columns: table => new
                {
                    LeaseContractTemplateVersionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseContractTemplateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    HtmlContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeSummary = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseContractTemplateVersions", x => x.LeaseContractTemplateVersionID);
                    table.ForeignKey(
                        name: "FK_LeaseContractTemplateVersions_LeaseContractTemplates_LeaseContractTemplateID",
                        column: x => x.LeaseContractTemplateID,
                        principalTable: "LeaseContractTemplates",
                        principalColumn: "LeaseContractTemplateID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaseContractTemplates_OrganizationID",
                table: "LeaseContractTemplates",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseContractTemplateVersions_LeaseContractTemplateID",
                table: "LeaseContractTemplateVersions",
                column: "LeaseContractTemplateID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaseContractTemplateVersions");

            migrationBuilder.DropTable(
                name: "LeaseContractTemplates");
        }
    }
}
