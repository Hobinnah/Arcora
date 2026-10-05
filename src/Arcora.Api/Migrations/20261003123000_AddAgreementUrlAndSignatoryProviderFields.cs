using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    public partial class AddAgreementUrlAndSignatoryProviderFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgreementUrl",
                table: "Lease",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderRequestID",
                table: "LeaseSignatory",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderDocumentID",
                table: "LeaseSignatory",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderDocumentUrl",
                table: "LeaseSignatory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureImageUrl",
                table: "LeaseSignatory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreementUrl",
                table: "Lease");

            migrationBuilder.DropColumn(
                name: "ProviderRequestID",
                table: "LeaseSignatory");

            migrationBuilder.DropColumn(
                name: "ProviderDocumentID",
                table: "LeaseSignatory");

            migrationBuilder.DropColumn(
                name: "ProviderDocumentUrl",
                table: "LeaseSignatory");

            migrationBuilder.DropColumn(
                name: "SignatureImageUrl",
                table: "LeaseSignatory");
        }
    }
}
