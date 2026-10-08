using Arcora.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Arcora.Api.Accounts;

public sealed record AccountPhoneNormalizationIssue(IReadOnlyList<long> AccountIds, bool IsDuplicate);

public sealed record AccountPhoneNormalizationResult(
    bool Succeeded,
    int UpdatedCount,
    IReadOnlyList<AccountPhoneNormalizationIssue> Issues);

public static class AccountPhoneNormalizationPreflight
{
    public const string SnapshotTableName = "AccountPhoneNormalizationPreflight";

    public static async Task<AccountPhoneNormalizationResult> RunAsync(
        ArcoraDbContext db,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        var users = await db.Users
            .Where(user => user.PhoneNumber != null)
            .ToListAsync(cancellationToken);
        var normalizedByUserId = new Dictionary<long, string>();
        var invalidAccountIds = new List<long>();

        foreach (var user in users)
        {
            if (!PhoneNumberNormalizer.TryNormalize(user.PhoneNumber, out var normalized))
            {
                invalidAccountIds.Add(user.Id);
                continue;
            }

            normalizedByUserId.Add(user.Id, normalized);
        }

        var duplicateIssues = normalizedByUserId
            .GroupBy(entry => entry.Value, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => new AccountPhoneNormalizationIssue(
                group.Select(entry => entry.Key).Order().ToArray(), IsDuplicate: true))
            .ToArray();

        if (invalidAccountIds.Count > 0 || duplicateIssues.Length > 0)
        {
            await transaction.RollbackAsync(cancellationToken);
            var issues = duplicateIssues
                .Append(new AccountPhoneNormalizationIssue(invalidAccountIds.Order().ToArray(), IsDuplicate: false))
                .Where(issue => issue.AccountIds.Count > 0)
                .ToArray();
            return new AccountPhoneNormalizationResult(false, 0, issues);
        }

        var updatedCount = 0;
        foreach (var user in users)
        {
            var normalized = normalizedByUserId[user.Id];
            if (string.Equals(user.PhoneNumber, normalized, StringComparison.Ordinal))
                continue;

            user.PhoneNumber = normalized;
            updatedCount++;
        }

        await db.SaveChangesAsync(cancellationToken);
        if (db.Database.IsSqlServer())
        {
            await db.Database.ExecuteSqlRawAsync(
                $"""
                IF OBJECT_ID(N'[dbo].[{SnapshotTableName}]', N'U') IS NOT NULL
                    DROP TABLE [dbo].[{SnapshotTableName}];

                CREATE TABLE [dbo].[{SnapshotTableName}] (
                    [UserId] bigint NOT NULL PRIMARY KEY,
                    [PhoneHash] varbinary(32) NOT NULL
                );

                INSERT INTO [dbo].[{SnapshotTableName}] ([UserId], [PhoneHash])
                SELECT
                    [Id],
                    HASHBYTES(
                        'SHA2_256',
                        CASE
                            WHEN [PhoneNumber] IS NULL THEN 0x00
                            ELSE 0x01 + CONVERT(varbinary(max), [PhoneNumber])
                        END)
                FROM [dbo].[AspNetUsers];
                """,
                cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return new AccountPhoneNormalizationResult(true, updatedCount, Array.Empty<AccountPhoneNormalizationIssue>());
    }
}
