using System.Text.Json;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Enums;
using Arcora.Api.Models;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Services.Implementations
{
    public class HostingSecurityDepositService : IHostingSecurityDepositService
    {
        private static readonly HashSet<string> AllowedEvidenceContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "application/pdf"
        };

        private const long MaxEvidenceSizeBytes = 10 * 1024 * 1024;

        private readonly ArcoraDbContext dbContext;
        private readonly IOrganizationMemberRepository organizationMemberRepository;
        private readonly IFileStorageService fileStorageService;
        private readonly IEmailSender emailSender;
        private readonly ISecurityDepositRefundProcessor refundProcessor;

        public HostingSecurityDepositService(
            ArcoraDbContext dbContext,
            IOrganizationMemberRepository organizationMemberRepository,
            IFileStorageService fileStorageService,
            IEmailSender emailSender,
            ISecurityDepositRefundProcessor refundProcessor)
        {
            this.dbContext = dbContext;
            this.organizationMemberRepository = organizationMemberRepository;
            this.fileStorageService = fileStorageService;
            this.emailSender = emailSender;
            this.refundProcessor = refundProcessor;
        }

        public async Task<HostSecurityDepositListResponseDto> GetHostSecurityDeposits(long actorUserID, int pageSize, int pageNumber, string? status)
        {
            var authorizedOrganizationIDs = await GetAuthorizedOrganizationIDs(actorUserID);
            if (authorizedOrganizationIDs.Count == 0)
                return new HostSecurityDepositListResponseDto();

            pageSize = Math.Clamp(pageSize, 1, 100);
            pageNumber = Math.Max(pageNumber, 1);

            var baseQuery = dbContext.SecurityDeposits
                .AsNoTracking()
                .Include(x => x.Lease)
                    .ThenInclude(x => x!.Listing)
                .Include(x => x.Tenant)
                    .ThenInclude(x => x!.User)
                .Where(x => authorizedOrganizationIDs.Contains(x.OrganizationID));

            if (!string.IsNullOrWhiteSpace(status))
                baseQuery = baseQuery.Where(x => x.Status == status);

            var allForSummary = await baseQuery.ToListAsync();
            var totalCount = allForSummary.Count;
            var depositIDs = allForSummary.Select(x => x.SecurityDepositID).ToList();
            var pendingReturnTransactions = depositIDs.Count == 0
                ? new List<SecurityDepositTransaction>()
                : await dbContext.SecurityDepositTransactions
                    .AsNoTracking()
                    .Include(x => x.Refund)
                    .Where(x => depositIDs.Contains(x.SecurityDepositID) && x.TransactionType == "RETURN_PENDING"
                        && x.Refund != null && (x.Refund.Status == "PENDING" || x.Refund.Status == "REQUIRES_ACTION"))
                    .ToListAsync();
            var pendingReturnAmounts = pendingReturnTransactions
                .GroupBy(x => x.SecurityDepositID)
                .ToDictionary(x => x.Key, x => x.Sum(item => item.Amount));

            var pageItems = allForSummary
                .OrderByDescending(x => x.CapturedDate ?? DateTime.MinValue)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => MapHostListItem(x, pendingReturnAmounts.GetValueOrDefault(x.SecurityDepositID)))
                .ToList();

            return new HostSecurityDepositListResponseDto
            {
                Data = pageItems,
                TotalCount = totalCount,
                Summary = BuildSummary(allForSummary, pendingReturnAmounts)
            };
        }

        public async Task<PagedResult<SecurityDepositTransactionDto>> GetTransactionsBySecurityDeposit(Guid securityDepositID, long actorUserID, int pageSize, int pageNumber)
        {
            var deposit = await GetAuthorizedDepositOrThrow(securityDepositID, actorUserID);

            pageSize = Math.Clamp(pageSize, 1, 100);
            pageNumber = Math.Max(pageNumber, 1);

            var query = dbContext.SecurityDepositTransactions
                .AsNoTracking()
                .Where(x => x.SecurityDepositID == deposit.SecurityDepositID)
                .OrderByDescending(x => x.OccurredAt);

            var totalCount = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SecurityDepositTransactionDto
                {
                    SecurityDepositTransactionID = x.SecurityDepositTransactionID,
                    SecurityDepositID = x.SecurityDepositID,
                    PaymentID = x.PaymentID,
                    RefundID = x.RefundID,
                    InvoiceMasterID = x.InvoiceMasterID,
                    InvoiceDetailID = x.InvoiceDetailID,
                    TransactionType = x.TransactionType,
                    Amount = x.Amount,
                    Currency = x.Currency,
                    Description = x.Description,
                    OccurredAt = x.OccurredAt,
                    CapturedDate = x.CapturedDate,
                    CapturedBy = x.CapturedBy
                })
                .ToListAsync();

            return new PagedResult<SecurityDepositTransactionDto>
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        public async Task<SecurityDepositEvidenceUploadResponseDto> UploadEvidence(Guid securityDepositID, long actorUserID, IFormFile file, CancellationToken cancellationToken)
        {
            await GetAuthorizedDepositOrThrow(securityDepositID, actorUserID);

            if (file == null || file.Length <= 0)
                throw new ArgumentException("No file was provided.");
            if (file.Length > MaxEvidenceSizeBytes)
                throw new ArgumentException($"File exceeds the maximum allowed size of {MaxEvidenceSizeBytes / (1024 * 1024)} MB.");
            if (!AllowedEvidenceContentTypes.Contains(file.ContentType))
                throw new ArgumentException("Invalid file type. Allowed types: image/jpeg, image/png, application/pdf.");

            await EnsureContentMatchesMimeTypeAsync(file, cancellationToken);

            await using var stream = file.OpenReadStream();
            var category = file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                ? StorageCategory.Image
                : StorageCategory.Document;

            var uploadResult = await fileStorageService.UploadAsync(category, file.FileName, stream, file.ContentType, cancellationToken);

            var attachment = new Attachment
            {
                AttachmentID = Guid.NewGuid(),
                EntityType = "SECURITY_DEPOSIT_EVIDENCE",
                EntityID = securityDepositID.ToString(),
                FileName = file.FileName,
                MimeType = file.ContentType,
                FileSizeBytes = file.Length,
                StorageProvider = "AZURE_BLOB",
                StorageReference = uploadResult.BlobName,
                AttachmentType = "EVIDENCE",
                CapturedBy = actorUserID.ToString(),
                CapturedDate = DateTime.UtcNow
            };

            dbContext.Attachments.Add(attachment);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new SecurityDepositEvidenceUploadResponseDto
            {
                AttachmentID = attachment.AttachmentID,
                FileName = attachment.FileName,
                MimeType = attachment.MimeType,
                FileSizeBytes = attachment.FileSizeBytes,
                StorageReference = attachment.StorageReference
            };
        }

        public async Task<SecurityDepositSettlementNoticeResponseDto> SendSettlementNotice(Guid securityDepositID, long actorUserID, string? idempotencyKey, SecurityDepositSettlementNoticeRequestDto request)
        {
            var deposit = await GetAuthorizedDepositOrThrow(securityDepositID, actorUserID, includeTenantUser: true);

            if (request.Deductions == null || request.Deductions.Count == 0)
                throw new ArgumentException("At least one deduction is required.");
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new ArgumentException("Idempotency-Key header is required.");

            var idempotencyEventID = $"settlement-notice:{securityDepositID}:{idempotencyKey}";
            var existingEvent = await dbContext.PaymentProviderEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProviderName == "INTERNAL_SECURITY_DEPOSIT" && x.ProviderEventID == idempotencyEventID);

            if (existingEvent != null)
            {
                var replay = JsonSerializer.Deserialize<SecurityDepositSettlementNoticeResponseDto>(existingEvent.Payload ?? string.Empty);
                if (replay != null)
                    return replay;
            }

            var deductionTotal = request.Deductions.Sum(x => x.Amount);
            if (deductionTotal <= 0)
                throw new ArgumentException("Deductions total must be greater than zero.");

            if (!string.Equals(request.Currency?.Trim(), deposit.Currency?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Currency must match the deposit currency.");

            foreach (var deduction in request.Deductions)
            {
                if (deduction.Amount <= 0)
                    throw new ArgumentException("Deduction amounts must be positive.");

                if (deduction.EvidenceAttachmentIDs == null || deduction.EvidenceAttachmentIDs.Count == 0)
                    throw new ArgumentException("Each deduction must include at least one evidence attachment.");

                var evidenceCount = await dbContext.Attachments.AsNoTracking().CountAsync(a =>
                    deduction.EvidenceAttachmentIDs.Contains(a.AttachmentID) &&
                    a.EntityType == "SECURITY_DEPOSIT_EVIDENCE" &&
                    a.EntityID == securityDepositID.ToString());

                if (evidenceCount != deduction.EvidenceAttachmentIDs.Count)
                    throw new ArgumentException("One or more evidence attachments are invalid for this deposit.");
            }

            var availableToSettle = Math.Max(0m, deposit.ReceivedAmount - deposit.ReturnedAmount - deposit.AppliedAmount);
            if (deductionTotal > availableToSettle)
                throw new ArgumentException("Deductions exceed the available amount for settlement.");

            var returnAmount = availableToSettle - deductionTotal;
            deposit.AppliedAmount += deductionTotal;
            deposit.Status = returnAmount > 0 ? "RETURN_DUE" : "CLOSED";
            deposit.UpdatedBy = actorUserID.ToString();
            deposit.UpdatedDate = DateTime.UtcNow;
            if (string.Equals(deposit.Status, "CLOSED", StringComparison.OrdinalIgnoreCase))
                deposit.ClosedAt = DateTime.UtcNow;

            var transaction = new SecurityDepositTransaction
            {
                SecurityDepositTransactionID = Guid.NewGuid(),
                SecurityDepositID = deposit.SecurityDepositID,
                TransactionType = "SETTLEMENT_NOTICE",
                Amount = deductionTotal,
                Currency = deposit.Currency,
                Description = JsonSerializer.Serialize(request.Deductions),
                OccurredAt = DateTime.UtcNow,
                CapturedBy = actorUserID.ToString(),
                CapturedDate = DateTime.UtcNow
            };

            dbContext.SecurityDepositTransactions.Add(transaction);

            var recipientEmail = deposit.Tenant?.User?.Email;
            var response = new SecurityDepositSettlementNoticeResponseDto
            {
                SecurityDepositID = deposit.SecurityDepositID,
                DeductionsTotal = deductionTotal,
                ReturnAmount = returnAmount,
                Currency = deposit.Currency,
                SentAtUtc = DateTime.UtcNow,
                RecipientEmail = recipientEmail,
                Status = deposit.Status
            };

            var providerEvent = new PaymentProviderEvent
            {
                PaymentProviderEventID = Guid.NewGuid(),
                ProviderName = "INTERNAL_SECURITY_DEPOSIT",
                ProviderEventID = idempotencyEventID,
                EventType = "SETTLEMENT_NOTICE",
                ProcessingStatus = "RECEIVED",
                Payload = JsonSerializer.Serialize(response),
                ReceivedAt = DateTime.UtcNow,
                CapturedDate = DateTime.UtcNow
            };
            dbContext.PaymentProviderEvents.Add(providerEvent);

            await dbContext.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(recipientEmail))
            {
                var body = $"Security deposit settlement notice for deposit {deposit.SecurityDepositID}. Deductions: {deductionTotal:0.00} {deposit.Currency}. Return amount: {returnAmount:0.00} {deposit.Currency}.";
                await emailSender.SendEmailAsync(recipientEmail, "Security deposit settlement notice", body);
            }

            providerEvent.ProcessingStatus = "PROCESSED";
            providerEvent.ProcessedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return response;
        }

        public async Task<SecurityDepositReturnResponseDto> ReturnDeposit(Guid securityDepositID, long actorUserID, SecurityDepositReturnRequestDto request)
        {
            ArgumentNullException.ThrowIfNull(request);
            await GetAuthorizedDepositOrThrow(securityDepositID, actorUserID);

            var idempotencyKey = request.IdempotencyKey?.Trim();
            if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Length > 200)
                throw new ArgumentException("A valid idempotencyKey of at most 200 characters is required.");

            var idempotencyEventID = $"deposit-return:{securityDepositID}:{idempotencyKey}";
            await using var tx = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var lockedDeposit = await dbContext.SecurityDeposits
                .FirstOrDefaultAsync(x => x.SecurityDepositID == securityDepositID);
            if (lockedDeposit == null)
                throw new KeyNotFoundException("Security deposit was not found.");

            var existingEvent = await dbContext.PaymentProviderEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProviderName == "INTERNAL_SECURITY_DEPOSIT" && x.ProviderEventID == idempotencyEventID);
            if (existingEvent != null)
            {
                var replay = JsonSerializer.Deserialize<SecurityDepositReturnResponseDto>(existingEvent.Payload ?? string.Empty);
                if (replay != null)
                {
                    var replayRefund = await dbContext.Refunds.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.RefundID == replay.RefundID);
                    if (replayRefund != null)
                    {
                        replay.RefundStatus = replayRefund.Status;
                        replay.ReturnedAmount = lockedDeposit.ReturnedAmount;
                        replay.HeldAmount = await GetAvailableHeldAmountAsync(securityDepositID, lockedDeposit);
                        replay.Status = lockedDeposit.Status;
                    }
                    replay.IdempotentReplay = true;
                    await tx.CommitAsync();
                    return replay;
                }
            }

            if (!string.Equals(request.Currency?.Trim(), lockedDeposit.Currency?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Currency must match the deposit currency.");
            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            var pendingReturnAmount = await GetPendingReturnAmountAsync(securityDepositID);
            var availableToReturn = Math.Max(0m,
                lockedDeposit.ReceivedAmount - lockedDeposit.AppliedAmount - lockedDeposit.ReturnedAmount - pendingReturnAmount);
            if (request.Amount > availableToReturn)
                throw new ArgumentException("Return amount exceeds the available held balance.");

            var linkedPaymentID = await dbContext.SecurityDepositTransactions
                .AsNoTracking()
                .Where(x => x.SecurityDepositID == securityDepositID && x.PaymentID != null)
                .OrderByDescending(x => x.OccurredAt)
                .Select(x => x.PaymentID)
                .FirstOrDefaultAsync();
            if (linkedPaymentID == null)
                throw new ArgumentException("No linked payment was found for this deposit. Processor refund cannot be reconciled.");

            var payment = await dbContext.Payments.AsNoTracking()
                .FirstOrDefaultAsync(x => x.PaymentID == linkedPaymentID.Value);
            if (payment == null || string.IsNullOrWhiteSpace(payment.ProviderChargeID))
                throw new ArgumentException("No reconciliable processor payment was found for this deposit.");

            var stripeRefund = await refundProcessor.CreateRefundAsync(
                payment.ProviderChargeID,
                ToMinorUnits(request.Amount),
                securityDepositID,
                $"deposit-return-{securityDepositID:N}-{idempotencyKey}");

            var normalizedRefundStatus = stripeRefund.Status?.ToUpperInvariant() ?? "PENDING";
            var refundSucceeded = normalizedRefundStatus == "SUCCEEDED";
            var refundPending = normalizedRefundStatus is "PENDING" or "REQUIRES_ACTION";
            var refund = new Arcora.Api.Entities.Refund
            {
                RefundID = Guid.NewGuid(),
                PaymentID = linkedPaymentID.Value,
                TenantID = lockedDeposit.TenantID,
                Amount = request.Amount,
                Currency = lockedDeposit.Currency,
                Reason = $"Security deposit return ({idempotencyKey})",
                Status = refundSucceeded ? "PROCESSED" : normalizedRefundStatus,
                ProviderName = "STRIPE",
                ProviderRefundID = stripeRefund.ProviderRefundID,
                RequestedAt = DateTime.UtcNow,
                ProcessedAt = refundSucceeded ? DateTime.UtcNow : null,
                FailedAt = normalizedRefundStatus is "FAILED" or "CANCELED" ? DateTime.UtcNow : null,
                FailureReason = stripeRefund.FailureReason,
                CapturedDate = DateTime.UtcNow,
                CapturedBy = actorUserID.ToString()
            };
            dbContext.Refunds.Add(refund);

            var transaction = new SecurityDepositTransaction
            {
                SecurityDepositTransactionID = Guid.NewGuid(),
                SecurityDepositID = securityDepositID,
                RefundID = refund.RefundID,
                PaymentID = linkedPaymentID,
                TransactionType = refundSucceeded ? "RETURN" : refundPending ? "RETURN_PENDING" : "RETURN_FAILED",
                Amount = request.Amount,
                Currency = lockedDeposit.Currency,
                Description = "Security deposit return",
                OccurredAt = DateTime.UtcNow,
                CapturedDate = DateTime.UtcNow,
                CapturedBy = actorUserID.ToString()
            };
            dbContext.SecurityDepositTransactions.Add(transaction);

            if (refundSucceeded)
            {
                lockedDeposit.ReturnedAmount += request.Amount;
                var remaining = Math.Max(0m,
                    lockedDeposit.ReceivedAmount - lockedDeposit.AppliedAmount - lockedDeposit.ReturnedAmount - pendingReturnAmount);
                lockedDeposit.Status = remaining <= 0 ? "RETURNED" : "PARTIALLY_RETURNED";
                if (remaining <= 0)
                    lockedDeposit.ClosedAt = DateTime.UtcNow;
            }
            else if (refundPending)
            {
                lockedDeposit.Status = "RETURN_PENDING";
            }

            lockedDeposit.UpdatedBy = actorUserID.ToString();
            lockedDeposit.UpdatedDate = DateTime.UtcNow;

            var response = new SecurityDepositReturnResponseDto
            {
                SecurityDepositID = lockedDeposit.SecurityDepositID,
                RefundID = refund.RefundID,
                SecurityDepositTransactionID = transaction.SecurityDepositTransactionID,
                ReturnedAmount = lockedDeposit.ReturnedAmount,
                HeldAmount = Math.Max(0m, availableToReturn - (refundSucceeded || refundPending ? request.Amount : 0m)),
                Status = lockedDeposit.Status,
                RefundStatus = refund.Status,
                IdempotentReplay = false
            };

            dbContext.PaymentProviderEvents.Add(new PaymentProviderEvent
            {
                PaymentProviderEventID = Guid.NewGuid(),
                ProviderName = "INTERNAL_SECURITY_DEPOSIT",
                ProviderEventID = idempotencyEventID,
                EventType = "DEPOSIT_RETURN",
                ProcessingStatus = "PROCESSED",
                Payload = JsonSerializer.Serialize(response),
                ReceivedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow,
                CapturedDate = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
            await tx.CommitAsync();
            return response;
        }

        public async Task ReconcileRefundAsync(string providerRefundID, string status, string? failureReason = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(providerRefundID))
                return;

            var refund = await dbContext.Refunds
                .FirstOrDefaultAsync(x => x.ProviderName == "STRIPE" && x.ProviderRefundID == providerRefundID, cancellationToken);
            if (refund == null)
                throw new InvalidOperationException("The local deposit refund has not been persisted yet.");

            var transaction = await dbContext.SecurityDepositTransactions
                .FirstOrDefaultAsync(x => x.RefundID == refund.RefundID, cancellationToken);
            if (transaction == null || transaction.TransactionType is not ("RETURN_PENDING" or "RETURN"))
                return;

            var deposit = await dbContext.SecurityDeposits
                .FirstOrDefaultAsync(x => x.SecurityDepositID == transaction.SecurityDepositID, cancellationToken);
            if (deposit == null)
                return;

            var normalizedStatus = status.Trim().ToUpperInvariant();
            var now = DateTime.UtcNow;
            if (normalizedStatus is "SUCCEEDED" or "PROCESSED")
            {
                if (refund.Status != "PROCESSED")
                {
                    refund.Status = "PROCESSED";
                    refund.ProcessedAt = now;
                    refund.FailedAt = null;
                    refund.FailureReason = null;
                    transaction.TransactionType = "RETURN";
                    deposit.ReturnedAmount += refund.Amount;
                }
            }
            else if (normalizedStatus is "FAILED" or "CANCELED" or "CANCELLED")
            {
                if (refund.Status != "PROCESSED")
                {
                    refund.Status = "FAILED";
                    refund.FailedAt = now;
                    refund.FailureReason = string.IsNullOrWhiteSpace(failureReason) ? "Stripe refund failed." : failureReason[..Math.Min(failureReason.Length, 256)];
                    transaction.TransactionType = "RETURN_FAILED";
                }
            }
            else if (refund.Status is not ("PROCESSED" or "FAILED"))
            {
                refund.Status = "PENDING";
                transaction.TransactionType = "RETURN_PENDING";
            }

            var otherPendingTransactions = await dbContext.SecurityDepositTransactions
                .Where(x => x.SecurityDepositID == deposit.SecurityDepositID && x.TransactionType == "RETURN_PENDING"
                    && x.RefundID != refund.RefundID && x.Refund != null
                    && (x.Refund.Status == "PENDING" || x.Refund.Status == "REQUIRES_ACTION"))
                .Select(x => x.Amount)
                .ToListAsync(cancellationToken);
            var otherPendingAmount = otherPendingTransactions.Sum();
            var currentPendingAmount = transaction.TransactionType == "RETURN_PENDING"
                && refund.Status is "PENDING" or "REQUIRES_ACTION"
                ? transaction.Amount
                : 0m;
            var totalPendingAmount = otherPendingAmount + currentPendingAmount;
            var available = Math.Max(0m,
                deposit.ReceivedAmount - deposit.AppliedAmount - deposit.ReturnedAmount - totalPendingAmount);
            if (totalPendingAmount > 0m)
                deposit.Status = "RETURN_PENDING";
            else if (deposit.ReceivedAmount <= 0m)
                deposit.Status = "EXPECTED";
            else if (available <= 0m)
            {
                deposit.Status = deposit.ReturnedAmount > 0m ? "RETURNED" : "CLOSED";
                deposit.ClosedAt ??= now;
            }
            else
            {
                deposit.Status = deposit.ReturnedAmount > 0m
                    ? "PARTIALLY_RETURNED"
                    : deposit.AppliedAmount > 0m ? "RETURN_DUE" : "HELD";
                deposit.ClosedAt = null;
            }
            deposit.UpdatedDate = now;
            deposit.UpdatedBy = "STRIPE_WEBHOOK";

            var idempotencyKey = refund.Reason?.StartsWith("Security deposit return (", StringComparison.Ordinal) == true
                ? refund.Reason["Security deposit return (".Length..].TrimEnd(')')
                : null;
            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                var eventID = $"deposit-return:{deposit.SecurityDepositID}:{idempotencyKey}";
                var returnEvent = await dbContext.PaymentProviderEvents.FirstOrDefaultAsync(
                    x => x.ProviderName == "INTERNAL_SECURITY_DEPOSIT" && x.ProviderEventID == eventID,
                    cancellationToken);
                if (returnEvent != null)
                {
                    var response = JsonSerializer.Deserialize<SecurityDepositReturnResponseDto>(returnEvent.Payload ?? string.Empty);
                    if (response != null)
                    {
                        response.ReturnedAmount = deposit.ReturnedAmount;
                        response.HeldAmount = available;
                        response.Status = deposit.Status;
                        response.RefundStatus = refund.Status;
                        returnEvent.Payload = JsonSerializer.Serialize(response);
                        returnEvent.ProcessedAt = now;
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<decimal> GetPendingReturnAmountAsync(Guid securityDepositID, Guid? exceptRefundID = null)
        {
            var amounts = await dbContext.SecurityDepositTransactions
                .Where(x => x.SecurityDepositID == securityDepositID && x.TransactionType == "RETURN_PENDING"
                    && (!exceptRefundID.HasValue || x.RefundID != exceptRefundID.Value)
                    && x.Refund != null && (x.Refund.Status == "PENDING" || x.Refund.Status == "REQUIRES_ACTION"))
                .Select(x => x.Amount)
                .ToListAsync();
            return amounts.Sum();
        }

        private async Task<decimal> GetAvailableHeldAmountAsync(Guid securityDepositID, SecurityDeposit deposit, Guid? exceptRefundID = null)
        {
            var pendingAmount = await GetPendingReturnAmountAsync(securityDepositID, exceptRefundID);
            return Math.Max(0m, deposit.ReceivedAmount - deposit.AppliedAmount - deposit.ReturnedAmount - pendingAmount);
        }

        private async Task<SecurityDeposit> GetAuthorizedDepositOrThrow(Guid securityDepositID, long actorUserID, bool includeTenantUser = false)
        {
            var organizationIDs = await GetAuthorizedOrganizationIDs(actorUserID);

            IQueryable<SecurityDeposit> query = dbContext.SecurityDeposits;
            if (includeTenantUser)
            {
                query = query.Include(x => x.Tenant)
                    .ThenInclude(x => x!.User);
            }

            var deposit = await query.FirstOrDefaultAsync(x => x.SecurityDepositID == securityDepositID);
            if (deposit == null)
                throw new KeyNotFoundException("Security deposit was not found.");
            if (!organizationIDs.Contains(deposit.OrganizationID))
                throw new UnauthorizedAccessException("You are not authorized to access this security deposit.");

            return deposit;
        }

        private async Task<HashSet<Guid>> GetAuthorizedOrganizationIDs(long actorUserID)
        {
            var memberships = await organizationMemberRepository.GetMemberOrganizationsAsync(actorUserID);
            return memberships
                .Where(x => x.OrganizationID != Guid.Empty &&
                            !string.Equals(x.Status, "DEACTIVATED", StringComparison.OrdinalIgnoreCase) &&
                            (x.IsPrimaryOwner ||
                             string.Equals(x.RoleName, "OWNER", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(x.RoleName, "ADMIN", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(x.RoleName, "LANDLORD", StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.OrganizationID)
                .ToHashSet();
        }

        private static HostSecurityDepositListItemDto MapHostListItem(SecurityDeposit deposit, decimal pendingReturnAmount)
        {
            var tenantName = deposit.Tenant?.User?.DisplayName;
            if (string.IsNullOrWhiteSpace(tenantName))
            {
                var first = deposit.Tenant?.User?.FirstName ?? string.Empty;
                var last = deposit.Tenant?.User?.LastName ?? string.Empty;
                tenantName = string.Join(" ", new[] { first, last }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
            }

            if (string.IsNullOrWhiteSpace(tenantName))
                tenantName = deposit.Tenant?.Description;

            return new HostSecurityDepositListItemDto
            {
                SecurityDepositID = deposit.SecurityDepositID,
                LeaseID = deposit.LeaseID,
                TenantID = deposit.TenantID,
                OrganizationID = deposit.OrganizationID,
                RequiredAmount = deposit.RequiredAmount,
                ReceivedAmount = deposit.ReceivedAmount,
                AppliedAmount = deposit.AppliedAmount,
                ReturnedAmount = deposit.ReturnedAmount,
                HeldAmount = Math.Max(0m, deposit.ReceivedAmount - deposit.AppliedAmount - deposit.ReturnedAmount - pendingReturnAmount),
                Currency = deposit.Currency,
                Status = deposit.Status,
                DueDate = deposit.DueDate,
                HeldAt = deposit.HeldAt,
                ClosedAt = deposit.ClosedAt,
                TenantDisplayName = tenantName,
                TenantEmail = deposit.Tenant?.User?.Email,
                LeaseDisplay = deposit.Lease?.LeaseCode ?? deposit.Lease?.LeaseNumber,
                ListingDisplay = deposit.Lease?.Listing?.Title
            };
        }

        private static HostSecurityDepositSummaryDto BuildSummary(IEnumerable<SecurityDeposit> deposits, IReadOnlyDictionary<Guid, decimal> pendingReturnAmounts)
        {
            var heldTotal = 0m;
            var returnsDueTotal = 0m;
            var reviewNeededCount = 0;

            foreach (var deposit in deposits)
            {
                var held = Math.Max(0m, deposit.ReceivedAmount - deposit.AppliedAmount - deposit.ReturnedAmount
                    - pendingReturnAmounts.GetValueOrDefault(deposit.SecurityDepositID));
                heldTotal += held;

                if (string.Equals(deposit.Status, "RETURN_DUE", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(deposit.Status, "REVIEW_NEEDED", StringComparison.OrdinalIgnoreCase))
                {
                    returnsDueTotal += held;
                }

                if (string.Equals(deposit.Status, "REVIEW_NEEDED", StringComparison.OrdinalIgnoreCase))
                    reviewNeededCount++;
            }

            return new HostSecurityDepositSummaryDto
            {
                HeldAmountTotal = heldTotal,
                ReturnsDueAmountTotal = returnsDueTotal,
                ReviewNeededCount = reviewNeededCount
            };
        }

        private static int ToMinorUnits(decimal amount)
        {
            return Convert.ToInt32(Math.Round(amount * 100m, MidpointRounding.AwayFromZero));
        }

        private static async Task EnsureContentMatchesMimeTypeAsync(IFormFile file, CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();
            var header = new byte[8];
            var read = await stream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
            if (read < 4)
                throw new ArgumentException("Uploaded file is invalid or corrupted.");

            var mime = file.ContentType?.Trim().ToLowerInvariant();
            var isJpeg = header[0] == 0xFF && header[1] == 0xD8;
            var isPng = read >= 8 &&
                        header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                        header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;
            var isPdf = header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46;

            var matches = mime switch
            {
                "image/jpeg" => isJpeg,
                "image/png" => isPng,
                "application/pdf" => isPdf,
                _ => false
            };

            if (!matches)
                throw new ArgumentException("File content does not match the declared content type.");
        }
    }
}
