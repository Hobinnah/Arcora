using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261022050000_AddDepositSettlementIdempotencyUniqueIndex")]
public sealed class AddDepositSettlementIdempotencyUniqueIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_PaymentProviderEvents_InternalSecurityDeposit_Idempotency",
            table: "PaymentProviderEvents",
            columns: new[] { "ProviderName", "ProviderEventID" },
            unique: true,
            filter: "[ProviderName] = 'INTERNAL_SECURITY_DEPOSIT'");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_PaymentProviderEvents_InternalSecurityDeposit_Idempotency",
            table: "PaymentProviderEvents");
    }
}