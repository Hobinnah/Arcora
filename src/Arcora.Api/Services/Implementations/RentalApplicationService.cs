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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Arcora.Api.Services.Implementations
{
    public class RentalApplicationService : IRentalApplicationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<RentalApplicationService> logger;
        private readonly IRentalApplicationRepository rentalapplicationRepository;
        private readonly IRentCollectionOrchestrator rentCollectionOrchestrator;
        private readonly IListingRepository listingRepository;
        private readonly IFeeRepository feeRepository;
        private readonly ISecurityDepositRepository securityDepositRepository;
        private readonly ILeaseRepository leaseRepository;
        private readonly ITenancyTypeRepository tenancyTypeRepository;
        private readonly IApplicationOccupantRepository applicationOccupantRepository;
        private readonly ILeaseDocumentsRepository leaseDocumentsRepository;
        private readonly ITenantGuarantorService tenantGuarantorService;
        private readonly ITenantRepository tenantRepository;
        private readonly IListingPhotoRepository listingPhotoRepository;
        private readonly IOrganizationMemberRepository organizationMemberRepository;
        private readonly IPreferenceService preferenceService;
        private readonly IConfiguration configuration;
        private readonly IEmailSender? emailSender;
        private readonly IOptions<CacheConfiguration> _options;
        public RentalApplicationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<RentalApplicationService> logger, IRentalApplicationRepository rentalapplicationRepository, IRentCollectionOrchestrator rentCollectionOrchestrator, IListingRepository listingRepository, IFeeRepository feeRepository, ISecurityDepositRepository securityDepositRepository, ILeaseRepository leaseRepository, ITenancyTypeRepository tenancyTypeRepository, IApplicationOccupantRepository applicationOccupantRepository, ILeaseDocumentsRepository leaseDocumentsRepository, ITenantGuarantorService tenantGuarantorService, ITenantRepository tenantRepository, IListingPhotoRepository listingPhotoRepository, IOrganizationMemberRepository organizationMemberRepository, IPreferenceService preferenceService, IConfiguration configuration, IEmailSender? emailSender = null)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.rentalapplicationRepository = rentalapplicationRepository;
            this.rentCollectionOrchestrator = rentCollectionOrchestrator;
            this.listingRepository = listingRepository;
            this.feeRepository = feeRepository;
            this.securityDepositRepository = securityDepositRepository;
            this.leaseRepository = leaseRepository;
            this.tenancyTypeRepository = tenancyTypeRepository;
            this.applicationOccupantRepository = applicationOccupantRepository;
            this.leaseDocumentsRepository = leaseDocumentsRepository;
            this.tenantGuarantorService = tenantGuarantorService;
            this.tenantRepository = tenantRepository;
            this.listingPhotoRepository = listingPhotoRepository;
            this.organizationMemberRepository = organizationMemberRepository;
            this.preferenceService = preferenceService;
            this.configuration = configuration;
            this.emailSender = emailSender;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<RentalApplicationDto>> GetAll(Paging paging)
        {
            IEnumerable<RentalApplication> entities;
            try
            {
                entities = cache.Get<IEnumerable<RentalApplication>>(Cache.RENTALAPPLICATIONS.ToString()) ?? new List<RentalApplication>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.rentalapplicationRepository.GetRentalApplicationAsync())?.Where(x => x != null) ?? new List<RentalApplication>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<RentalApplication>>(Cache.RENTALAPPLICATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching RentalApplication by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<RentalApplicationDto>
                {
                    Data = new List<RentalApplicationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<RentalApplication> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ApplicationCode) && x.ApplicationCode.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.RentalApplicationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<RentalApplicationDto>>(pagedEntities);
            return new PagedResult<RentalApplicationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<RentalApplicationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<RentalApplication> entities = cache.Get<IEnumerable<RentalApplication>>(Cache.RENTALAPPLICATIONS.ToString()) ?? new List<RentalApplication>();
                RentalApplication? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.RentalApplicationID == ID);
                }
                else
                {
                    match = await this.rentalapplicationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<RentalApplicationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching RentalApplication by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RentalApplicationDto> CreateRentalApplication(RentalApplicationDto rentalapplicationDto)
        {
            RentalApplication rentalApplication = new RentalApplication();
            IEnumerable<RentalApplication?> checkEntity;
            try
            {
                checkEntity = await this.rentalapplicationRepository.Find(x => x.ApplicationCode!.ToLower().Trim() == rentalapplicationDto.ApplicationCode!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    rentalApplication = this.mapper.Map<RentalApplication>(rentalapplicationDto);
                    rentalApplication.RentalApplicationID = Guid.NewGuid();
                    rentalApplication.ApplicationCode = await GenerateUniqueApplicationCodeAsync();
                    rentalApplication.ReviewedByOrganizationMemberID = rentalapplicationDto.ReviewedByOrganizationMemberID == Guid.Empty ? null : rentalapplicationDto.ReviewedByOrganizationMemberID;
                    rentalApplication.CapturedDate = DateTime.UtcNow;
                    rentalApplication.UpdatedBy = string.Empty;
                    rentalApplication.UpdatedDate = null;
                    rentalApplication = await rentalapplicationRepository.Create(rentalApplication) ?? new RentalApplication();
                    await rentalapplicationRepository.Save();
                    cache.Remove(Cache.RENTALAPPLICATIONS.ToString());

                    // Backfill the newly generated application id onto the related records that were
                    // captured before the application existed (occupants and uploaded documents).
                    await LinkRelatedRecordsAsync(rentalApplication);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating RentalApplication. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<RentalApplicationDto>(rentalApplication);
        }

        /// <summary>
        /// Generates a 10-character uppercase alphanumeric application code (e.g. "HMEQH2ZTA4"),
        /// retrying until a code not already present in the repository is produced.
        /// </summary>
        private async Task<string> GenerateUniqueApplicationCodeAsync()
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int length = 10;

            for (var attempt = 0; attempt < 10; attempt++)
            {
                var chars = new char[length];
                var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(length);
                for (var i = 0; i < length; i++)
                {
                    chars[i] = alphabet[bytes[i] % alphabet.Length];
                }

                var code = new string(chars);
                var existing = await rentalapplicationRepository.Find(x =>
                    x.ApplicationCode != null && x.ApplicationCode == code);

                if (existing == null || !existing.Any())
                {
                    return code;
                }
            }

            // Extremely unlikely fallback: append a short unique suffix.
            return $"{Guid.NewGuid():N}".Substring(0, length).ToUpperInvariant();
        }

        /// <summary>
        /// Associates records that were captured before the rental application existed with the newly
        /// created application. Occupants and lease documents belonging to the same tenant (and listing,
        /// for documents) that do not yet reference an application are updated with the new
        /// <see cref="RentalApplication.RentalApplicationID"/>.
        /// </summary>
        private async Task LinkRelatedRecordsAsync(RentalApplication application)
        {
            try
            {
                var occupants = await applicationOccupantRepository.Find(x =>
                    x.RentalApplicationID == null && x.TenantID == application.TenantID);

                var occupantList = occupants?.Where(o => o != null).ToList() ?? new List<ApplicationOccupant?>();

                foreach (var occupant in occupantList)
                {
                    occupant!.RentalApplicationID = application.RentalApplicationID;
                    await applicationOccupantRepository.Update(occupant);
                }

                if (occupantList.Any())
                {
                    await applicationOccupantRepository.Save();
                    cache.Remove(Cache.APPLICATIONOCCUPANTS.ToString());
                }

                // Derive the occupant counts on the application from the linked occupants.
                var allOccupants = (await applicationOccupantRepository.Find(x =>
                    x.RentalApplicationID == application.RentalApplicationID))?
                    .Where(o => o != null).ToList() ?? new List<ApplicationOccupant?>();

                application.AdultOccupantCount = allOccupants.Count(o =>
                    string.Equals(o!.OccupantType?.Trim(), "ADULT", StringComparison.OrdinalIgnoreCase));
                application.ChildOccupantCount = allOccupants.Count(o =>
                    string.Equals(o!.OccupantType?.Trim(), "CHILD", StringComparison.OrdinalIgnoreCase));
                application.PetCount = allOccupants.Count(o =>
                    string.Equals(o!.OccupantType?.Trim(), "PET", StringComparison.OrdinalIgnoreCase));

                application.UpdatedDate = DateTime.UtcNow;
                await rentalapplicationRepository.Update(application);
                await rentalapplicationRepository.Save();
                cache.Remove(Cache.RENTALAPPLICATIONS.ToString());

                var documents = await leaseDocumentsRepository.Find(x =>
                    x.RentalApplicationID == null &&
                    x.TenantID == application.TenantID &&
                    x.ListingID == application.ListingID);

                foreach (var document in documents.Where(d => d != null)!)
                {
                    document!.RentalApplicationID = application.RentalApplicationID;
                    await leaseDocumentsRepository.Update(document);
                }

                if (documents != null && documents.Any(d => d != null))
                {
                    await leaseDocumentsRepository.Save();
                    cache.Remove(Cache.LEASEDOCUMENTS.ToString());
                }

                // Now that the application exists (and guarantors can be linked to it), notify the
                // tenant's guarantors by email that they've been named as a guarantor for this listing.
                await tenantGuarantorService.NotifyGuarantorsForApplicationAsync(
                    application.RentalApplicationID, application.TenantID, application.ListingID);

                // Send Airbnb-style confirmation emails to both the tenant and the host/landlord.
                await SendApplicationSubmittedEmailsAsync(application);
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to link related records for application {AppId}. Timestamp: {Timestamp}", application.RentalApplicationID, DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Sends confirmation emails after a rental application is saved: one to the tenant confirming
        /// their submission, and one to the host/landlord (organization primary owner) notifying them of
        /// the new application. Failures are logged but never block application creation.
        /// </summary>
        private async Task SendApplicationSubmittedEmailsAsync(RentalApplication application)
        {
            try
            {
                if (emailSender is null)
                {
                    return;
                }

                var listing = application.Listing ?? await listingRepository.GetByID(application.ListingID);
                var listingTitle = listing?.Title ?? "the listing";
                var currency = application.Currency ?? listing?.Currency ?? "CAD";
                var rent = application.ProposedMonthlyRentAmount ?? listing?.BaseMonthlyRentAmount ?? 0m;
                var monthlyRent = $"{rent.ToString("N2", CultureInfo.InvariantCulture)} {currency}";
                var moveInDate = application.DesiredMoveInDate.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
                var leaseTerm = $"{application.RequestedLeaseTermMonths} months";
                var occupants = $"{application.AdultOccupantCount} adult(s), {application.ChildOccupantCount} child(ren), {application.PetCount} pet(s)";
                var appCode = application.ApplicationCode ?? string.Empty;

                // Cover image for the listing.
                string? imageUrl = null;
                if (listing != null)
                {
                    var photos = await listingPhotoRepository.Find(p => p.ListingID == listing.ListingID);
                    var photoList = photos?.Where(p => p != null).ToList() ?? new List<ListingPhoto?>();
                    imageUrl = photoList.FirstOrDefault(p => p!.IsCoverPhoto)?.Url
                        ?? photoList.OrderBy(p => p!.DisplayOrder).FirstOrDefault()?.Url;
                }

                var (companyName, companyEmail) = await GetCompanyInfoAsync();
                var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;
                var frontendUrl = (configuration["FrontendUrl"] ?? string.Empty).TrimEnd('/');

                // Resolve the tenant (with User for name/email).
                var tenant = await tenantRepository.GetByID(application.TenantID);
                if (tenant != null)
                {
                    tenant = await tenantRepository.GetTenantByUserIDAsync(tenant.UserID) ?? tenant;
                }
                var tenantName = tenant?.User != null
                    ? $"{tenant.User.FirstName} {tenant.User.LastName}".Trim()
                    : "there";
                var tenantEmail = tenant?.User?.Email;

                // ----- Tenant confirmation -----
                if (!string.IsNullOrWhiteSpace(tenantEmail))
                {
                    var tenantHtml = EmailTemplates.BuildRentalApplicationSubmittedEmail(
                        subject: $"Your application for {listingTitle}",
                        eyebrow: "Application received",
                        headline: $"Thanks {tenantName}, your application is in!",
                        intro: $"We've submitted your rental application for {listingTitle} to the host. You'll be notified as soon as they review it. Here's a summary of what you sent.",
                        listingTitle: listingTitle,
                        applicationCode: appCode,
                        monthlyRent: monthlyRent,
                        moveInDate: moveInDate,
                        leaseTerm: leaseTerm,
                        occupants: occupants,
                        listingImageUrl: imageUrl,
                        applicantName: null,
                        applicantPhone: null,
                        actionUrl: string.IsNullOrWhiteSpace(frontendUrl) ? "#" : $"{frontendUrl}/applications/{application.RentalApplicationID}",
                        actionLabel: "View your application",
                        footnote: "If you didn't submit this application, please contact us right away.",
                        companyName: brand,
                        supportEmail: companyEmail);

                    await emailSender.SendEmailAsync(tenantEmail, $"Your application for {listingTitle} — {brand}", tenantHtml);
                }

                // ----- Host / landlord notification -----
                var hostEmail = await GetHostEmailAsync(application.OrganizationID);
                if (!string.IsNullOrWhiteSpace(hostEmail))
                {
                    var applicantPhone = tenant?.PhoneNumber;
                    var applicantName = tenant?.User != null
                        ? $"{tenant.User.FirstName} {tenant.User.LastName}".Trim()
                        : "A prospective tenant";

                    var hostHtml = EmailTemplates.BuildRentalApplicationSubmittedEmail(
                        subject: $"New application for {listingTitle}",
                        eyebrow: "New application",
                        headline: "You have a new rental application",
                        intro: $"{applicantName} has applied for {listingTitle}. Review the details below and respond when you're ready.",
                        listingTitle: listingTitle,
                        applicationCode: appCode,
                        monthlyRent: monthlyRent,
                        moveInDate: moveInDate,
                        leaseTerm: leaseTerm,
                        occupants: occupants,
                        listingImageUrl: imageUrl,
                        applicantName: applicantName,
                        applicantPhone: applicantPhone,
                        actionUrl: string.IsNullOrWhiteSpace(frontendUrl) ? "#" : $"{frontendUrl}/applications/{application.RentalApplicationID}/review",
                        actionLabel: "Review application",
                        footnote: "Respond promptly to give applicants the best experience.",
                        companyName: brand,
                        supportEmail: companyEmail);

                    await emailSender.SendEmailAsync(hostEmail, $"New application for {listingTitle} — {brand}", hostHtml);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to send application submitted emails for application {AppId}. Timestamp: {Timestamp}", application.RentalApplicationID, DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Resolves the host/landlord email for an organization: prefers the primary owner, otherwise any
        /// active member with an email.
        /// </summary>
        private async Task<string?> GetHostEmailAsync(Guid organizationId)
        {
            var members = await organizationMemberRepository.GetOrganizationMembersAsync();
            var orgMembers = members
                .Where(m => m.OrganizationID == organizationId && m.User != null && !string.IsNullOrWhiteSpace(m.User.Email))
                .ToList();

            var owner = orgMembers.FirstOrDefault(m => m.IsPrimaryOwner)
                ?? orgMembers.FirstOrDefault(m => string.Equals(m.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                ?? orgMembers.FirstOrDefault();

            return owner?.User?.Email;
        }

        private async Task<(string CompanyName, string CompanyEmail)> GetCompanyInfoAsync()
        {
            try
            {
                var preference = await preferenceService.GetPreference();
                if (preference != null)
                    return (preference.CompanyName ?? "", preference.CompanyEmail ?? "");
            }
            catch
            { /* non-critical */
            }

            return ("", "");
        }

        /// <inheritdoc/>
        public async Task<RentalApplicationDto?> UpdateRentalApplication(Guid id, RentalApplicationDto rentalapplicationDto)
        {
            try
            {
                var existing = await this.rentalapplicationRepository.GetByID(id);
                if (existing == null)
                    return null;
                RentalApplication rentalApplication = this.mapper.Map<RentalApplication>(rentalapplicationDto);
                rentalApplication = await rentalapplicationRepository.Update(rentalApplication) ?? new RentalApplication();
                await rentalapplicationRepository.Save();
                cache.Remove(Cache.RENTALAPPLICATIONS.ToString());
                rentalapplicationDto = this.mapper.Map<RentalApplicationDto>(rentalApplication);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating RentalApplication. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return rentalapplicationDto;
        }

        /// <inheritdoc/>
        public async Task DeleteRentalApplication(Guid ID)
        {
            try
            {
                var rentalApplication = await this.rentalapplicationRepository.GetByID(ID);
                if (rentalApplication == null)
                    throw new KeyNotFoundException("RentalApplication with the specified ID was not found.");
                await rentalapplicationRepository.Delete(rentalApplication);
                await rentalapplicationRepository.Save();
                cache.Remove(Cache.RENTALAPPLICATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting RentalApplication . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RentalApplicationDto?> UpdateRentalApplicationStatus(Guid id, string status)
        {
            var rentalApplication = await rentalapplicationRepository.GetByID(id);
            if (rentalApplication == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                rentalApplication.Status = "Pending";
            }
            else
            {
                rentalApplication.Status = status;
            }

            await rentalapplicationRepository.Update(rentalApplication);
            await rentalapplicationRepository.Save();
            cache.Remove(Cache.RENTALAPPLICATIONS.ToString());

            // First rent charge is only taken once the host approves the application. Subsequent
            // monthly charges are handled by the recurring scheduler on the lease anniversary.
            if (rentalApplication.Status != null &&
                rentalApplication.Status.Equals("APPROVED", StringComparison.OrdinalIgnoreCase))
            {
                await TryInitiateFirstChargeAsync(rentalApplication);
            }

            return this.mapper.Map<RentalApplicationDto>(rentalApplication);
        }

        /// <summary>
        /// Attempts the first rent collection at approval time. Failures are logged but never block the
        /// approval itself - collection issues surface through the payment intent status and webhooks.
        /// </summary>
        /// <remarks>
        /// At approval a <see cref="Lease"/> record is created first, before any payment is charged.
        /// The first payment taken at approval is typically larger than a normal monthly rent charge
        /// because it bundles:
        ///   1. the first month's rent (the proposed monthly rent),
        ///   2. the security deposit for the listing, and
        ///   3. any active platform/booking fees (charged only when the booking is made through this platform).
        /// Percentage-based platform fees are calculated on the rent only, never on rent + security deposit.
        /// Subsequent monthly charges are handled by the recurring scheduler on the lease anniversary and
        /// only include the recurring rent.
        /// </remarks>
        private async Task TryInitiateFirstChargeAsync(RentalApplication application)
        {
            try
            {
                // A lease record is created before payment is charged.
                var lease = await CreateLeaseForApprovalAsync(application);

                var firstPayment = await CalculateFirstPaymentAmountAsync(application, lease);
                if (firstPayment <= 0m)
                {
                    logger.LogWarning("Skipping first charge for application {AppId}: no proposed rent amount.", application.RentalApplicationID);
                    return;
                }

                await rentCollectionOrchestrator.InitiateCollectionAsync(
                    leaseId: lease?.LeaseID,
                    tenantId: application.TenantID,
                    invoiceMasterId: Guid.Empty,
                    amount: firstPayment,
                    idempotencyKey: $"pi_firstcharge_{application.RentalApplicationID:N}");
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to initiate first rent charge for application {AppId}. Timestamp: {Timestamp}", application.RentalApplicationID, DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Creates and persists a <see cref="Lease"/> record for the approved application before any
        /// payment is charged. Idempotent: if a lease already exists for the application it is returned
        /// as-is rather than duplicated.
        /// </summary>
        private async Task<Lease?> CreateLeaseForApprovalAsync(RentalApplication application)
        {
            try
            {
                var existing = await leaseRepository.Find(x => x.RentalApplicationID == application.RentalApplicationID);
                if (existing != null && existing.Any())
                {
                    return existing.First();
                }

                var listing = application.Listing ?? await listingRepository.GetByID(application.ListingID);
                if (listing == null)
                {
                    logger.LogWarning("Cannot create lease for application {AppId}: listing {ListingId} not found.", application.RentalApplicationID, application.ListingID);
                    return null;
                }

                var rent = application.ProposedMonthlyRentAmount ?? 0m;
                var startDate = application.DesiredMoveInDate;
                var termMonths = application.RequestedLeaseTermMonths;
                var tenancyType = (await tenancyTypeRepository.GetAll())?.FirstOrDefault();

                var lease = new Lease
                {
                    LeaseID = Guid.NewGuid(),
                    OrganizationID = application.OrganizationID,
                    ListingID = listing.ListingID,
                    RentalUnitID = listing.RentalUnitID,
                    TenancyTypeID = tenancyType?.TenancyTypeID ?? 0,
                    TenantID = application.TenantID,
                    RentalApplicationID = application.RentalApplicationID,
                    LeaseCode = $"L-{application.ApplicationCode}",
                    LeaseNumber = $"LN-{application.RentalApplicationID:N}".Substring(0, Math.Min(100, 3 + 32)),
                    Status = "DRAFT",
                    StartDate = startDate,
                    EndDate = termMonths > 0 ? startDate.AddMonths(termMonths) : application.DesiredMoveOutDate,
                    LeaseTermMonths = termMonths,
                    BaseRentAmount = rent,
                    Currency = application.Currency ?? "CAD",
                    GracePeriodDays = 0,
                    LateFeeFixedAmount = 0m,
                    LateFeePercentage = 0m,
                    AutoRenew = false,
                    CapturedDate = DateTime.UtcNow,
                    CapturedBy = "SYSTEM"
                };

                lease = await leaseRepository.Create(lease) ?? lease;
                await leaseRepository.Save();
                cache.Remove(Cache.LEASES.ToString());
                return lease;
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to create lease for application {AppId}. Timestamp: {Timestamp}", application.RentalApplicationID, DateTime.UtcNow);
                return null;
            }
        }

        /// <summary>
        /// Computes the total amount for the first payment at approval time:
        /// first month's rent + security deposit + any active platform/booking fees.
        /// </summary>
        private async Task<decimal> CalculateFirstPaymentAmountAsync(RentalApplication application, Lease? lease)
        {
            var rent = application.ProposedMonthlyRentAmount ?? 0m;
            if (rent <= 0m)
            {
                return 0m;
            }

            // The security deposit is defined on the listing and is collected as part of the first payment.
            var listing = application.Listing ?? await listingRepository.GetByID(application.ListingID);
            var securityDeposit = listing?.SecurityDepositAmount ?? 0m;

            // Record the security deposit against the dedicated SecurityDeposit ledger entity so it is
            // tracked separately from rent, even though it is collected within the same first payment.
            if (securityDeposit > 0m)
            {
                await RecordSecurityDepositAsync(application, lease, securityDeposit);
            }

            // Platform/booking fees only apply when the booking was made through this platform.
            // Percentage-based fees are calculated on the rent only (never on rent + security deposit).
            var platformFees = await CalculatePlatformFeesAsync(application, rent);

            var total = rent + securityDeposit + platformFees;

            logger.LogInformation(
                "First payment for application {AppId}: rent {Rent} + deposit {Deposit} + platform fees {Fees} = {Total} {Currency}",
                application.RentalApplicationID, rent, securityDeposit, platformFees, total, application.Currency);

            return total;
        }

        /// <summary>
        /// Records the expected security deposit for the approved application in the SecurityDeposit
        /// entity, associated with the lease created at approval time. Idempotent: skips if a deposit for
        /// the same lease is already expected.
        /// </summary>
        private async Task RecordSecurityDepositAsync(RentalApplication application, Lease? lease, decimal amount)
        {
            try
            {
                var existing = await securityDepositRepository.Find(x =>
                    x.TenantID == application.TenantID &&
                    x.OrganizationID == application.OrganizationID &&
                    ((lease != null && x.LeaseID == lease.LeaseID) || x.LeaseID == null) &&
                    x.Status == "EXPECTED");

                if (existing != null && existing.Any())
                {
                    return;
                }

                var deposit = new SecurityDeposit
                {
                    SecurityDepositID = Guid.NewGuid(),
                    LeaseID = lease?.LeaseID,
                    TenantID = application.TenantID,
                    OrganizationID = application.OrganizationID,
                    RequiredAmount = amount,
                    ReceivedAmount = 0m,
                    AppliedAmount = 0m,
                    ReturnedAmount = 0m,
                    Currency = application.Currency ?? "CAD",
                    Status = "EXPECTED",
                    DueDate = application.DesiredMoveInDate,
                    CapturedDate = DateTime.UtcNow,
                    CapturedBy = "SYSTEM"
                };

                await securityDepositRepository.Create(deposit);
                await securityDepositRepository.Save();
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to record security deposit for application {AppId}. Timestamp: {Timestamp}", application.RentalApplicationID, DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Sums all active platform/booking fees applicable to the application. Organization-specific
        /// fees take precedence but global platform fees (no organization) also apply. Percentage fees
        /// are calculated against the first month's rent and clamped to any configured min/max.
        /// </summary>
        private async Task<decimal> CalculatePlatformFeesAsync(RentalApplication application, decimal rent)
        {
            var now = DateTime.UtcNow;
            var fees = await feeRepository.Find(f =>
                f.IsActive &&
                f.FeeType != null &&
                f.FeeType.IsPlatformFee == true &&
                (f.OrganizationID == null || f.OrganizationID == application.OrganizationID) &&
                (f.EffectiveFrom == null || f.EffectiveFrom <= now) &&
                (f.EffectiveTo == null || f.EffectiveTo >= now));

            var total = 0m;
            foreach (var fee in fees.Where(f => f != null)!)
            {
                total += CalculateFeeAmount(fee!, rent);
            }

            return total;
        }

        /// <summary>
        /// Resolves a single fee to a concrete amount based on its calculation type, applying the
        /// configured minimum/maximum bounds.
        /// </summary>
        private static decimal CalculateFeeAmount(Fee fee, decimal baseAmount)
        {
            var amount = fee.CalculationType?.Trim().ToUpperInvariant() switch
            {
                "PERCENTAGE" => baseAmount * ((fee.PercentageRate ?? 0m) / 100m),
                _ => fee.FixedAmount ?? 0m
            };

            if (fee.MinimumFeeAmount.HasValue && amount < fee.MinimumFeeAmount.Value)
            {
                amount = fee.MinimumFeeAmount.Value;
            }

            if (fee.MaximumFeeAmount.HasValue && amount > fee.MaximumFeeAmount.Value)
            {
                amount = fee.MaximumFeeAmount.Value;
            }

            return amount < 0m ? 0m : amount;
        }
    }
}