using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261029220000_AddRentDisbursements")]
public sealed class AddRentDisbursements : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("RentDisbursements", table => new
        {
            PaymentID = table.Column<Guid>(nullable: false),
            OrganizationID = table.Column<Guid>(nullable: false),
            OrgPayoutAccountID = table.Column<long>(nullable: false),
            PayoutID = table.Column<long>(nullable: false),
            AccountReference = table.Column<string>(maxLength: 255, nullable: false),
            PaymentIntentReference = table.Column<string>(maxLength: 255, nullable: false),
            TransferReference = table.Column<string>(maxLength: 255, nullable: true),
            BankPayoutReference = table.Column<string>(maxLength: 255, nullable: true),
            Status = table.Column<string>(maxLength: 50, nullable: false),
            FailureReason = table.Column<string>(maxLength: 1000, nullable: true),
            Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            CollectedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            Currency = table.Column<string>(maxLength: 3, nullable: false),
            CapturedDate = table.Column<DateTime>(nullable: false),
            LastCheckedAt = table.Column<DateTime>(nullable: true),
            PayoutAttemptedAt = table.Column<DateTime>(nullable: true),
            CapturedBy = table.Column<string>(maxLength: 256, nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_RentDisbursements", x => x.PaymentID);
            table.ForeignKey("FK_RentDisbursements_Payments_PaymentID", x => x.PaymentID,
                "Payments", "PaymentID", onDelete: ReferentialAction.Restrict);
            table.ForeignKey("FK_RentDisbursements_Payouts_PayoutID", x => x.PayoutID,
                "Payouts", "PayoutID", onDelete: ReferentialAction.Restrict);
        });
        migrationBuilder.CreateIndex("IX_RentDisbursements_PayoutID", "RentDisbursements", "PayoutID", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("RentDisbursements");
}
