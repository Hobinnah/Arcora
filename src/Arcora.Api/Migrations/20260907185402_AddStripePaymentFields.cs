using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStripePaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Payload",
                table: "PaymentProviderEvents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<string>(
                name: "CardFunding",
                table: "PaymentMethods",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MethodRole",
                table: "PaymentMethods",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureCategory",
                table: "PaymentIntents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureCode",
                table: "PaymentIntents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureCategory",
                table: "PaymentAttempts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MethodKind",
                table: "PaymentAttempts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodID",
                table: "PaymentAttempts",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardFunding",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "MethodRole",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "FailureCategory",
                table: "PaymentIntents");

            migrationBuilder.DropColumn(
                name: "FailureCode",
                table: "PaymentIntents");

            migrationBuilder.DropColumn(
                name: "FailureCategory",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "MethodKind",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "PaymentMethodID",
                table: "PaymentAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "Payload",
                table: "PaymentProviderEvents",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
