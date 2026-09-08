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
using System.Security.Cryptography;
using System.Text;

namespace Arcora.Api.Services.Implementations
{
    public class TenantGuarantorService : ITenantGuarantorService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TenantGuarantorService> logger;
        private readonly ITenantGuarantorRepository tenantguarantorRepository;
        private readonly ITenantRepository tenantRepository;
        private readonly IListingRepository listingRepository;
        private readonly IListingPhotoRepository listingPhotoRepository;
        private readonly IRentalApplicationRepository rentalApplicationRepository;
        private readonly IPreferenceService preferenceService;
        private readonly IEmailSender? emailSender;
        private readonly IConfiguration configuration;
        private readonly IOptions<CacheConfiguration> _options;
        public TenantGuarantorService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TenantGuarantorService> logger, ITenantGuarantorRepository tenantguarantorRepository, ITenantRepository tenantRepository, IListingRepository listingRepository, IListingPhotoRepository listingPhotoRepository, IRentalApplicationRepository rentalApplicationRepository, IPreferenceService preferenceService, IConfiguration configuration, IEmailSender? emailSender = null)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenantguarantorRepository = tenantguarantorRepository;
            this.tenantRepository = tenantRepository;
            this.listingRepository = listingRepository;
            this.listingPhotoRepository = listingPhotoRepository;
            this.rentalApplicationRepository = rentalApplicationRepository;
            this.preferenceService = preferenceService;
            this.configuration = configuration;
            this.emailSender = emailSender;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenantGuarantorDto>> GetAll(Paging paging)
        {
            IEnumerable<TenantGuarantor> entities;
            try
            {
                entities = cache.Get<IEnumerable<TenantGuarantor>>(Cache.TENANTGUARANTORS.ToString()) ?? new List<TenantGuarantor>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.tenantguarantorRepository.GetTenantGuarantorAsync())?.Where(x => x != null) ?? new List<TenantGuarantor>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<TenantGuarantor>>(Cache.TENANTGUARANTORS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantGuarantor by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenantGuarantorDto>
                {
                    Data = new List<TenantGuarantorDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TenantGuarantor> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenantGuarantorID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenantGuarantorDto>>(pagedEntities);
            return new PagedResult<TenantGuarantorDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<TenantGuarantor> entities = cache.Get<IEnumerable<TenantGuarantor>>(Cache.TENANTGUARANTORS.ToString()) ?? new List<TenantGuarantor>();
                TenantGuarantor? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TenantGuarantorID == ID);
                }
                else
                {
                    match = await this.tenantguarantorRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TenantGuarantorDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenantGuarantor by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto> CreateTenantGuarantor(TenantGuarantorDto tenantguarantorDto)
        {
            TenantGuarantor tenantGuarantor = new TenantGuarantor();
            IEnumerable<TenantGuarantor?> checkEntity;
            try
            {
                checkEntity = await this.tenantguarantorRepository.Find(x => x.LastName!.ToLower().Trim() == tenantguarantorDto.LastName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenantGuarantor = this.mapper.Map<TenantGuarantor>(tenantguarantorDto);
                    tenantGuarantor.TenantGuarantorID = Guid.NewGuid();
                    tenantGuarantor.UserID = tenantguarantorDto.UserID == 0 ? null : tenantguarantorDto.UserID;
                    tenantGuarantor.Status = "PENDING";
                    tenantGuarantor.InvitedAt = DateTime.UtcNow;
                    tenantGuarantor.CapturedDate = DateTime.UtcNow;
                    tenantGuarantor = await tenantguarantorRepository.Create(tenantGuarantor) ?? new TenantGuarantor();
                    await tenantguarantorRepository.Save();
                    cache.Remove(Cache.TENANTGUARANTORS.ToString());

                    // The invitation email is intentionally NOT sent here. Guarantors are captured before
                    // the rental application exists, so the listing/rent/deposit details aren't yet linked.
                    // The email is sent from NotifyGuarantorsForApplicationAsync once the application is saved.
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TenantGuarantor. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenantGuarantorDto>(tenantGuarantor);
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto?> UpdateTenantGuarantor(Guid id, TenantGuarantorDto tenantguarantorDto)
        {
            try
            {
                var existing = await this.tenantguarantorRepository.GetByID(id);
                if (existing == null)
                    return null;
                TenantGuarantor tenantGuarantor = this.mapper.Map<TenantGuarantor>(tenantguarantorDto);
                tenantGuarantor = await tenantguarantorRepository.Update(tenantGuarantor) ?? new TenantGuarantor();
                await tenantguarantorRepository.Save();
                cache.Remove(Cache.TENANTGUARANTORS.ToString());
                tenantguarantorDto = this.mapper.Map<TenantGuarantorDto>(tenantGuarantor);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating TenantGuarantor. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return tenantguarantorDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTenantGuarantor(Guid ID)
        {
            try
            {
                var tenantGuarantor = await this.tenantguarantorRepository.GetByID(ID);
                if (tenantGuarantor == null)
                    throw new KeyNotFoundException("TenantGuarantor with the specified ID was not found.");
                await tenantguarantorRepository.Delete(tenantGuarantor);
                await tenantguarantorRepository.Save();
                cache.Remove(Cache.TENANTGUARANTORS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TenantGuarantor . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto?> UpdateTenantGuarantorStatus(Guid id, string status)
        {
            var tenantGuarantor = await tenantguarantorRepository.GetByID(id);
            if (tenantGuarantor == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                tenantGuarantor.Status = "Pending";
            }
            else
            {
                tenantGuarantor.Status = status;
            }

            await tenantguarantorRepository.Update(tenantGuarantor);
            await tenantguarantorRepository.Save();
            cache.Remove(Cache.TENANTGUARANTORS.ToString());
            return this.mapper.Map<TenantGuarantorDto>(tenantGuarantor);
        }

        /// <inheritdoc/>
        public async Task<TenantGuarantorDto?> RespondToInvitationAsync(string response, string token)
        {
            // The token both identifies the guarantor and proves the link is genuine and unexpired.
            if (!TryValidateInviteToken(token, out var id))
            {
                logger.LogWarning("Invalid or expired guarantor invite token.");
                return null;
            }

            var tenantGuarantor = await tenantguarantorRepository.GetByID(id);
            if (tenantGuarantor == null)
                return null;

            var normalized = response?.Trim().ToUpperInvariant();
            switch (normalized)
            {
                case "ACCEPT":
                case "ACCEPTED":
                    tenantGuarantor.Status = "ACCEPTED";
                    tenantGuarantor.AcceptedAt = DateTime.UtcNow;
                    break;
                case "DECLINE":
                case "DECLINED":
                    tenantGuarantor.Status = "DECLINED";
                    break;
                default:
                    logger.LogWarning("Unsupported guarantor response '{Response}' for guarantor {GuarantorId}.", response, id);
                    return null;
            }

            tenantGuarantor.UpdatedDate = DateTime.UtcNow;
            await tenantguarantorRepository.Update(tenantGuarantor);
            await tenantguarantorRepository.Save();
            cache.Remove(Cache.TENANTGUARANTORS.ToString());
            return this.mapper.Map<TenantGuarantorDto>(tenantGuarantor);
        }

        /// <inheritdoc/>
        public async Task NotifyGuarantorsForApplicationAsync(Guid rentalApplicationId, Guid tenantId, Guid listingId)
        {
            try
            {
                // Pick up guarantors captured before the application existed as well as any already
                // linked to this application that are still awaiting an invite.
                var guarantors = await tenantguarantorRepository.Find(x =>
                    x.TenantID == tenantId &&
                    (x.RentalApplicationID == null || x.RentalApplicationID == rentalApplicationId));

                var list = guarantors?.Where(g => g != null).ToList() ?? new List<TenantGuarantor?>();
                if (!list.Any())
                {
                    return;
                }

                var listing = await listingRepository.GetByID(listingId);

                foreach (var guarantor in list)
                {
                    // Backfill the application id so the guarantor is tied to this application.
                    if (guarantor!.RentalApplicationID != rentalApplicationId)
                    {
                        guarantor.RentalApplicationID = rentalApplicationId;
                        guarantor.UpdatedDate = DateTime.UtcNow;
                        await tenantguarantorRepository.Update(guarantor);
                        await tenantguarantorRepository.Save();
                    }

                    await SendGuarantorInvitationEmailAsync(guarantor, listing);
                }

                cache.Remove(Cache.TENANTGUARANTORS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to notify guarantors for application {AppId}. Timestamp: {Timestamp}", rentalApplicationId, DateTime.UtcNow);
            }
        }

        /// <inheritdoc/>
        public async Task<GuarantorInviteDetailsDto?> GetInviteDetailsAsync(string token)
        {
            if (!TryValidateInviteToken(token, out var id))
            {
                logger.LogWarning("Invalid or expired guarantor invite token on details request.");
                return null;
            }

            var guarantor = await tenantguarantorRepository.GetByID(id);
            if (guarantor == null)
                return null;

            return await BuildInviteDetailsAsync(guarantor);
        }

        /// <summary>
        /// Gathers the tenant, listing and guarantor display data used both by the invitation email and
        /// the public invite landing page.
        /// </summary>
        private async Task<GuarantorInviteDetailsDto> BuildInviteDetailsAsync(TenantGuarantor guarantor, Listing? listing = null)
        {
            var tenant = await tenantRepository.GetByID(guarantor.TenantID);
            // Reload with the User navigation so we can build the tenant's full name.
            if (tenant != null)
            {
                tenant = await tenantRepository.GetTenantByUserIDAsync(tenant.UserID) ?? tenant;
            }
            var tenantName = tenant?.User != null
                ? $"{tenant.User.FirstName} {tenant.User.LastName}".Trim()
                : "A tenant";

            // Fall back to resolving the listing via the linked rental application when not supplied.
            if (listing == null && guarantor.RentalApplicationID.HasValue)
            {
                var application = await rentalApplicationRepository.GetByID(guarantor.RentalApplicationID.Value);
                if (application != null)
                {
                    listing = await listingRepository.GetByID(application.ListingID);
                }
            }

            var currency = listing?.Currency ?? "CAD";
            var monthlyRent = listing != null
                ? FormatMoney(listing.BaseMonthlyRentAmount, currency)
                : "See application";
            var securityDeposit = listing != null
                ? FormatMoney(listing.SecurityDepositAmount, currency)
                : "See application";
            var listingTitle = listing?.Title ?? "The listing";

            string? imageUrl = null;
            if (listing != null)
            {
                var photos = await listingPhotoRepository.Find(p => p.ListingID == listing.ListingID);
                var photoList = photos?.Where(p => p != null).ToList() ?? new List<ListingPhoto?>();
                imageUrl = photoList.FirstOrDefault(p => p!.IsCoverPhoto)?.Url
                    ?? photoList.OrderBy(p => p!.DisplayOrder).FirstOrDefault()?.Url;
            }

            return new GuarantorInviteDetailsDto
            {
                GuarantorName = $"{guarantor.FirstName} {guarantor.LastName}".Trim(),
                TenantName = tenantName,
                TenantPhone = tenant?.PhoneNumber,
                ListingTitle = listingTitle,
                MonthlyRent = monthlyRent,
                SecurityDeposit = securityDeposit,
                ListingImageUrl = imageUrl,
                Status = guarantor.Status
            };
        }

        /// <summary>
        /// Builds and sends the guarantor invitation email containing the tenant's details, the listing
        /// details (rent, deposit and a cover image) and signed accept/decline links. Failures are logged
        /// but never block the caller.
        /// </summary>
        private async Task SendGuarantorInvitationEmailAsync(TenantGuarantor guarantor, Listing? listing = null)
        {
            try
            {
                if (emailSender is null || string.IsNullOrWhiteSpace(guarantor.Email))
                {
                    return;
                }

                var details = await BuildInviteDetailsAsync(guarantor, listing);

                var (companyName, companyEmail) = await GetCompanyInfoAsync();
                var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;

                var token = GenerateInviteToken(guarantor.TenantGuarantorID);
                var frontendUrl = (configuration["FrontendUrl"] ?? string.Empty).TrimEnd('/');
                // The frontend landing page (/guarantor-invite/:token) reads the token, shows the details
                // and lets the guarantor choose accept or decline, then calls the API with that choice.
                var inviteUrl = $"{frontendUrl}/guarantor-invite/{token}";
                var acceptUrl = $"{inviteUrl}?response=ACCEPT";
                var declineUrl = $"{inviteUrl}?response=DECLINE";

                var htmlBody = EmailTemplates.BuildGuarantorInvitationEmail(
                    guarantorName: details.GuarantorName ?? string.Empty,
                    tenantName: details.TenantName ?? "A tenant",
                    tenantPhone: details.TenantPhone,
                    listingTitle: details.ListingTitle ?? "The listing",
                    monthlyRent: details.MonthlyRent ?? "See application",
                    securityDeposit: details.SecurityDeposit ?? "See application",
                    listingImageUrl: details.ListingImageUrl,
                    acceptUrl: acceptUrl,
                    declineUrl: declineUrl,
                    companyName: brand,
                    supportEmail: companyEmail);

                await emailSender.SendEmailAsync(guarantor.Email, $"{details.TenantName} has named you as a guarantor on {brand}", htmlBody);
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to send guarantor invitation email for guarantor {GuarantorId}. Timestamp: {Timestamp}", guarantor.TenantGuarantorID, DateTime.UtcNow);
            }
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

        private static string FormatMoney(decimal amount, string currency)
        {
            return $"{amount.ToString("N2", CultureInfo.InvariantCulture)} {currency}";
        }

        /// <summary>
        /// Number of days a guarantor invitation link remains valid before it expires.
        /// </summary>
        private const int InviteTokenValidDays = 7;

        /// <summary>
        /// Generates a URL-safe, self-contained token of the form <c>{guarantorId}.{expiryUnix}.{signature}</c>.
        /// The token both identifies the guarantor and carries its own expiry, so no id needs to travel
        /// separately in the URL. The signature is an HMAC over the id and expiry, preventing tampering.
        /// </summary>
        private string GenerateInviteToken(Guid guarantorId)
        {
            var expiryUnix = DateTimeOffset.UtcNow.AddDays(InviteTokenValidDays).ToUnixTimeSeconds();
            var payload = $"{guarantorId:N}.{expiryUnix}";
            var signature = SignPayload(payload);
            return ToBase64Url($"{payload}.{signature}");
        }

        /// <summary>
        /// Validates a guarantor invite token: verifies the signature and that it has not expired, and
        /// extracts the guarantor id when valid.
        /// </summary>
        private bool TryValidateInviteToken(string token, out Guid guarantorId)
        {
            guarantorId = Guid.Empty;
            if (string.IsNullOrWhiteSpace(token))
                return false;

            string decoded;
            try
            {
                decoded = FromBase64Url(token);
            }
            catch
            {
                return false;
            }

            var parts = decoded.Split('.');
            if (parts.Length != 3)
                return false;

            var payload = $"{parts[0]}.{parts[1]}";
            var expectedSignature = SignPayload(payload);

            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expectedSignature),
                    Encoding.UTF8.GetBytes(parts[2])))
            {
                return false;
            }

            if (!long.TryParse(parts[1], out var expiryUnix))
                return false;

            if (DateTimeOffset.FromUnixTimeSeconds(expiryUnix) < DateTimeOffset.UtcNow)
                return false; // expired

            if (!Guid.TryParseExact(parts[0], "N", out guarantorId))
                return false;

            return true;
        }

        private string SignPayload(string payload)
        {
            var key = configuration["JwtSettings:Key"] ?? "arcora-default-signing-key";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"guarantor-invite:{payload}"));
            return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").TrimEnd('=');
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
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
        }
    }
}