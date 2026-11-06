using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261022040000_AddTenantMoveOutInspectionUniqueIndex")]
public sealed class AddTenantMoveOutInspectionUniqueIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Inspections_LeaseID_TenantMoveOut",
            table: "Inspections",
            column: "LeaseID",
            unique: true,
            filter: "[LeaseID] IS NOT NULL AND [InspectionType] = 'TENANT_MOVE_OUT'");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Inspections_LeaseID_TenantMoveOut",
            table: "Inspections");
    }
}