using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261029000100_AddPaymentIntentIdempotencyUniqueIndex")]
public sealed class AddPaymentIntentIdempotencyUniqueIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.CreateIndex("IX_PaymentIntents_IdempotencyKey", "PaymentIntents", "IdempotencyKey", unique: true);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropIndex("IX_PaymentIntents_IdempotencyKey", "PaymentIntents");
}
