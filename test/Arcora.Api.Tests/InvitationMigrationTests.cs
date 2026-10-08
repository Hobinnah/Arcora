using System.Text.RegularExpressions;
using Arcora.Api.Accounts;
using Arcora.Api.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Arcora.Api.Tests;

public sealed class InvitationMigrationTests
{
    [SqlServerFact]
    public async Task PhonePreflightOutboxAndQuoteMigrationsApplyToPreviousSchema()
    {
        var connectionString = Environment.GetEnvironmentVariable("ARCORA_GUARD_TEST_SQLSERVER");
        if (string.IsNullOrWhiteSpace(connectionString)) return;
        var connection = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = $"ArcoraInvitationMigrations_{Guid.NewGuid():N}"
        };
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlServer(connection.ConnectionString).Options;
        await using var db = new ArcoraDbContext(options, new ConfigurationBuilder().Build());
        await db.Database.EnsureCreatedAsync();
        try
        {
            var legacyInvitationId = Guid.NewGuid();
            db.TenantInvitations.Add(new TenantInvitation
            {
                TenantInvitationID = legacyInvitationId, Email = "legacy@example.test",
                TokenHash = "legacy", InvitationPurpose = "Direct lease invitation",
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            });
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
            await db.Database.ExecuteSqlRawAsync("""
                DROP TABLE [TenantInvitationEmails];
                ALTER TABLE [TenantInvitations] DROP COLUMN [MonthlyRentAmount], [SecurityDepositAmount], [Currency],
                    [StartDate], [EndDate], [LeaseTermMonths], [ReservationHoldID];
                DROP INDEX [IX_AspNetUsers_PhoneNumber] ON [AspNetUsers];
                ALTER TABLE [AspNetUsers] ALTER COLUMN [PhoneNumber] nvarchar(max) NULL;
                CREATE TABLE [__EFMigrationsHistory] (
                    [MigrationId] nvarchar(150) NOT NULL PRIMARY KEY,
                    [ProductVersion] nvarchar(32) NOT NULL
                );
                """);
            Assert.True((await AccountPhoneNormalizationPreflight.RunAsync(db)).Succeeded);
            var script = db.GetService<IMigrator>().GenerateScript(
                "20261007232147_AddActiveAutopayMandateUniqueIndex",
                "20261008000200_AddTenantInvitationQuote");
            await db.Database.OpenConnectionAsync();
            foreach (var batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(batch))
                    await db.Database.ExecuteSqlRawAsync(batch);
            }

            await using var command = db.Database.GetDbConnection().CreateCommand();
            await db.Database.OpenConnectionAsync();
            command.CommandText = """
                SELECT COUNT(*) FROM sys.indexes
                WHERE name = 'IX_AspNetUsers_PhoneNumber' AND is_unique = 1 AND has_filter = 1
                """;
            Assert.Equal(1, Convert.ToInt32(await command.ExecuteScalarAsync()));
            command.CommandText = """
                SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('TenantInvitations')
                AND name IN ('MonthlyRentAmount','SecurityDepositAmount','Currency','StartDate','EndDate','LeaseTermMonths','ReservationHoldID')
                AND is_nullable = 1
                """;
            Assert.Equal(7, Convert.ToInt32(await command.ExecuteScalarAsync()));
            var legacy = await db.TenantInvitations.AsNoTracking().SingleAsync(i => i.TenantInvitationID == legacyInvitationId);
            Assert.Equal("legacy@example.test", legacy.Email);
            Assert.Null(legacy.MonthlyRentAmount);
            Assert.Null(legacy.SecurityDepositAmount);
            Assert.Null(legacy.Currency);
            Assert.Null(legacy.StartDate);
            Assert.Null(legacy.EndDate);
            Assert.Null(legacy.LeaseTermMonths);
            Assert.Null(legacy.ReservationHoldID);
            command.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'TenantInvitationEmails'";
            Assert.Equal(1, Convert.ToInt32(await command.ExecuteScalarAsync()));
            command.CommandText = """
                SELECT COUNT(*) FROM sys.columns
                WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'PhoneNumber' AND max_length = 32
                """;
            Assert.Equal(1, Convert.ToInt32(await command.ExecuteScalarAsync()));
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }
}
