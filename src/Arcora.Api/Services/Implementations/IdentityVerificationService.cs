// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace Arcora.Api.Services.Implementations
{
    public class IdentityVerificationService : IIdentityVerificationService
    {
        private readonly IMapper mapper;
        private readonly HybridCache cache;
        private readonly ILogger<IdentityVerificationService> logger;
        private readonly IIdentityVerificationRepository identityverificationRepository;
        private readonly IOptions<CacheConfiguration> _options;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityVerificationProviderFactory _identityVerificationProviderFactory;

        private const string NotStartedStatus = "NOT_STARTED";
        private const string PendingStatus = "PENDING";
        private const string SubmittedStatus = "SUBMITTED";
        private const string UnderReviewStatus = "UNDER_REVIEW";
        private const string ApprovedStatus = "APPROVED";
        private const string FailedStatus = "FAILED";
        private const string ExpiredStatus = "EXPIRED";
        private const string CanceledStatus = "CANCELED";

        public IdentityVerificationService(
            IMapper mapper,
            HybridCache cache,
            IOptions<CacheConfiguration> options,
            ILogger<IdentityVerificationService> logger,
            IIdentityVerificationRepository identityverificationRepository,
            UserManager<User> userManager,
            IIdentityVerificationProviderFactory identityVerificationProviderFactory)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.identityverificationRepository = identityverificationRepository;
            this._options = options;
            _userManager = userManager;
            _identityVerificationProviderFactory = identityVerificationProviderFactory;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<IdentityVerificationDto>> GetAll(Paging paging)
        {
            IEnumerable<IdentityVerification> entities;
            try
            {
                entities = cache.Get<IEnumerable<IdentityVerification>>(Cache.IDENTITYVERIFICATIONS.ToString()) ?? new List<IdentityVerification>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.identityverificationRepository.GetIdentityVerificationAsync())?.Where(x => x != null) ?? new List<IdentityVerification>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<IdentityVerification>>(Cache.IDENTITYVERIFICATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching IdentityVerification by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<IdentityVerificationDto>
                {
                    Data = new List<IdentityVerificationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<IdentityVerification> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.VerificationType) && x.VerificationType.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.IdentityVerificationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<IdentityVerificationDto>>(pagedEntities);
            return new PagedResult<IdentityVerificationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<IdentityVerificationDto>> GetForUser(long userID, Paging paging)
        {
            var verifications = await identityverificationRepository.GetByUserIDAsync(userID);
            IEnumerable<IdentityVerification> filtered = verifications;
            if (!string.IsNullOrWhiteSpace(paging.Search))
            {
                filtered = filtered.Where(verification =>
                    !string.IsNullOrWhiteSpace(verification.VerificationType)
                    && verification.VerificationType.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = filtered.Count();
            var pageNumber = Math.Max(1, paging.PageNumber);
            var pageSize = Math.Clamp(paging.PageSize, 1, 50);
            var page = filtered
                .OrderByDescending(verification => verification.UpdatedDate ?? verification.CapturedDate ?? verification.RequestedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<IdentityVerificationDto>
            {
                Data = mapper.Map<IEnumerable<IdentityVerificationDto>>(page),
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<IdentityVerification> entities = cache.Get<IEnumerable<IdentityVerification>>(Cache.IDENTITYVERIFICATIONS.ToString()) ?? new List<IdentityVerification>();
                IdentityVerification? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.IdentityVerificationID == ID);
                }
                else
                {
                    match = await this.identityverificationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<IdentityVerificationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching IdentityVerification by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto> CreateIdentityVerification(IdentityVerificationDto identityverificationDto)
        {
            IdentityVerification identityVerification = new IdentityVerification();
            IEnumerable<IdentityVerification?> checkEntity;
            try
            {
                checkEntity = await this.identityverificationRepository.Find(x => x.VerificationType!.ToLower().Trim() == identityverificationDto.VerificationType!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    identityVerification = this.mapper.Map<IdentityVerification>(identityverificationDto);
                    identityVerification.IdentityVerificationID = Guid.NewGuid();
                    identityVerification.ProviderReferenceID = string.IsNullOrEmpty(identityverificationDto.ProviderReferenceID) ? null : identityverificationDto.ProviderReferenceID;
                    identityVerification.CapturedDate = DateTime.UtcNow;
                    identityVerification = await identityverificationRepository.Create(identityVerification) ?? new IdentityVerification();
                    await identityverificationRepository.Save();
                    cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating IdentityVerification. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<IdentityVerificationDto>(identityVerification);
        }

        /// <inheritdoc/>
        public async Task<StartIdentityVerificationResponseDto> StartIdentityVerificationAsync(StartIdentityVerificationRequestDto request, CancellationToken cancellationToken = default)
        {
            var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
            var user = await _userManager.FindByIdAsync(request.UserID.ToString());
            if (user == null)
                throw new KeyNotFoundException("User with the specified ID was not found.");

            var provider = _identityVerificationProviderFactory.GetProvider(request.ProviderName);
            var verificationType = string.IsNullOrWhiteSpace(request.VerificationType)
                ? "IDENTITY"
                : request.VerificationType.Trim().ToUpperInvariant();

            logger.LogInformation(
                "Starting identity verification. CorrelationId: {CorrelationId}, UserId: {UserId}, Provider: {Provider}, VerificationType: {VerificationType}",
                correlationId,
                request.UserID,
                provider.ProviderName,
                verificationType);

            var providerName = provider.ProviderName.Trim().ToUpperInvariant();
            var existingRecords = await identityverificationRepository.Find(record =>
                record.UserID == request.UserID
                && record.ProviderName != null
                && record.ProviderName.ToUpper() == providerName
                && record.VerificationType != null
                && record.VerificationType.ToUpper() == verificationType);

            var activeRecords = (existingRecords ?? Enumerable.Empty<IdentityVerification>())
                .Where(record => IsActiveStatus(record.Status))
                .OrderByDescending(record => record.UpdatedDate ?? record.CapturedDate ?? record.RequestedAt)
                .ToList();

            var reuseCandidate = activeRecords.FirstOrDefault();
            foreach (var staleRecord in activeRecords.Skip(1))
            {
                staleRecord.Status = CanceledStatus;
                staleRecord.FailureReason = "Superseded by a newer verification attempt.";
                staleRecord.UpdatedDate = DateTime.UtcNow;
                staleRecord.UpdatedBy = string.IsNullOrWhiteSpace(request.CapturedBy) ? user.Email : request.CapturedBy.Trim();
                await identityverificationRepository.Update(staleRecord);
            }

            if (activeRecords.Count > 1)
            {
                logger.LogInformation(
                    "Superseded duplicate active identity verifications. CorrelationId: {CorrelationId}, UserId: {UserId}, Provider: {Provider}, VerificationType: {VerificationType}, SupersededCount: {SupersededCount}",
                    correlationId,
                    request.UserID,
                    providerName,
                    verificationType,
                    activeRecords.Count - 1);
            }

            var providerRequest = new IdentityVerificationProviderRequest(
                request.UserID,
                verificationType,
                request.UserID.ToString(),
                user.Email,
                user.FirstName,
                user.LastName,
                request.RedirectUrl);

            var providerResult = await provider.StartVerificationAsync(providerRequest, cancellationToken);
            var normalizedStatus = NormalizeStatus(providerResult.Status);
            var now = DateTime.UtcNow;
            var updatedBy = string.IsNullOrWhiteSpace(request.CapturedBy) ? user.Email : request.CapturedBy.Trim();

            var identityVerification = reuseCandidate ?? new IdentityVerification
            {
                IdentityVerificationID = Guid.NewGuid(),
                UserID = request.UserID,
                VerificationType = verificationType,
                ProviderName = providerResult.ProviderName,
                CapturedBy = updatedBy,
                CapturedDate = now,
                RequestedAt = now
            };

            identityVerification.ProviderName = providerResult.ProviderName;
            identityVerification.ProviderReferenceID = providerResult.ProviderReferenceId;
            identityVerification.Status = normalizedStatus;
            identityVerification.RequestedAt = now;
            identityVerification.VerifiedAt = normalizedStatus == ApprovedStatus ? now : null;
            identityVerification.ExpiresAt = providerResult.ExpiresAt;
            identityVerification.FailureReason = providerResult.FailureReason;
            identityVerification.UpdatedBy = updatedBy;
            identityVerification.UpdatedDate = now;

            if (reuseCandidate is null)
                await identityverificationRepository.Create(identityVerification);
            else
                await identityverificationRepository.Update(identityVerification);

            await identityverificationRepository.Save();
            cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());

            logger.LogInformation(
                "Identity verification started. CorrelationId: {CorrelationId}, IdentityVerificationId: {IdentityVerificationId}, Provider: {Provider}, Status: {Status}, ReusedRecord: {ReusedRecord}",
                correlationId,
                identityVerification.IdentityVerificationID,
                identityVerification.ProviderName,
                identityVerification.Status,
                reuseCandidate is not null);

            return new StartIdentityVerificationResponseDto
            {
                IdentityVerificationID = identityVerification.IdentityVerificationID,
                UserID = identityVerification.UserID,
                VerificationType = identityVerification.VerificationType ?? verificationType,
                ProviderName = identityVerification.ProviderName ?? provider.ProviderName,
                ProviderReferenceID = identityVerification.ProviderReferenceID,
                Status = identityVerification.Status ?? PendingStatus,
                VerificationUrl = providerResult.VerificationUrl,
                ClientToken = providerResult.ClientToken,
                FailureReason = identityVerification.FailureReason,
                RequestedAt = identityVerification.RequestedAt,
                ExpiresAt = identityVerification.ExpiresAt
            };
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto?> RefreshVerificationStatusAsync(
            long userId,
            Guid identityVerificationId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var identityVerification = await identityverificationRepository.GetByID(identityVerificationId);
            if (identityVerification is null || identityVerification.UserID != userId)
                return null;

            if (string.IsNullOrWhiteSpace(identityVerification.ProviderName)
                || string.IsNullOrWhiteSpace(identityVerification.ProviderReferenceID))
            {
                return mapper.Map<IdentityVerificationDto>(identityVerification);
            }

            var provider = _identityVerificationProviderFactory.GetProvider(identityVerification.ProviderName);
            var providerResult = await provider.GetVerificationStatusAsync(identityVerification.ProviderReferenceID, cancellationToken);

            var updated = await UpdateProviderVerificationStatusAsync(
                provider.ProviderName,
                identityVerification.ProviderReferenceID,
                providerResult.Status,
                providerResult.FailureReason,
                $"{provider.ProviderName}_POLL",
                cancellationToken);

            return updated ?? mapper.Map<IdentityVerificationDto>(identityVerification);
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto?> UpdateIdentityVerification(Guid id, IdentityVerificationDto identityverificationDto)
        {
            try
            {
                var existing = await this.identityverificationRepository.GetByID(id);
                if (existing == null)
                    return null;
                IdentityVerification identityVerification = this.mapper.Map<IdentityVerification>(identityverificationDto);
                identityVerification = await identityverificationRepository.Update(identityVerification) ?? new IdentityVerification();
                await identityverificationRepository.Save();
                cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());
                identityverificationDto = this.mapper.Map<IdentityVerificationDto>(identityVerification);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating IdentityVerification. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return identityverificationDto;
        }

        /// <inheritdoc/>
        public async Task DeleteIdentityVerification(Guid ID)
        {
            try
            {
                var identityVerification = await this.identityverificationRepository.GetByID(ID);
                if (identityVerification == null)
                    throw new KeyNotFoundException("IdentityVerification with the specified ID was not found.");
                await identityverificationRepository.Delete(identityVerification);
                await identityverificationRepository.Save();
                cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting IdentityVerification . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto?> UpdateIdentityVerificationStatus(Guid id, string status)
        {
            var identityVerification = await identityverificationRepository.GetByID(id);
            if (identityVerification == null)
                return null;

            var normalizedStatus = status.Equals("Revert", StringComparison.OrdinalIgnoreCase)
                ? PendingStatus
                : NormalizeStatus(status);

            identityVerification.Status = normalizedStatus;
            identityVerification.UpdatedDate = DateTime.UtcNow;
            if (normalizedStatus == ApprovedStatus)
                identityVerification.VerifiedAt ??= DateTime.UtcNow;

            await identityverificationRepository.Update(identityVerification);
            await identityverificationRepository.Save();
            cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());
            return this.mapper.Map<IdentityVerificationDto>(identityVerification);
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto?> UpdateProviderVerificationStatusAsync(
            string providerName,
            string providerReferenceId,
            string status,
            string? failureReason,
            string? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
            var normalizedProviderName = providerName?.Trim().ToUpperInvariant() ?? string.Empty;
            var normalizedStatus = normalizedProviderName == "PERSONA"
                ? NormalizePersonaStatus(status)
                : NormalizeStatus(status);

            var matches = await identityverificationRepository.Find(x =>
                x.ProviderReferenceID == providerReferenceId &&
                x.ProviderName != null &&
                x.ProviderName.ToUpper() == normalizedProviderName);

            var identityVerification = matches?.FirstOrDefault();
            if (identityVerification == null)
            {
                logger.LogWarning(
                    "Identity verification callback ignored. CorrelationId: {CorrelationId}, Provider: {Provider}, ProviderReferenceId: {ProviderReferenceId}",
                    correlationId,
                    normalizedProviderName,
                    providerReferenceId);
                return null;
            }

            var now = DateTime.UtcNow;
            identityVerification.Status = normalizedStatus;
            identityVerification.FailureReason = string.IsNullOrWhiteSpace(failureReason) ? null : failureReason.Trim();
            identityVerification.UpdatedDate = now;
            identityVerification.UpdatedBy = string.IsNullOrWhiteSpace(updatedBy)
                ? $"{normalizedProviderName}_WEBHOOK"
                : updatedBy.Trim();

            if (normalizedStatus == ApprovedStatus)
                identityVerification.VerifiedAt ??= now;
            if (normalizedStatus == ExpiredStatus)
                identityVerification.ExpiresAt ??= now;

            await identityverificationRepository.Update(identityVerification);
            await identityverificationRepository.Save();
            cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());

            logger.LogInformation(
                "Identity verification callback processed. CorrelationId: {CorrelationId}, IdentityVerificationId: {IdentityVerificationId}, Provider: {Provider}, Status: {Status}",
                correlationId,
                identityVerification.IdentityVerificationID,
                normalizedProviderName,
                normalizedStatus);

            return mapper.Map<IdentityVerificationDto>(identityVerification);
        }

        private static bool IsActiveStatus(string? status)
        {
            var normalized = NormalizeStatus(status);
            return normalized is NotStartedStatus or PendingStatus or SubmittedStatus or UnderReviewStatus;
        }

        private static string NormalizePersonaStatus(string? status)
        {
            var normalized = status?.Trim().Replace("-", string.Empty).Replace("_", string.Empty).ToUpperInvariant();
            return normalized is "COMPLETE" or "COMPLETED"
                ? SubmittedStatus
                : NormalizeStatus(status);
        }

        private static string NormalizeStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return PendingStatus;

            var normalized = status.Trim().Replace("-", string.Empty).Replace("_", string.Empty).ToUpperInvariant();
            return normalized switch
            {
                "NOTSTARTED" or "NEW" => NotStartedStatus,
                "PENDING" or "INITIATED" or "STARTED" or "CREATED" or "INPROGRESS" or "PROCESSING" or "AWAITINGUSER" => PendingStatus,
                "REVIEW" or "UNDERREVIEW" or "NEEDSREVIEW" or "REVIEWREQUIRED" => UnderReviewStatus,
                "APPROVED" or "VERIFIED" or "PASSED" or "SUCCESS" or "COMPLETE" or "COMPLETED" => ApprovedStatus,
                "FAILED" or "DECLINED" or "REJECTED" or "ERROR" => FailedStatus,
                "EXPIRED" => ExpiredStatus,
                "CANCELED" or "CANCELLED" => CanceledStatus,
                _ => PendingStatus
            };
        }
    }
}