using System.Text.Json;
using Arcora.Api.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Email;

public sealed class TenantInvitationEmailWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<TenantInvitationEmailWorker> logger) : BackgroundService
{
    public const string ProtectionPurpose = "Arcora.TenantInvitationEmail.v1";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
                var ids = await db.TenantInvitationEmails.AsNoTracking()
                    .Where(email => (email.Status == "PENDING" || email.Status == "FAILED")
                        && email.NextAttemptAt <= DateTime.UtcNow)
                    .OrderBy(email => email.NextAttemptAt)
                    .Select(email => email.TenantInvitationID)
                    .Take(20).ToListAsync(stoppingToken);
                foreach (var id in ids)
                {
                    using var deliveryScope = scopeFactory.CreateScope();
                    await DispatchAsync(deliveryScope.ServiceProvider, id, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Tenant invitation email dispatcher failed. Pending jobs remain persisted for retry.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    public static async Task DispatchAsync(IServiceProvider services, Guid id, CancellationToken ct)
    {
        var db = services.GetRequiredService<ArcoraDbContext>();
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        // Serialize delivery across API instances; a crash rolls back the claim for a later retry.
        await LockAsync(db, id, ct);
        var email = await db.TenantInvitationEmails.Include(e => e.TenantInvitation)
            .SingleOrDefaultAsync(e => e.TenantInvitationID == id, ct);
        if (email == null || email.Status is "SENT" or "CANCELLED" || email.NextAttemptAt > DateTime.UtcNow)
            return;

        var invitation = email.TenantInvitation;
        if (invitation == null || invitation.ExpiresAt <= DateTime.UtcNow
            || invitation.Status is "REVOKED" or "DECLINED" or "EXPIRED")
        {
            email.Status = "CANCELLED";
            email.ProtectedMessage = string.Empty;
        }
        else
        {
            email.Attempts++;
            email.LastAttemptAt = DateTime.UtcNow;
            try
            {
                var protector = services.GetRequiredService<IDataProtectionProvider>()
                    .CreateProtector(ProtectionPurpose);
                var message = JsonSerializer.Deserialize<EmailMessage>(protector.Unprotect(email.ProtectedMessage))
                    ?? throw new InvalidOperationException("The persisted invitation email is invalid.");
                await services.GetRequiredService<IEmailSender>()
                    .SendEmailAsync(message.To, message.Subject, message.HtmlBody);
                email.Status = "SENT";
                email.SentAt = DateTime.UtcNow;
                email.ProtectedMessage = string.Empty;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                services.GetRequiredService<ILogger<TenantInvitationEmailWorker>>()
                    .LogError(ex, "Invitation email dispatch failed for {InvitationID}, attempt {Attempt}.", id, email.Attempts);
                email.Status = "FAILED";
                email.NextAttemptAt = DateTime.UtcNow.AddSeconds(Math.Min(3600, 30 * Math.Pow(2, Math.Min(email.Attempts, 7))));
            }
        }

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public static async Task LockAsync(ArcoraDbContext db, Guid id, CancellationToken ct = default)
    {
        if (db.Database.IsSqlServer())
        {
            var resource = $"invitation-email:{id:N}";
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                DECLARE @result int;
                EXEC @result = sys.sp_getapplock @Resource={resource}, @LockMode='Exclusive',
                    @LockOwner='Transaction', @LockTimeout=10000;
                IF @result < 0 THROW 51001, 'Unable to acquire invitation email delivery lock.', 1;
                """, ct);
        }
    }
}
