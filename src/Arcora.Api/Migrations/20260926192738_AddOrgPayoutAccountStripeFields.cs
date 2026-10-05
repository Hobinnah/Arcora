using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgPayoutAccountStripeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChargesEnabled",
                table: "OrgPayoutAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DetailsSubmitted",
                table: "OrgPayoutAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastStripeSyncAt",
                table: "OrgPayoutAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PayoutsEnabled",
                table: "OrgPayoutAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequirementsCurrentlyDue",
                table: "OrgPayoutAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequirementsEventuallyDue",
                table: "OrgPayoutAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeAccountID",
                table: "OrgPayoutAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargesEnabled",
                table: "OrgPayoutAccounts");

            migrationBuilder.DropColumn(
                name: "DetailsSubmitted",
                table: "OrgPayoutAccounts");

            migrationBuilder.DropColumn(
                name: "LastStripeSyncAt",
                table: "OrgPayoutAccounts");

            migrationBuilder.DropColumn(
                name: "PayoutsEnabled",
                table: "OrgPayoutAccounts");

            migrationBuilder.DropColumn(
                name: "RequirementsCurrentlyDue",
                table: "OrgPayoutAccounts");

            migrationBuilder.DropColumn(
                name: "RequirementsEventuallyDue",
                table: "OrgPayoutAccounts");

            migrationBuilder.DropColumn(
                name: "StripeAccountID",
                table: "OrgPayoutAccounts");
        }
    }
}
