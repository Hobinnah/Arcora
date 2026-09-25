// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Arcora.Api.Services.Implementations
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<OrganizationMemberService> logger;
        private readonly IOrganizationMemberRepository organizationmemberRepository;
        private readonly ICohostInvitationRepository cohostInvitationRepository;
        private readonly IOrganizationRepository organizationRepository;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IConfiguration configuration;
        private readonly IEmailSender? emailSender;
        private readonly IPreferenceService preferenceService;
        private readonly IOptions<CacheConfiguration> _options;
        public OrganizationMemberService(
            IMapper mapper,
            IMemoryCache cache,
            IOptions<CacheConfiguration> options,
            ILogger<OrganizationMemberService> logger,
            IOrganizationMemberRepository organizationmemberRepository,
            ICohostInvitationRepository cohostInvitationRepository,
            IOrganizationRepository organizationRepository,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            IEmailSender? emailSender,
            IPreferenceService preferenceService)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.organizationmemberRepository = organizationmemberRepository;
            this.cohostInvitationRepository = cohostInvitationRepository;
            this.organizationRepository = organizationRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.emailSender = emailSender;
            this.preferenceService = preferenceService;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<OrganizationMemberDto>> GetAll(Paging paging)
        {
            IEnumerable<OrganizationMember> entities;
            try
            {
                entities = cache.Get<IEnumerable<OrganizationMember>>(Cache.ORGANIZATIONMEMBERS.ToString()) ?? new List<OrganizationMember>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.organizationmemberRepository.GetOrganizationMembersAsync())?.Where(x => x != null) ?? new List<OrganizationMember>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<OrganizationMember>>(Cache.ORGANIZATIONMEMBERS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }

            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrganizationMember by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<OrganizationMemberDto>
                {
                    Data = new List<OrganizationMemberDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<OrganizationMember> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.RoleName) && x.RoleName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.OrganizationMemberID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<OrganizationMemberDto>>(pagedEntities);
            return new PagedResult<OrganizationMemberDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<OrganizationMemberDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<OrganizationMember> entities = cache.Get<IEnumerable<OrganizationMember>>(Cache.ORGANIZATIONMEMBERS.ToString()) ?? new List<OrganizationMember>();
                OrganizationMember? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.OrganizationMemberID == ID);
                }
                else
                {
                    match = await this.organizationmemberRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<OrganizationMemberDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrganizationMember by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<List<OrganizationMemberDto>?> GetOrganizationMemberByOrgID(Guid organizationID)
        {
            try
            {
                List<OrganizationMember>? match = await this.organizationmemberRepository.GetOrganizationMembersByOrgIDAsync(organizationID);

                return match == null ? null : this.mapper.Map<List<OrganizationMemberDto>>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrganizationMember by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganizationMemberDto>?> GetMemberOrganizationsAsync(long userID)
        {
            try
            {
                IEnumerable<OrganizationMember> entities = await this.organizationmemberRepository.GetMemberOrganizationsAsync(userID);
                return entities == null ? null : this.mapper.Map<IEnumerable<OrganizationMemberDto>>(entities);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrganizationMember by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrganizationMemberDto> CreateOrganizationMember(OrganizationMemberDto organizationmemberDto)
        {
            OrganizationMember organizationMember = new OrganizationMember();
            IEnumerable<OrganizationMember?> checkEntity;
            try
            {
                checkEntity = await this.organizationmemberRepository.Find(x => x.RoleName!.ToLower().Trim() == organizationmemberDto.RoleName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    organizationMember = this.mapper.Map<OrganizationMember>(organizationmemberDto);
                    organizationMember.OrganizationMemberID = Guid.NewGuid();
                    organizationMember.CapturedDate = DateTime.UtcNow;
                    organizationMember = await organizationmemberRepository.Create(organizationMember) ?? new OrganizationMember();
                    await organizationmemberRepository.Save();
                    cache.Remove(Cache.ORGANIZATIONMEMBERS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating OrganizationMember. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<OrganizationMemberDto>(organizationMember);
        }

        /// <inheritdoc/>
        public async Task<OrganizationMemberDto?> UpdateOrganizationMember(Guid id, OrganizationMemberDto organizationmemberDto)
        {
            try
            {
                var existing = await this.organizationmemberRepository.GetByID(id);
                if (existing == null)
                    return null;
                OrganizationMember organizationMember = this.mapper.Map<OrganizationMember>(organizationmemberDto);
                organizationMember = await organizationmemberRepository.Update(organizationMember) ?? new OrganizationMember();
                await organizationmemberRepository.Save();
                cache.Remove(Cache.ORGANIZATIONMEMBERS.ToString());
                organizationmemberDto = this.mapper.Map<OrganizationMemberDto>(organizationMember);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating OrganizationMember. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return organizationmemberDto;
        }

        /// <inheritdoc/>
        public async Task DeleteOrganizationMember(Guid ID)
        {
            try
            {
                var organizationMember = await this.organizationmemberRepository.GetByID(ID);
                if (organizationMember == null)
                    throw new KeyNotFoundException("OrganizationMember with the specified ID was not found.");
                await organizationmemberRepository.Delete(organizationMember);
                await organizationmemberRepository.Save();
                cache.Remove(Cache.ORGANIZATIONMEMBERS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting OrganizationMember . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrganizationMemberDto?> UpdateOrganizationMemberStatus(Guid id, string status)
        {
            var organizationMember = await organizationmemberRepository.GetByID(id);
            if (organizationMember == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                organizationMember.Status = "Pending";
            }
            else
            {
                organizationMember.Status = status;
            }

            await organizationmemberRepository.Update(organizationMember);
            await organizationmemberRepository.Save();
            cache.Remove(Cache.ORGANIZATIONMEMBERS.ToString());
            return this.mapper.Map<OrganizationMemberDto>(organizationMember);
        }

        /// <inheritdoc/>
        public async Task<CohostInvitationResultDto> InviteCohostAsync(CohostInvitationDto cohostInvitationDto, long invitedByUserID)
        {
            if (cohostInvitationDto == null)
                throw new ArgumentException("Invitation request is required.");

            var email = cohostInvitationDto.Email?.Trim();
            var cohostName = cohostInvitationDto.CohostName?.Trim();
            var phoneNumber = cohostInvitationDto.PhoneNumber?.Trim();
            var normalizedAccess = NormalizeCohostAccess(cohostInvitationDto.CohostAccess);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(cohostName) || string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Email, cohost name, phone number and cohost access are required.");

            if (invitedByUserID <= 0)
                throw new ArgumentException("A valid inviter user is required.");

            var inviter = await userManager.FindByIdAsync(invitedByUserID.ToString());
            var inviterName = BuildUserDisplayName(inviter);

            var organization = await organizationRepository.GetByID(cohostInvitationDto.OrganizationID);
            if (organization == null)
                throw new KeyNotFoundException("Organization not found.");

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                var existingMemberships = await organizationmemberRepository.Find(x =>
                    x.OrganizationID == cohostInvitationDto.OrganizationID &&
                    x.UserID == existingUser.Id);

                var hasActiveOrPendingMembershipInTargetOrganization =
                    existingMemberships?
                        .Where(x => x != null)
                        .Any(x => IsActiveOrPendingMembershipStatus(x!.Status)) == true;

                if (hasActiveOrPendingMembershipInTargetOrganization)
                    throw new ArgumentException("This user already has an active or pending membership in the selected organization.");
            }

            var expiresAtUtc = DateTime.UtcNow.AddDays(CohostInviteTokenValidDays);
            var token = GenerateCohostInviteToken(cohostInvitationDto.OrganizationID, email, phoneNumber, normalizedAccess, expiresAtUtc);
            var tokenHash = HashToken(token);

            var existingPendingInvitations = await cohostInvitationRepository.Find(x =>
                x.OrganizationID == cohostInvitationDto.OrganizationID &&
                x.Email!.ToLower() == email.ToLower() &&
                x.Status != null &&
                (x.Status.ToUpper() == "PENDING" || x.Status.ToUpper() == "INVITED") &&
                x.ExpiresAt > DateTime.UtcNow);

            var pendingInvite = existingPendingInvitations?.FirstOrDefault(x => x != null);
            CohostInvitation persistedInvite;

            if (pendingInvite != null)
            {
                pendingInvite.CohostName = cohostName;
                pendingInvite.PhoneNumber = phoneNumber;
                pendingInvite.CohostAccess = normalizedAccess;
                pendingInvite.TokenHash = tokenHash;
                pendingInvite.ExpiresAt = expiresAtUtc;
                pendingInvite.Status = "PENDING";
                pendingInvite.UpdatedDate = DateTime.UtcNow;
                persistedInvite = await cohostInvitationRepository.Update(pendingInvite) ?? pendingInvite;
            }
            else
            {
                var invite = new CohostInvitation
                {
                    CohostInvitationID = Guid.NewGuid(),
                    OrganizationID = cohostInvitationDto.OrganizationID,
                    Email = email,
                    CohostName = cohostName,
                    PhoneNumber = phoneNumber,
                    CohostAccess = normalizedAccess,
                    TokenHash = tokenHash,
                    Status = "PENDING",
                    ExpiresAt = expiresAtUtc,
                    CapturedDate = DateTime.UtcNow,
                    CapturedBy = "SYSTEM"
                };

                persistedInvite = await cohostInvitationRepository.Create(invite) ?? invite;
            }

            await cohostInvitationRepository.Save();

            var frontendUrl = (configuration["FrontendUrl"] ?? string.Empty).TrimEnd('/');
            var inviteUrl = string.IsNullOrWhiteSpace(frontendUrl)
                ? $"cohost-invite/{token}"
                : $"{frontendUrl}/cohost-invite/{token}";
            var acceptUrl = $"{inviteUrl}?response=ACCEPT";
            var declineUrl = $"{inviteUrl}?response=DECLINE";

            await SendCohostInvitationEmailAsync(
                organizationName: organization.DisplayName ?? organization.LegalName ?? "Organization",
                cohostName: cohostName,
                inviterName: inviterName,
                cohostAccess: normalizedAccess,
                email: email,
                phoneNumber: phoneNumber,
                expiresAtUtc: expiresAtUtc,
                acceptUrl: acceptUrl,
                declineUrl: declineUrl);

            return new CohostInvitationResultDto
            {
                CohostInvitationID = persistedInvite.CohostInvitationID,
                Email = email,
                CohostName = cohostName,
                PhoneNumber = phoneNumber,
                CohostAccess = normalizedAccess,
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                Status = persistedInvite.Status
            };
        }

        /// <inheritdoc/>
        public async Task<List<CohostInvitationRecordDto>> GetCohostInvitationsByOrganizationAsync(Guid organizationID)
        {
            var invites = await cohostInvitationRepository.GetByOrganizationIDAsync(organizationID);
            invites = invites
                .Where(x => string.Equals(x.Status, "PENDING", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(x.Status, "INVITED", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return invites.Select(MapCohostInvitation).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<CohostInvitationRecordDto>> GetAllCohostInvitationsByOrganizationAsync(Guid organizationID)
        {
            var invites = await cohostInvitationRepository.GetByOrganizationIDAsync(organizationID);
            return invites.Select(MapCohostInvitation).ToList();
        }

        /// <inheritdoc/>
        public async Task<CohostInvitationRecordDto?> GetCohostInviteDetailsAsync(string token)
        {
            var invite = await GetInvitationByTokenAsync(token, includeExpired: false);
            return invite == null ? null : MapCohostInvitation(invite);
        }

        /// <inheritdoc/>
        public async Task<CohostInvitationRecordDto?> RespondToCohostInvitationAsync(string token, string response, long userID)
        {
            var invite = await GetInvitationByTokenAsync(token, includeExpired: true);
            if (invite == null)
                return null;

            if (invite.ExpiresAt <= DateTime.UtcNow)
            {
                if (!string.Equals(invite.Status, "EXPIRED", StringComparison.OrdinalIgnoreCase))
                {
                    invite.Status = "EXPIRED";
                    invite.UpdatedDate = DateTime.UtcNow;
                    await cohostInvitationRepository.Update(invite);
                    await cohostInvitationRepository.Save();
                }

                return null;
            }

            var normalizedResponse = response.Trim().ToUpperInvariant();
            switch (normalizedResponse)
            {
                case "DECLINE":
                case "DECLINED":
                    invite.Status = "DECLINED";
                    invite.DeclinedAt = DateTime.UtcNow;
                    invite.UpdatedDate = DateTime.UtcNow;
                    await cohostInvitationRepository.Update(invite);
                    await cohostInvitationRepository.Save();
                    return MapCohostInvitation(invite);

                case "ACCEPT":
                case "ACCEPTED":
                    return await AcceptCohostInvitationAsync(invite, userID);

                default:
                    throw new ArgumentException("Invalid response. Allowed values are ACCEPT or DECLINE.");
            }
        }

        /// <inheritdoc/>
        public async Task<CohostInvitationRecordDto?> RevokeCohostInvitationAsync(Guid cohostInvitationID, long updatedByUserID)
        {
            var invite = await cohostInvitationRepository.GetByID(cohostInvitationID);
            if (invite == null)
                return null;

            var normalizedStatus = invite.Status?.Trim().ToUpperInvariant();
            if (normalizedStatus == "REVOKED")
                return MapCohostInvitation(invite);

            if (normalizedStatus is not ("PENDING" or "INVITED" or "ACCEPTED"))
                throw new ArgumentException("Only pending invitations can be revoked.");

            invite.Status = "REVOKED";
            invite.RevokedAt = DateTime.UtcNow;
            invite.UpdatedDate = DateTime.UtcNow;
            invite.UpdatedBy = updatedByUserID.ToString();

            await cohostInvitationRepository.Update(invite);
            await cohostInvitationRepository.Save();

            var invitedUser = await userManager.FindByEmailAsync(invite.Email ?? string.Empty);
            if (invitedUser != null)
            {
                var memberships = await organizationmemberRepository.Find(x =>
                    x.OrganizationID == invite.OrganizationID &&
                    x.UserID == invitedUser.Id);

                var membership = memberships?
                    .Where(x => x != null)
                    .OrderByDescending(x => x!.CapturedDate)
                    .FirstOrDefault();

                if (membership != null)
                {
                    membership.Status = "REVOKED";
                    membership.DeactivatedAt = DateTime.UtcNow;
                    membership.UpdatedDate = DateTime.UtcNow;
                    membership.UpdatedBy = updatedByUserID.ToString();
                    await organizationmemberRepository.Update(membership);
                    await organizationmemberRepository.Save();
                    cache.Remove(Cache.ORGANIZATIONMEMBERS.ToString());
                }
            }

            return MapCohostInvitation(invite);
        }

        /// <inheritdoc/>
        public async Task<CohostInvitationRecordDto?> ReactivateRevokedCohostAsync(Guid cohostInvitationID, long updatedByUserID)
        {
            var invite = await cohostInvitationRepository.GetByID(cohostInvitationID);
            if (invite == null)
                return null;

            var normalizedStatus = invite.Status?.Trim().ToUpperInvariant();
            if (normalizedStatus is not "REVOKED")
                throw new ArgumentException("Only revoked invitations can be reactivated.");

            invite.RevokedAt = null;
            invite.UpdatedDate = DateTime.UtcNow;
            invite.UpdatedBy = updatedByUserID.ToString();
            if (invite.ExpiresAt <= DateTime.UtcNow)
            {
                invite.ExpiresAt = DateTime.UtcNow.AddDays(CohostInviteTokenValidDays);
                invite.Status = "ACCEPTED";
            } 
            else
                invite.Status = "PENDING";

            await cohostInvitationRepository.Update(invite);
            await cohostInvitationRepository.Save();

            var invitedUser = await userManager.FindByEmailAsync(invite.Email ?? string.Empty);
            if (invitedUser != null)
            {
                var memberships = await organizationmemberRepository.Find(x =>
                    x.OrganizationID == invite.OrganizationID &&
                    x.UserID == invitedUser.Id);

                var membership = memberships?
                    .Where(x => x != null)
                    .OrderByDescending(x => x!.CapturedDate)
                    .FirstOrDefault();

                if (membership != null)
                {
                    membership.Status = "ACTIVE";
                    membership.DeactivatedAt = null;
                    membership.UpdatedDate = DateTime.UtcNow;
                    membership.UpdatedBy = updatedByUserID.ToString();
                    await organizationmemberRepository.Update(membership);
                    await organizationmemberRepository.Save();
                    cache.Remove(Cache.ORGANIZATIONMEMBERS.ToString());
                }
            }

            return MapCohostInvitation(invite);
        }

        private static string NormalizeCohostAccess(string? access)
        {
            var normalized = access?.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("Cohost access is required. Allowed values are: Full access, Calendar and message access, Calendar access.");

            if (normalized.Equals("fullaccess", StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("full access", StringComparison.OrdinalIgnoreCase))
            {
                return "Full access";
            }

            if (normalized.Equals("calendar and message access", StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("calender and message access", StringComparison.OrdinalIgnoreCase))
            {
                return "Calendar and message access";
            }

            if (normalized.Equals("calendar access", StringComparison.OrdinalIgnoreCase))
            {
                return "Calendar access";
            }

            throw new ArgumentException("Invalid cohost access. Allowed values are: Full access, Calendar and message access, Calendar access.");
        }

        private async Task<CohostInvitationRecordDto> AcceptCohostInvitationAsync(CohostInvitation invite, long userID)
        {
            var user = await userManager.FindByIdAsync(userID.ToString());
            if (user == null)
                throw new KeyNotFoundException("Authenticated user was not found.");

            if (!string.Equals(user.Email?.Trim(), invite.Email?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("This invitation is tied to a different email address.");

            if (!await userManager.IsInRoleAsync(user, "LandLord"))
            {
                if (!await roleManager.RoleExistsAsync("LandLord"))
                {
                    var createRoleResult = await roleManager.CreateAsync(new Role { Name = "LandLord", Enabled = true });
                    if (!createRoleResult.Succeeded)
                        throw new ArgumentException("Failed to create required LandLord role.");
                }

                var addRoleResult = await userManager.AddToRoleAsync(user, "LandLord");
                if (!addRoleResult.Succeeded)
                    throw new ArgumentException("Failed to assign LandLord role to the cohost user.");
            }

            var memberships = await organizationmemberRepository.Find(x => x.OrganizationID == invite.OrganizationID && x.UserID == userID);
            var membership = memberships?.FirstOrDefault(x => x != null);

            if (membership == null)
            {
                membership = new OrganizationMember
                {
                    OrganizationMemberID = Guid.NewGuid(),
                    OrganizationID = invite.OrganizationID,
                    UserID = userID,
                    RoleName = invite.CohostAccess,
                    Status = "ACTIVE",
                    IsPrimaryOwner = false,
                    InvitedAt = invite.CapturedDate ?? DateTime.UtcNow,
                    AcceptedAt = DateTime.UtcNow,
                    CapturedDate = DateTime.UtcNow,
                    CapturedBy = "SYSTEM"
                };

                await organizationmemberRepository.Create(membership);
            }
            else
            {
                membership.RoleName = invite.CohostAccess;
                membership.Status = "ACTIVE";
                membership.AcceptedAt = DateTime.UtcNow;
                membership.UpdatedDate = DateTime.UtcNow;
                await organizationmemberRepository.Update(membership);
            }

            await organizationmemberRepository.Save();
            cache.Remove(Cache.ORGANIZATIONMEMBERS.ToString());

            invite.Status = "ACCEPTED";
            invite.AcceptedAt = DateTime.UtcNow;
            invite.UpdatedDate = DateTime.UtcNow;
            await cohostInvitationRepository.Update(invite);
            await cohostInvitationRepository.Save();

            return MapCohostInvitation(invite);
        }

        private async Task<CohostInvitation?> GetInvitationByTokenAsync(string token, bool includeExpired)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHash = HashToken(token.Trim());
            var invitations = await cohostInvitationRepository.Find(x => x.TokenHash == tokenHash);
            var invite = invitations?
                .Where(x => x != null)
                .OrderByDescending(x => x!.CapturedDate)
                .FirstOrDefault();

            if (invite == null)
                return null;

            var status = invite.Status?.Trim().ToUpperInvariant();
            var validStatuses = new[] { "PENDING", "INVITED" };
            if (!validStatuses.Contains(status))
                return null;

            if (!includeExpired && invite.ExpiresAt <= DateTime.UtcNow)
                return null;

            return invite;
        }

        private static CohostInvitationRecordDto MapCohostInvitation(CohostInvitation x)
        {
            return new CohostInvitationRecordDto
            {
                CohostInvitationID = x.CohostInvitationID,
                OrganizationID = x.OrganizationID,
                Email = x.Email,
                CohostName = x.CohostName,
                PhoneNumber = x.PhoneNumber,
                CohostAccess = x.CohostAccess,
                Status = x.Status,
                ExpiresAtUtc = x.ExpiresAt,
                AcceptedAtUtc = x.AcceptedAt,
                DeclinedAtUtc = x.DeclinedAt,
                CapturedDateUtc = x.CapturedDate,
                UpdatedDateUtc = x.UpdatedDate
            };
        }

        private static bool IsActiveOrPendingMembershipStatus(string? status)
        {
            var normalized = status?.Trim().ToUpperInvariant();
            return normalized is "INVITED" or "PENDING" or "ACTIVE" or "ACCEPTED";
        }

        private async Task SendCohostInvitationEmailAsync(
            string organizationName,
            string cohostName,
            string inviterName,
            string cohostAccess,
            string email,
            string phoneNumber,
            DateTime expiresAtUtc,
            string acceptUrl,
            string declineUrl)
        {
            if (emailSender is null)
                return;

            var (companyName, companyEmail) = await GetCompanyInfoAsync();
            var brand = string.IsNullOrWhiteSpace(companyName) ? "Arcora" : companyName;
            var htmlBody = EmailTemplates.BuildCohostInvitationEmail(
                organizationName: organizationName,
                cohostName: cohostName,
                inviterName: inviterName,
                cohostAccess: cohostAccess,
                email: email,
                phoneNumber: phoneNumber,
                expiresAt: expiresAtUtc.ToString("dddd, dd MMM yyyy 'at' HH:mm 'UTC'"),
                acceptUrl: acceptUrl,
                declineUrl: declineUrl,
                companyName: brand,
                supportEmail: companyEmail);

            await emailSender.SendEmailAsync(email, $"You are invited to cohost on {brand}", htmlBody);
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
            {
            }

            return ("", "");
        }

        private const int CohostInviteTokenValidDays = 5;

        private string GenerateCohostInviteToken(Guid organizationId, string email, string phoneNumber, string access, DateTime expiresAtUtc)
        {
            var expiryUnix = new DateTimeOffset(expiresAtUtc).ToUnixTimeSeconds();
            var payload = $"{organizationId:N}.{email.ToLowerInvariant()}.{phoneNumber}.{access}.{expiryUnix}";
            var signature = SignPayload(payload);
            return ToBase64Url($"{payload}.{signature}");
        }

        private string SignPayload(string payload)
        {
            var key = configuration["JwtSettings:Key"] ?? "arcora-default-signing-key";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"cohost-invite:{payload}"));
            return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        private static string ToBase64Url(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        private static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hash);
        }

        private static string BuildUserDisplayName(User? user)
        {
            if (user == null)
                return "A host";

            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            if (!string.IsNullOrWhiteSpace(fullName))
                return fullName;

            if (!string.IsNullOrWhiteSpace(user.DisplayName))
                return user.DisplayName;

            return user.Email ?? "A host";
        }
    }
}