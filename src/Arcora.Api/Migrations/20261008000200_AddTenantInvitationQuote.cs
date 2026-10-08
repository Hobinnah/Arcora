using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261008000200_AddTenantInvitationQuote")]
public sealed class AddTenantInvitationQuote : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>("MonthlyRentAmount", "TenantInvitations", type: "decimal(18,2)", nullable: true);
        migrationBuilder.AddColumn<decimal>("SecurityDepositAmount", "TenantInvitations", type: "decimal(18,2)", nullable: true);
        migrationBuilder.AddColumn<string>("Currency", "TenantInvitations", type: "nvarchar(3)", maxLength: 3, nullable: true);
        migrationBuilder.AddColumn<DateTime>("StartDate", "TenantInvitations", type: "datetime2", nullable: true);
        migrationBuilder.AddColumn<DateTime>("EndDate", "TenantInvitations", type: "datetime2", nullable: true);
        migrationBuilder.AddColumn<short>("LeaseTermMonths", "TenantInvitations", type: "smallint", nullable: true);
        migrationBuilder.AddColumn<Guid>("ReservationHoldID", "TenantInvitations", type: "uniqueidentifier", nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var column in new[] { "MonthlyRentAmount", "SecurityDepositAmount", "Currency", "StartDate", "EndDate", "LeaseTermMonths", "ReservationHoldID" })
            migrationBuilder.DropColumn(column, "TenantInvitations");
    }
}
