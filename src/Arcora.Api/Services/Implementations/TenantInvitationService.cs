// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Arcora.Api;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Arcora.Api.Email;
using Arcora.Api.Exceptions;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace Arcora.Api.Services.Implementations
{
    public class TenantInvitationService : ITenantInvitationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TenantInvitationService> logger;
        private readonly ITenantInvitationRepository tenantinvitationRepository;
        private readonly IOptions<CacheConfiguration> _options;
        private readonly ArcoraDbContext dbContext;
        private readonly IListingService listingService;
        private readonly IEmailQueue emailQueue;
        private readonly IPreferenceService preferenceService;
        private readonly IConfiguration configuration;
        private readonly IPaymentOnboardingService paymentOnboardingService;
        private readonly IDataProtector emailProtector;
        private const int TenantInvitationTokenValidDays = 3;

        public TenantInvitationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TenantInvitationService> logger, ITenantInvitationRepository tenantinvitationRepository, ArcoraDbContext dbContext, IListingService listingService, IEmailQueue emailQueue, IPreferenceService preferenceService, IConfiguration configuration, IPaymentOnboardingService paymentOnboardingService, IDataProtectionProvider dataProtection)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenantinvitationRepository = tenantinvitationRepository;
            this.dbContext = dbContext;
            this.listingService = listingService;
            this.emailQueue = emailQueue;
            this.preferenceService = preferenceService;
            this.configuration = configuration;
            this.paymentOnboardingService = paymentOnboardingService;
            this.emailProtector = dataProtection.CreateProtector(TenantInvitationEmailWorker.ProtectionPurpose);
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenantInvitationDto>> GetAll(Paging paging)
        {
            IEnumerable<TenantInvitation> entities;
            try
            {
                entities = cache.Get<IEnumerable<TenantInvitation>>(Cache.TENANTINVITATIONS.ToString()) ?? new List<TenantInvitation>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.tenantinvitationRepository.GetTenantInvitationAsync())?.Where(x => x != null) ?? new List<TenantInvitation>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<TenantInvitation>>(Cache.TENANTINVITATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantInvitation by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenantInvitationDto>
                {
                    Data = new List<TenantInvitationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TenantInvitation> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenantInvitationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenantInvitationDto>>(pagedEntities);
            return new PagedResult<TenantInvitationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        public async Task<List<TenantInvitationDto>> GetByOrganizationAsync(Guid organizationID)
        {
            var createdSince = DateTime.UtcNow.AddMonths(-6);
            var invitations = await this.dbContext.TenantInvitations
                .AsNoTracking()
                .Where(invitation => invitation.ListingID.HasValue
                    && invitation.CapturedDate >= createdSince
                    && this.dbContext.Listings.Any(listing =>
                        listing.ListingID == invitation.ListingID.Value
                        && listing.OrganizationID == organizationID))
                .OrderByDescending(invitation => invitation.CapturedDate)
                .ToListAsync();

            var results = this.mapper.Map<List<TenantInvitationDto>>(invitations);
            var ids = invitations.Select(i => i.TenantInvitationID).ToList();
            var emails = await dbContext.TenantInvitationEmails.AsNoTracking()
                .Where(email => ids.Contains(email.TenantInvitationID)).ToDictionaryAsync(email => email.TenantInvitationID);
            foreach (var invitation in results)
            {
                invitation.OrganizationID = organizationID;
                if (invitation.TenantInvitationID.HasValue && emails.TryGetValue(invitation.TenantInvitationID.Value, out var email))
                    ApplyDeliveryStatus(invitation, email);
            }
            return results;
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<TenantInvitation> entities = cache.Get<IEnumerable<TenantInvitation>>(Cache.TENANTINVITATIONS.ToString()) ?? new List<TenantInvitation>();
                TenantInvitation? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TenantInvitationID == ID);
                }
                else
                {
                    match = await this.tenantinvitationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TenantInvitationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantInvitation by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationDto> CreateTenantInvitation(TenantInvitationDto tenantinvitationDto)
        {
            TenantInvitation tenantInvitation = new TenantInvitation();
            IEnumerable<TenantInvitation?> checkEntity;
            try
            {
                checkEntity = await this.tenantinvitationRepository.Find(x => x.Email!.ToLower().Trim() == tenantinvitationDto.Email!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenantInvitation = this.mapper.Map<TenantInvitation>(tenantinvitationDto);
                    tenantInvitation.TenantInvitationID = Guid.NewGuid();
                    tenantInvitation.LeaseID = tenantinvitationDto.LeaseID == Guid.Empty ? null : tenantinvitationDto.LeaseID;
                    tenantInvitation.RentalApplicationID = tenantinvitationDto.RentalApplicationID == Guid.Empty ? null : tenantinvitationDto.RentalApplicationID;
                    tenantInvitation.CapturedDate = DateTime.UtcNow;
                    tenantInvitation = await tenantinvitationRepository.Create(tenantInvitation) ?? new TenantInvitation();
                    await tenantinvitationRepository.Save();
                    cache.Remove(Cache.TENANTINVITATIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TenantInvitation. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenantInvitationDto>(tenantInvitation);
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationDto?> UpdateTenantInvitation(Guid id, TenantInvitationDto tenantinvitationDto)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            await LockInvitationListingAsync(id);
            var existing = await dbContext.TenantInvitations.SingleOrDefaultAsync(i => i.TenantInvitationID == id);
            if (existing == null) return null;
            if (existing.Status != "PENDING" || existing.ExpiresAt <= DateTime.UtcNow)
                throw new ApiProblemException(409, "Invitation not editable", "Only pending, unexpired invitations can be edited.");
            if (tenantinvitationDto.ListingID != existing.ListingID
                || tenantinvitationDto.LeaseID != existing.LeaseID
                || tenantinvitationDto.RentalApplicationID != existing.RentalApplicationID
                || tenantinvitationDto.Email != existing.Email
                || tenantinvitationDto.TokenHash != existing.TokenHash
                || tenantinvitationDto.Status != existing.Status
                || tenantinvitationDto.ExpiresAt != existing.ExpiresAt
                || tenantinvitationDto.Price != existing.MonthlyRentAmount
                || tenantinvitationDto.SecurityDepositAmount != existing.SecurityDepositAmount
                || tenantinvitationDto.Currency != existing.Currency
                || tenantinvitationDto.StartDate != existing.StartDate
                || tenantinvitationDto.EndDate != existing.EndDate
                || tenantinvitationDto.LeaseTermMonths != existing.LeaseTermMonths)
                throw new ApiProblemException(400, "Immutable invitation terms", "Invitation identity, recipient, token, dates and status cannot be changed. Revoke and create a new invitation.");
            existing.Name = tenantinvitationDto.Name;
            existing.PhoneNumber = tenantinvitationDto.PhoneNumber;
            existing.InvitationPurpose = tenantinvitationDto.InvitationPurpose;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            cache.Remove(Cache.TENANTINVITATIONS.ToString());
            return mapper.Map<TenantInvitationDto>(existing);
        }

        /// <inheritdoc/>
        public async Task DeleteTenantInvitation(Guid ID)
        {
            try
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync();
                await LockInvitationListingAsync(ID);
                var tenantInvitation = await dbContext.TenantInvitations.SingleOrDefaultAsync(i => i.TenantInvitationID == ID);
                if (tenantInvitation == null)
                    throw new KeyNotFoundException("TenantInvitation with the specified ID was not found.");
                if (tenantInvitation.LeaseID.HasValue)
                    throw new ApiProblemException(409, "Invitation has a lease", "An invitation linked to a lease cannot be deleted.");
                await ReleaseInvitationHoldsAsync(ID);
                var email = await dbContext.TenantInvitationEmails.SingleOrDefaultAsync(e => e.TenantInvitationID == ID);
                if (email != null) dbContext.TenantInvitationEmails.Remove(email);
                dbContext.TenantInvitations.Remove(tenantInvitation);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                cache.Remove(Cache.TENANTINVITATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TenantInvitation . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationDto?> UpdateTenantInvitationStatus(Guid id, string status)
        {
            if (!string.Equals(status, "REVOKED", StringComparison.OrdinalIgnoreCase))
                throw new ApiProblemException(400, "Invalid status transition", "Landlords can only revoke an invitation. Tenant responses must use the invitation link.");
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            await LockInvitationListingAsync(id);
            var tenantInvitation = await dbContext.TenantInvitations.SingleOrDefaultAsync(i => i.TenantInvitationID == id);
            if (tenantInvitation == null)
                return null;
            if (tenantInvitation.Status == "REVOKED")
                return mapper.Map<TenantInvitationDto>(tenantInvitation);
            if (tenantInvitation.LeaseID.HasValue || tenantInvitation.Status is not ("PENDING" or "ACCEPTED"))
                throw new ApiProblemException(409, "Invitation not revocable", "Only pending or accepted invitations without a lease can be revoked.");
            tenantInvitation.Status = "REVOKED";
            tenantInvitation.RevokedAt = DateTime.UtcNow;
            await ReleaseInvitationHoldsAsync(id);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            cache.Remove(Cache.TENANTINVITATIONS.ToString());
            cache.Remove(Cache.RESERVATIONHOLDS.ToString());
            return this.mapper.Map<TenantInvitationDto>(tenantInvitation);
        }

        /// <inheritdoc/>
        public async Task<CreateTenantInvitationResultDto> CreateTenantInvitationWithHold(CreateTenantInvitationRequestDto request, long signedInUserID)
        {
            if (request == null || request.OrganizationID == Guid.Empty || request.ListingID == Guid.Empty || request.LeaseTermMonths <= 0)
            {
                return new CreateTenantInvitationResultDto
                {
                    Success = false,
                    Message = "organizationId, listingId, startDate and a positive leaseTermMonths are required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new CreateTenantInvitationResultDto
                {
                    Success = false,
                    Message = "Email is required to send the tenant invitation."
                };
            }

            if (!await InvitationAuthorization.CanManageAsync(dbContext, request.OrganizationID, signedInUserID))
                throw new ApiProblemException(403, "Forbidden", "Active organization management permission is required.");
            request.CapturedBy = signedInUserID.ToString(CultureInfo.InvariantCulture);

            if (request.RentalApplicationID.HasValue && request.RentalApplicationID.Value != Guid.Empty
                && !await dbContext.RentalApplications.AnyAsync(application =>
                    application.RentalApplicationID == request.RentalApplicationID.Value
                    && application.ListingID == request.ListingID))
                throw new ApiProblemException(400, "Invalid application", "The application does not belong to the selected listing.");

            var listing = await this.dbContext.Listings
                .AsNoTracking()
                .Include(l => l.ListingTermPrices)
                .FirstOrDefaultAsync(l => l.ListingID == request.ListingID && l.OrganizationID == request.OrganizationID);

            if (listing == null)
            {
                return new CreateTenantInvitationResultDto
                {
                    Success = false,
                    Message = "Listing was not found for the provided organization."
                };
            }

            var organization = await this.dbContext.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrganizationID == request.OrganizationID);

            Lease? sourceLease = null;
            if (request.LeaseID.HasValue && request.LeaseID.Value != Guid.Empty)
            {
                sourceLease = await this.dbContext.Leases
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.LeaseID == request.LeaseID.Value
                        && l.OrganizationID == request.OrganizationID
                        && l.ListingID == request.ListingID);

                if (sourceLease == null)
                {
                    return new CreateTenantInvitationResultDto
                    {
                        Success = false,
                        Message = "Lease was not found for the provided listing and organization."
                    };
                }
            }

            var strategy = this.dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await this.dbContext.Database.BeginTransactionAsync();
                try
                {
                    await dbContext.AcquireListingLockAsync(request.ListingID);
                    var availability = await this.listingService.GetListingAvailability(
                        request.OrganizationID,
                        request.ListingID,
                        request.StartDate,
                        request.LeaseTermMonths);

                    if (availability == null)
                    {
                        await transaction.RollbackAsync();
                        return new CreateTenantInvitationResultDto
                        {
                            Success = false,
                            Message = "Unable to evaluate listing availability.",
                        };
                    }

                    if (!availability.IsAvailable)
                    {
                        await transaction.RollbackAsync();
                        return new CreateTenantInvitationResultDto
                        {
                            Success = false,
                            Message = "Listing is no longer available for the requested date range.",
                            Availability = availability
                        };
                    }

                    var resolvedTermPrice = listing.ListingTermPrices?
                        .FirstOrDefault(tp => tp.IsActive
                            && tp.LeaseTermMonths == request.LeaseTermMonths
                            && (!tp.EffectiveFrom.HasValue || tp.EffectiveFrom.Value.Date <= request.StartDate.Date)
                            && (!tp.EffectiveTo.HasValue || tp.EffectiveTo.Value.Date >= request.StartDate.Date));

                    var agreedMonthlyRent = sourceLease?.BaseRentAmount
                        ?? resolvedTermPrice?.MonthlyRentAmount
                        ?? listing.BaseMonthlyRentAmount;

                    var agreedDeposit = resolvedTermPrice?.SecurityDepositAmount
                        ?? listing.SecurityDepositAmount;

                    var agreedCurrency = sourceLease?.Currency
                        ?? listing.Currency
                        ?? "CAD";
                    agreedCurrency = agreedCurrency.Trim().ToUpperInvariant();
                    if (agreedMonthlyRent < 0 || agreedDeposit < 0
                        || agreedCurrency.Length != 3 || agreedCurrency.Any(c => c < 'A' || c > 'Z'))
                        throw new ApiProblemException(409, "Invalid invitation quote",
                            "The listing or lease contains invalid pricing or currency. Correct it before sending an invitation.");

                    var invitationExpiresAt = DateTime.UtcNow.AddDays(TenantInvitationTokenValidDays);
                    var invitationId = Guid.NewGuid();
                    var generatedToken = GenerateTenantInvitationToken(invitationId, request.Email!.Trim(), invitationExpiresAt);
                    var invitedTenantName = await ResolveInvitedTenantDisplayNameAsync(request);

                    var invitation = new TenantInvitation
                    {
                        TenantInvitationID = invitationId,
                        ListingID = request.ListingID,
                        LeaseID = request.LeaseID,
                        RentalApplicationID = request.RentalApplicationID,
                        InvitationPurpose = request.InvitationPurpose,
                        Email = request.Email,
                        Name = invitedTenantName,
                        PhoneNumber = request.PhoneNumber,
                        MonthlyRentAmount = agreedMonthlyRent,
                        SecurityDepositAmount = agreedDeposit,
                        Currency = agreedCurrency,
                        StartDate = request.StartDate.Date,
                        EndDate = request.StartDate.Date.AddMonths(request.LeaseTermMonths),
                        LeaseTermMonths = request.LeaseTermMonths,
                        ReservationHoldID = Guid.NewGuid(),
                        TokenHash = HashToken(generatedToken),
                        Status = "PENDING",
                        ExpiresAt = invitationExpiresAt,
                        CapturedBy = request.CapturedBy,
                        CapturedDate = DateTime.UtcNow
                    };

                    var hold = new ReservationHold
                    {
                        ReservationHoldID = invitation.ReservationHoldID.Value,
                        ListingID = request.ListingID,
                        RentalApplicationID = request.RentalApplicationID,
                        TenantID = request.TenantID,
                        StartDate = request.StartDate.Date,
                        EndDate = request.StartDate.Date.AddMonths(request.LeaseTermMonths),
                        HoldReason = $"Tenant invitation {invitation.TenantInvitationID} | {request.LeaseTermMonths}mo | {agreedMonthlyRent:0.##} {agreedCurrency} | deposit {agreedDeposit:0.##}",
                        Status = "ACTIVE",
                        ExpiresAt = invitationExpiresAt,
                        CapturedBy = request.CapturedBy,
                        CapturedDate = DateTime.UtcNow
                    };

                    await this.dbContext.TenantInvitations.AddAsync(invitation);
                    await this.dbContext.ReservationHolds.AddAsync(hold);
                    var message = await this.BuildTenantInvitationEmailAsync(
                            tenantEmail: request.Email!.Trim(),
                            tenantDisplayName: invitation.Name ?? request.Email!.Trim(),
                            organizationName: organization?.DisplayName ?? organization?.LegalName ?? "Landlord",
                            listingTitle: string.IsNullOrWhiteSpace(listing.Title) ? "Listing" : listing.Title!,
                            invitationPurpose: string.IsNullOrWhiteSpace(request.InvitationPurpose) ? "TENANT_INVITATION" : request.InvitationPurpose!,
                            startDate: hold.StartDate,
                            endDate: hold.EndDate,
                            leaseTermMonths: request.LeaseTermMonths,
                            monthlyRentAmount: agreedMonthlyRent,
                            securityDepositAmount: agreedDeposit,
                            currency: agreedCurrency,
                            expiresAt: invitationExpiresAt,
                            invitationToken: generatedToken);
                    var email = new TenantInvitationEmail
                    {
                        TenantInvitationID = invitationId,
                        ProtectedMessage = emailProtector.Protect(JsonSerializer.Serialize(message)),
                        NextAttemptAt = DateTime.UtcNow
                    };
                    dbContext.TenantInvitationEmails.Add(email);
                    await this.dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    cache.Remove(Cache.TENANTINVITATIONS.ToString());
                    cache.Remove(Cache.RESERVATIONHOLDS.ToString());

                    var invitationDto = this.mapper.Map<TenantInvitationDto>(invitation);
                    invitationDto.OrganizationID = request.OrganizationID;
                    invitationDto.StartDate = hold.StartDate;
                    invitationDto.LeaseTermMonths = request.LeaseTermMonths;
                    invitationDto.Price = agreedMonthlyRent;
                    ApplyDeliveryStatus(invitationDto, email);

                    return new CreateTenantInvitationResultDto
                    {
                        Success = true,
                        Message = "Invitation and reservation hold created. Email delivery is pending.",
                        Availability = availability,
                        TenantInvitation = invitationDto,
                        ReservationHold = this.mapper.Map<ReservationHoldDto>(hold)
                    };
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while creating tenant invitation with reservation hold. Timestamp: {Timestamp}", DateTime.UtcNow);
                    throw;
                }
            });
        }

        private async Task<EmailMessage> BuildTenantInvitationEmailAsync(
            string tenantEmail,
            string tenantDisplayName,
            string organizationName,
            string listingTitle,
            string invitationPurpose,
            DateTime startDate,
            DateTime endDate,
            short leaseTermMonths,
            decimal monthlyRentAmount,
            decimal securityDepositAmount,
            string currency,
            DateTime expiresAt,
            string invitationToken)
        {
            var (companyName, companyEmail) = await GetCompanyInfoAsync();
            var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;

            var frontendUrl = (this.configuration["FrontendUrl"] ?? string.Empty).TrimEnd('/');
            if (!Uri.TryCreate(frontendUrl, UriKind.Absolute, out var frontendUri)
                || (frontendUri.Scheme != Uri.UriSchemeHttps && frontendUri.Scheme != Uri.UriSchemeHttp))
                throw new InvalidOperationException("A valid HTTP(S) FrontendUrl is required to create invitation links.");
            var acceptUrl = string.IsNullOrWhiteSpace(frontendUrl)
                ? string.Empty
                : $"{frontendUrl}/tenant-invitation/respond?token={Uri.EscapeDataString(invitationToken)}&response=ACCEPT";
            var declineUrl = string.IsNullOrWhiteSpace(frontendUrl)
                ? string.Empty
                : $"{frontendUrl}/tenant-invitation/respond?token={Uri.EscapeDataString(invitationToken)}&response=DECLINE";

            var money = string.IsNullOrWhiteSpace(currency) ? "CAD" : currency.ToUpperInvariant();
            var body = EmailTemplates.BuildTenantInvitationEmail(
                tenantEmail: string.IsNullOrWhiteSpace(tenantDisplayName) ? tenantEmail : tenantDisplayName,
                organizationName: organizationName,
                listingTitle: listingTitle,
                startDate: startDate.ToString("dddd, dd MMM yyyy"),
                endDate: endDate.ToString("dddd, dd MMM yyyy"),
                leaseTerm: $"{leaseTermMonths} month(s)",
                monthlyRent: $"{monthlyRentAmount.ToString("N2", CultureInfo.InvariantCulture)} {money}",
                securityDeposit: $"{securityDepositAmount.ToString("N2", CultureInfo.InvariantCulture)} {money}",
                invitationPurpose: invitationPurpose,
                expiresAt: expiresAt.ToString("dddd, dd MMM yyyy 'at' HH:mm 'UTC'"),
                acceptUrl: acceptUrl,
                declineUrl: declineUrl,
                companyName: brand,
                supportEmail: companyEmail);

            return new EmailMessage(
                To: tenantEmail,
                Subject: $"Rental invitation from {organizationName}",
                HtmlBody: body);
        }

        private async Task<string> ResolveInvitedTenantDisplayNameAsync(CreateTenantInvitationRequestDto request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name))
                return request.Name.Trim();

            var fallback = request.Email?.Trim() ?? string.Empty;

            if (request.RentalApplicationID.HasValue && request.RentalApplicationID.Value != Guid.Empty)
            {
                var occupants = await this.dbContext.ApplicationOccupants
                    .AsNoTracking()
                    .Where(o => o.RentalApplicationID == request.RentalApplicationID.Value)
                    .ToListAsync();

                var matchingOccupant = occupants.FirstOrDefault(o =>
                    !string.IsNullOrWhiteSpace(o.Email)
                    && !string.IsNullOrWhiteSpace(request.Email)
                    && string.Equals(o.Email.Trim(), request.Email.Trim(), StringComparison.OrdinalIgnoreCase));

                var selectedOccupant = matchingOccupant
                    ?? occupants.FirstOrDefault(o => o.IsPrimaryApplicant)
                    ?? occupants.FirstOrDefault();

                var occupantName = $"{selectedOccupant?.FirstName} {selectedOccupant?.LastName}".Trim();
                if (!string.IsNullOrWhiteSpace(occupantName))
                    return occupantName;
            }

            var tenantId = request.TenantID;
            if ((!tenantId.HasValue || tenantId.Value == Guid.Empty)
                && request.RentalApplicationID.HasValue
                && request.RentalApplicationID.Value != Guid.Empty)
            {
                tenantId = await this.dbContext.RentalApplications
                    .AsNoTracking()
                    .Where(r => r.RentalApplicationID == request.RentalApplicationID.Value)
                    .Select(r => (Guid?)r.TenantID)
                    .FirstOrDefaultAsync();
            }

            if (tenantId.HasValue && tenantId.Value != Guid.Empty)
            {
                var tenantName = await this.dbContext.Tenants
                    .AsNoTracking()
                    .Where(t => t.TenantID == tenantId.Value)
                    .Select(t => t.Description)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrWhiteSpace(tenantName))
                    return tenantName.Trim();
            }

            return fallback;
        }

        private async Task<(string CompanyName, string CompanyEmail)> GetCompanyInfoAsync()
        {
            try
            {
                var preference = await preferenceService.GetPreference();
                if (preference != null)
                    return (preference.CompanyName ?? "", preference.CompanyEmail ?? "");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to load company preferences for invitation email.");
                throw;
            }

            return ("", "");
        }

        private async Task LockInvitationListingAsync(Guid id)
        {
            var listingID = await dbContext.TenantInvitations.AsNoTracking()
                .Where(i => i.TenantInvitationID == id).Select(i => i.ListingID).FirstOrDefaultAsync();
            if (listingID.HasValue)
                await dbContext.AcquireListingLockAsync(listingID.Value);
        }

        private async Task ReleaseInvitationHoldsAsync(Guid id)
        {
            var key = $"Tenant invitation {id} |";
            var holds = await dbContext.ReservationHolds
                .Where(h => h.HoldReason != null && h.HoldReason.StartsWith(key) && h.ReleasedAt == null)
                .ToListAsync();
            foreach (var hold in holds)
            {
                hold.Status = "RELEASED";
                hold.ReleasedAt = DateTime.UtcNow;
            }
            cache.Remove(Cache.RESERVATIONHOLDS.ToString());
        }

        private static void ApplyDeliveryStatus(TenantInvitationDto dto, TenantInvitationEmail email)
        {
            dto.EmailDeliveryStatus = email.Status;
            dto.EmailDeliveryAttempts = email.Attempts;
            dto.EmailSentAt = email.SentAt;
        }

        public async Task<TenantInvitationDto> GetAcceptedInvitationForUserAsync(Guid id, long userID)
        {
            var email = await dbContext.Users.AsNoTracking().Where(user => user.Id == userID && userID > 0)
                .Select(user => user.Email).FirstOrDefaultAsync();
            var invitation = await dbContext.TenantInvitations.AsNoTracking()
                .SingleOrDefaultAsync(i => i.TenantInvitationID == id);
            if (string.IsNullOrWhiteSpace(email) || invitation == null
                || !string.Equals(email.Trim(), invitation.Email?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new ApiProblemException(404, "Invitation not found", "No invitation was found for your account.");
            if (invitation.Status != "ACCEPTED")
                throw new ApiProblemException(409, "Invitation not accepted", "This invitation is no longer accepted.");
            var dto = mapper.Map<TenantInvitationDto>(invitation);
            dto.TokenHash = null;
            return dto;
        }

        public async Task<TenantInvitationDto?> RetryInvitationEmailAsync(Guid id)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            await TenantInvitationEmailWorker.LockAsync(dbContext, id);
            var email = await dbContext.TenantInvitationEmails.Include(e => e.TenantInvitation)
                .SingleOrDefaultAsync(e => e.TenantInvitationID == id);
            if (email?.TenantInvitation == null)
                throw new ApiProblemException(404, "Delivery job not found", "No retryable email job exists for this invitation.");
            var invitation = email.TenantInvitation;
            if (invitation.ExpiresAt <= DateTime.UtcNow || invitation.Status != "PENDING")
                throw new ApiProblemException(409, "Invitation not pending", "Only pending, unexpired invitation emails can be retried.");
            if (email.Status != "FAILED")
                throw new ApiProblemException(409, "Email not retryable", "Only failed email deliveries can be retried.");
            if (email.LastAttemptAt > DateTime.UtcNow.AddSeconds(-30))
                throw new ApiProblemException(429, "Retry scheduled", "Automatic delivery retry is already scheduled. Please wait before retrying.");
            email.Status = "PENDING";
            email.NextAttemptAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            var dto = mapper.Map<TenantInvitationDto>(invitation);
            ApplyDeliveryStatus(dto, email);
            return dto;
        }

        /// <inheritdoc/>
        public async Task<TenantInvitationTokenResponseDto> RespondToTenantInvitationAsync(string token, string response)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new TenantInvitationTokenResponseDto
                {
                    Success = false,
                    Message = "Invitation token is required."
                };
            }

            if (!TryReadTenantInvitationToken(token, out var invitationId, out var tokenEmail, out var tokenExpiryUtc))
            {
                return new TenantInvitationTokenResponseDto
                {
                    Success = false,
                    Message = "Invalid invitation token."
                };
            }

            var normalizedResponse = response?.Trim().ToUpperInvariant();
            if (normalizedResponse != "ACCEPT" && normalizedResponse != "DECLINE")
                return new TenantInvitationTokenResponseDto { Success = false, Message = "Response must be ACCEPT or DECLINE." };
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            await LockInvitationListingAsync(invitationId);
            var invitation = await dbContext.TenantInvitations.SingleOrDefaultAsync(i => i.TenantInvitationID == invitationId);
            if (invitation == null)
            {
                return new TenantInvitationTokenResponseDto
                {
                    Success = false,
                    Message = "Invitation not found."
                };
            }

            var expectedHash = HashToken(token);
            if (!string.Equals(invitation.TokenHash, expectedHash, StringComparison.Ordinal))
            {
                return new TenantInvitationTokenResponseDto
                {
                    Success = false,
                    Message = "Invalid invitation token."
                };
            }

            if (!string.Equals(invitation.Email?.Trim(), tokenEmail, StringComparison.OrdinalIgnoreCase))
            {
                return new TenantInvitationTokenResponseDto
                {
                    Success = false,
                    Message = "Invitation token email mismatch."
                };
            }

            var requestedStatus = normalizedResponse == "ACCEPT" ? "ACCEPTED" : "DECLINED";
            if (invitation.Status == requestedStatus)
                return new TenantInvitationTokenResponseDto
                {
                    Success = true, Message = $"Invitation already {requestedStatus.ToLowerInvariant()}.",
                    TenantInvitationID = invitationId, Status = invitation.Status
                };
            if (invitation.Status != "PENDING")
                return new TenantInvitationTokenResponseDto
                {
                    Success = false, Message = "This invitation is no longer pending and cannot be changed.",
                    TenantInvitationID = invitationId, Status = invitation.Status
                };

            var now = DateTime.UtcNow;
            if (now > invitation.ExpiresAt || now > tokenExpiryUtc)
            {
                invitation.Status = "EXPIRED";
                await ReleaseInvitationHoldsAsync(invitationId);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                cache.Remove(Cache.TENANTINVITATIONS.ToString());

                return new TenantInvitationTokenResponseDto
                {
                    Success = false,
                    Message = "Invitation has expired.",
                    TenantInvitation = this.mapper.Map<TenantInvitationDto>(invitation),
                    TenantInvitationID = invitation.TenantInvitationID,
                    Status = invitation.Status
                };
            }

            invitation.Status = normalizedResponse == "ACCEPT" ? "ACCEPTED" : "DECLINED";
            invitation.AcceptedAt = normalizedResponse == "ACCEPT" ? DateTime.UtcNow : null;
            invitation.RevokedAt = normalizedResponse == "DECLINE" ? DateTime.UtcNow : invitation.RevokedAt;

            if (normalizedResponse == "DECLINE")
                await ReleaseInvitationHoldsAsync(invitationId);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            cache.Remove(Cache.TENANTINVITATIONS.ToString());

            return new TenantInvitationTokenResponseDto
            {
                Success = true,
                Message = normalizedResponse == "ACCEPT" ? "Invitation accepted." : "Invitation declined.",
                TenantInvitation = this.mapper.Map<TenantInvitationDto>(invitation),
                TenantInvitationID = invitation.TenantInvitationID,
                Status = invitation.Status
            };
        }

        /// <inheritdoc/>
        public async Task<CreateLeaseFromTenantInvitationResultDto?> CreateLeaseFromTenantInvitationAsync(Guid tenantInvitationID, Guid tenantID, long signedInUserId, string signedInEmail, string? capturedBy)
        {
            if (tenantID == Guid.Empty)
                throw new ApiProblemException(400, "Invalid tenant", "A valid tenantID is required.");

            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            await LockInvitationListingAsync(tenantInvitationID);
            var invitation = await this.dbContext.TenantInvitations
                .FirstOrDefaultAsync(x => x.TenantInvitationID == tenantInvitationID);

            if (invitation == null)
                return null;

            var normalizedStatus = (invitation.Status ?? string.Empty).Trim().ToUpperInvariant();
            if (normalizedStatus is "DECLINED" or "REVOKED")
                throw new ApiProblemException(400, "Invitation not usable", "The invitation is no longer usable.");

            if (DateTime.UtcNow > invitation.ExpiresAt && !invitation.LeaseID.HasValue)
            {
                invitation.Status = "EXPIRED";
                await ReleaseInvitationHoldsAsync(tenantInvitationID);
                await this.dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                cache.Remove(Cache.TENANTINVITATIONS.ToString());
                throw new ApiProblemException(400, "Invitation expired", "The invitation has expired.");
            }

            if (!string.Equals(normalizedStatus, "ACCEPTED", StringComparison.OrdinalIgnoreCase))
                throw new ApiProblemException(400, "Invitation not accepted", "The invitation must be accepted before creating a lease.");

            var signedInTenant = await this.dbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TenantID == tenantID);

            if (signedInTenant == null)
                throw new ApiProblemException(400, "Tenant not found", "The provided tenantID was not found.");

            if (signedInTenant.UserID != signedInUserId)
                throw new ApiProblemException(403, "Forbidden", "The invitation does not belong to the signed-in user.");

            var normalizedSignedInEmail = (signedInEmail ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedSignedInEmail))
            {
                normalizedSignedInEmail = await this.dbContext.Users
                    .AsNoTracking()
                    .Where(u => u.Id == signedInUserId)
                    .Select(u => u.Email ?? string.Empty)
                    .FirstOrDefaultAsync();
            }

            if (!string.Equals(invitation.Email?.Trim(), normalizedSignedInEmail?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new ApiProblemException(403, "Forbidden", "The invitation does not belong to the signed-in user.");

            var onboarding = await this.paymentOnboardingService.GetStatusAsync(tenantID);
            if (!onboarding.IsReady)
                throw new ApiProblemException(400, "Payment onboarding incomplete", "Please add a verified card and an active PAD method before creating the lease.");

            if (invitation.LeaseID.HasValue && invitation.LeaseID.Value != Guid.Empty)
            {
                var existingLease = await this.dbContext.Leases
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.LeaseID == invitation.LeaseID.Value);

                if (existingLease != null)
                {
                    return new CreateLeaseFromTenantInvitationResultDto
                    {
                        LeaseID = existingLease.LeaseID,
                        Status = string.IsNullOrWhiteSpace(existingLease.Status) ? "PENDING_TENANT_SIGNATURE" : existingLease.Status!,
                        AlreadyExists = true
                    };
                }
            }

            if (!invitation.ListingID.HasValue || invitation.ListingID.Value == Guid.Empty)
                throw new ApiProblemException(400, "Invalid invitation", "The invitation is missing a valid listing reference.");

            if (!invitation.MonthlyRentAmount.HasValue || invitation.MonthlyRentAmount.Value < 0
                || !invitation.SecurityDepositAmount.HasValue || invitation.SecurityDepositAmount.Value < 0
                || string.IsNullOrWhiteSpace(invitation.Currency) || invitation.Currency.Length != 3
                || !invitation.StartDate.HasValue || !invitation.EndDate.HasValue
                || !invitation.LeaseTermMonths.HasValue || invitation.LeaseTermMonths.Value <= 0
                || !invitation.ReservationHoldID.HasValue)
                throw new ApiProblemException(409, "Invitation quote unavailable",
                    "This invitation does not have a complete persisted quote. Ask the landlord to revoke it and send a new invitation.");
            var startDate = invitation.StartDate.Value;
            var endDate = invitation.EndDate.Value;
            var leaseTermMonths = invitation.LeaseTermMonths.Value;
            if (startDate.Date != startDate || endDate != startDate.AddMonths(leaseTermMonths))
                throw new ApiProblemException(409, "Invitation quote invalid", "The persisted invitation dates do not match its lease term. Ask the landlord to send a new invitation.");

            var listing = await this.dbContext.Listings
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.ListingID == invitation.ListingID.Value);

            if (listing == null)
                throw new ApiProblemException(400, "Listing not found", "The listing linked to this invitation was not found.");

            var hold = await this.dbContext.ReservationHolds
                .FirstOrDefaultAsync(h => h.ReservationHoldID == invitation.ReservationHoldID.Value
                    && h.ListingID == listing.ListingID);

            if (hold == null)
                throw new ApiProblemException(400, "Invitation data incomplete", "The invitation does not have persisted lease term dates.");

            if (hold.StartDate != startDate || hold.EndDate != endDate || hold.ReleasedAt.HasValue
                || hold.ExpiresAt <= DateTime.UtcNow || hold.Status != "ACTIVE")
                throw new ApiProblemException(409, "Invitation hold invalid", "The reservation hold no longer matches the invitation. Ask the landlord to send a new invitation.");

            var availability = await this.listingService.GetListingAvailability(listing.OrganizationID, listing.ListingID, startDate, leaseTermMonths);
            if (availability == null)
                throw new ApiProblemException(400, "Availability check failed", "Unable to evaluate listing availability.");

            var conflicts = availability.Conflicts
                .Where(c => !(string.Equals(c.SourceType, "RESERVATION_HOLD", StringComparison.OrdinalIgnoreCase)
                    && c.SourceID == hold.ReservationHoldID))
                .ToList();

            if (!availability.IsAvailable && conflicts.Count != 0)
                throw new ApiProblemException(409, "Listing unavailable", "The selected dates are no longer available.");

            var tenancyTypeId = await ResolveTenancyTypeIDAsync(leaseTermMonths);
            var leaseDefaults = await this.dbContext.Leases
                .AsNoTracking()
                .Where(l => l.OrganizationID == listing.OrganizationID && l.ListingID == listing.ListingID)
                .OrderByDescending(l => l.CapturedDate)
                .FirstOrDefaultAsync();

            var now = DateTime.UtcNow;
            var createdBy = string.IsNullOrWhiteSpace(capturedBy) ? normalizedSignedInEmail : capturedBy;
            var lease = new Lease
            {
                LeaseID = Guid.NewGuid(),
                OrganizationID = listing.OrganizationID,
                ListingID = listing.ListingID,
                RentalUnitID = listing.RentalUnitID,
                TenancyTypeID = tenancyTypeId,
                TenantID = tenantID,
                RentalApplicationID = null,
                LeaseCode = $"LEASE-{tenantInvitationID.ToString("N")[..8].ToUpperInvariant()}-{now:yyyyMMddHHmmss}",
                LeaseNumber = $"INV-{tenantInvitationID.ToString("N")[..8].ToUpperInvariant()}-{now:yyyyMMddHHmmss}",
                Status = "PENDING_TENANT_SIGNATURE",
                StartDate = startDate,
                EndDate = endDate,
                LeaseTermMonths = leaseTermMonths,
                BaseRentAmount = invitation.MonthlyRentAmount.Value,
                Currency = invitation.Currency,
                GracePeriodDays = leaseDefaults?.GracePeriodDays ?? (short)5,
                LateFeeFixedAmount = leaseDefaults?.LateFeeFixedAmount ?? 0,
                LateFeePercentage = leaseDefaults?.LateFeePercentage ?? 0,
                AutoRenew = leaseDefaults?.AutoRenew ?? false,
                RenewalNoticeDays = leaseDefaults?.RenewalNoticeDays ?? 90,
                CapturedBy = createdBy,
                CapturedDate = now
            };

                try
                {
                    await this.dbContext.Leases.AddAsync(lease);
                    await dbContext.SecurityDeposits.AddAsync(new SecurityDeposit
                    {
                        SecurityDepositID = Guid.NewGuid(),
                        LeaseID = lease.LeaseID,
                        TenantID = tenantID,
                        OrganizationID = listing.OrganizationID,
                        RequiredAmount = invitation.SecurityDepositAmount.Value,
                        Currency = invitation.Currency,
                        Status = "EXPECTED",
                        DueDate = startDate,
                        CapturedBy = createdBy,
                        CapturedDate = now
                    });

                    invitation.LeaseID = lease.LeaseID;
                    invitation.Status = "ACCEPTED";
                    invitation.AcceptedAt ??= now;

                    hold.Status = "RELEASED";
                    hold.ReleasedAt = now;
                    hold.ConvertedToLeaseAt = now;

                    await this.dbContext.CalendarEvents.AddAsync(new CalendarEvent
                    {
                        CalendarEventID = Guid.NewGuid(),
                        ListingID = listing.ListingID,
                        LeaseID = lease.LeaseID,
                        ReservationHoldID = hold.ReservationHoldID,
                        EventType = "LEASE",
                        Status = "CONFIRMED",
                        StartAt = lease.StartDate,
                        EndAt = lease.EndDate ?? lease.StartDate.AddMonths(leaseTermMonths),
                        IsAllDay = true,
                        Title = $"Lease block ({lease.Status})",
                        OccupantName = invitation.Name,
                        SourceSystem = "Arcora.Api",
                        SourceReferenceID = lease.LeaseID.ToString(),
                        BlocksAvailability = true,
                        CapturedBy = createdBy,
                        CapturedDate = now
                    });

                    await this.dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            cache.Remove(Cache.LEASES.ToString());
            cache.Remove(Cache.TENANTINVITATIONS.ToString());
            cache.Remove(Cache.RESERVATIONHOLDS.ToString());
            cache.Remove(Cache.CALENDAREVENTS.ToString());

            return new CreateLeaseFromTenantInvitationResultDto
            {
                LeaseID = lease.LeaseID,
                Status = lease.Status ?? "PENDING_TENANT_SIGNATURE",
                AlreadyExists = false
            };
        }

        private async Task<int> ResolveTenancyTypeIDAsync(short leaseTermMonths)
        {
            var isShortTerm = leaseTermMonths <= 6;
            var preferred = isShortTerm ? "SHORT" : "LONG";

            var preferredTypeId = await this.dbContext.TenancyTypes
                .AsNoTracking()
                .Where(t => t.IsActive
                    && ((t.Code != null && t.Code.ToUpper().Contains(preferred))
                        || (t.Name != null && t.Name.ToUpper().Contains(preferred))))
                .OrderBy(t => t.TenancyTypeID)
                .Select(t => (int?)t.TenancyTypeID)
                .FirstOrDefaultAsync();

            if (preferredTypeId.HasValue && preferredTypeId.Value > 0)
                return preferredTypeId.Value;

            var fallbackTypeId = await this.dbContext.TenancyTypes
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.TenancyTypeID)
                .Select(t => (int?)t.TenancyTypeID)
                .FirstOrDefaultAsync();

            if (!fallbackTypeId.HasValue || fallbackTypeId.Value <= 0)
                throw new ApiProblemException(400, "Tenancy type missing", "No active tenancy type was found to create the lease.");

            return fallbackTypeId.Value;
        }

        private string GenerateTenantInvitationToken(Guid invitationId, string email, DateTime expiresAtUtc)
        {
            var expiryUnix = new DateTimeOffset(expiresAtUtc).ToUnixTimeSeconds();
            var payload = $"{invitationId:N}.{email.Trim().ToLowerInvariant()}.{expiryUnix}";
            var signature = SignTokenPayload(payload);
            return ToBase64Url($"{payload}.{signature}");
        }

        private bool TryReadTenantInvitationToken(string token, out Guid invitationId, out string email, out DateTime expiryUtc)
        {
            invitationId = Guid.Empty;
            email = string.Empty;
            expiryUtc = DateTime.MinValue;

            var decoded = FromBase64Url(token);
            if (string.IsNullOrWhiteSpace(decoded))
                return false;

            var firstSeparator = decoded.IndexOf('.');
            var lastSeparator = decoded.LastIndexOf('.');
            if (firstSeparator <= 0 || lastSeparator <= firstSeparator)
                return false;

            var secondLastSeparator = decoded.LastIndexOf('.', lastSeparator - 1);
            if (secondLastSeparator <= firstSeparator)
                return false;

            var invitationPart = decoded[..firstSeparator];
            email = decoded.Substring(firstSeparator + 1, secondLastSeparator - firstSeparator - 1);
            var expiryPart = decoded.Substring(secondLastSeparator + 1, lastSeparator - secondLastSeparator - 1);
            var signature = decoded[(lastSeparator + 1)..];

            if (!Guid.TryParseExact(invitationPart, "N", out invitationId))
                return false;

            if (string.IsNullOrWhiteSpace(email))
                return false;

            if (!long.TryParse(expiryPart, out var expiryUnix))
                return false;

            var payload = $"{invitationPart}.{email}.{expiryPart}";
            var expectedSignature = SignTokenPayload(payload);
            if (!string.Equals(signature, expectedSignature, StringComparison.Ordinal))
                return false;

            expiryUtc = DateTimeOffset.FromUnixTimeSeconds(expiryUnix).UtcDateTime;
            return true;
        }

        private string SignTokenPayload(string payload)
        {
            var key = this.configuration["JwtSettings:Key"] ?? "arcora-default-signing-key";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"tenant-invite:{payload}"));
            return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        /// <inheritdoc/>
        public async Task<List<TenantInvitationDto>> GetPendingInvitationsForTenantAsync(Guid tenantId)
        {
            try
            {
                var invitations = await this.dbContext.TenantInvitations
                    .AsNoTracking()
                    .Where(x => x.Email != null
                        && x.Status == "ACCEPTED"
                        && x.LeaseID == null)
                    .OrderByDescending(x => x.AcceptedAt)
                    .ThenByDescending(x => x.CapturedDate)
                    .ToListAsync();

                if (!invitations.Any())
                    return new List<TenantInvitationDto>();

                var tenant = await this.dbContext.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TenantID == tenantId);

                if (tenant == null)
                    return new List<TenantInvitationDto>();

                // Match on the signed-in user's account email, the same check used when creating the lease.
                var userEmail = await this.dbContext.Users
                    .AsNoTracking()
                    .Where(u => u.Id == tenant.UserID)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(userEmail))
                    return new List<TenantInvitationDto>();

                var matchingInvitations = invitations
                    .Where(x => !string.IsNullOrWhiteSpace(x.Email)
                        && string.Equals(userEmail.Trim(), x.Email.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return this.mapper.Map<List<TenantInvitationDto>>(matchingInvitations);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching pending invitations for tenant {TenantId}. Timestamp: {Timestamp}", tenantId, DateTime.UtcNow);
                return new List<TenantInvitationDto>();
            }
        }

        private static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hash);
        }

        private static string ToBase64Url(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        private static string FromBase64Url(string value)
        {
            var padded = value.Replace("-", "+").Replace("_", "/");
            switch (padded.Length % 4)
            {
                case 2:
                    padded += "==";
                    break;
                case 3:
                    padded += "=";
                    break;
            }

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}