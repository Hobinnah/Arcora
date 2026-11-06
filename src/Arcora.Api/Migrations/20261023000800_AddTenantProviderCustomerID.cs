using Arcora.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations;

[DbContext(typeof(ArcoraDbContext))]
[Migration("20261023000800_AddTenantProviderCustomerID")]
public sealed class AddTenantProviderCustomerID : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Tenants]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Tenants]', N'ProviderCustomerID') IS NULL
BEGIN
    ALTER TABLE [dbo].[Tenants] ADD [ProviderCustomerID] nvarchar(255) NULL;
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Tenants]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Tenants]', N'ProviderCustomerID') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Tenants] DROP COLUMN [ProviderCustomerID];
END
");
    }
}