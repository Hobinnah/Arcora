using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261029213000_AddRentPaymentAccounting")]
public sealed class AddRentPaymentAccounting : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "RentPaymentAccountings",
            columns: table => new
            {
                PaymentIntentID = table.Column<Guid>(nullable: false),
                PaymentID = table.Column<Guid>(nullable: true),
                LeaseID = table.Column<Guid>(nullable: false),
                OrganizationID = table.Column<Guid>(nullable: false),
                ListingID = table.Column<Guid>(nullable: false),
                RentalUnitID = table.Column<Guid>(nullable: false),
                RentPeriod = table.Column<DateTime>(nullable: false),
                Currency = table.Column<string>(maxLength: 3, nullable: false),
                RentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TenantFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PlacementFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                MonthlyFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                HostPlacementFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                HostMonthlyFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                HostNetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CapturedDate = table.Column<DateTime>(nullable: false),
                SettledAt = table.Column<DateTime>(nullable: true),
                ReconciliationBy = table.Column<string>(maxLength: 256, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RentPaymentAccountings", x => x.PaymentIntentID);
                table.ForeignKey("FK_RentPaymentAccountings_PaymentIntents_PaymentIntentID",
                    x => x.PaymentIntentID, "PaymentIntents", "PaymentIntentID", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_RentPaymentAccountings_Payments_PaymentID",
                    x => x.PaymentID, "Payments", "PaymentID", onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex("IX_RentPaymentAccountings_PaymentID", "RentPaymentAccountings",
            "PaymentID", unique: true, filter: "[PaymentID] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_RentPaymentAccountings_PlacementOnce", "RentPaymentAccountings",
            "LeaseID", unique: true, filter: "[SettledAt] IS NOT NULL AND [HostPlacementFee] > 0");
        migrationBuilder.CreateIndex("IX_RentPaymentAccountings_MonthlyOnce", "RentPaymentAccountings",
            new[] { "RentalUnitID", "RentPeriod" }, unique: true,
            filter: "[SettledAt] IS NOT NULL AND [HostMonthlyFee] > 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable("RentPaymentAccountings");
}
