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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<OrganizationService> logger;
        private readonly IOrganizationRepository organizationRepository;
        private readonly IOrganizationMemberRepository organizationMemberRepository;
        private readonly UserManager<User> userManager;
        private readonly IOptions<CacheConfiguration> _options;
        public OrganizationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<OrganizationService> logger, IOrganizationRepository organizationRepository, IOrganizationMemberRepository organizationMemberRepository, UserManager<User> userManager)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.organizationRepository = organizationRepository;
            this.organizationMemberRepository = organizationMemberRepository;
            this.userManager = userManager;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<OrganizationDto>> GetAll(Paging paging)
        {
            IEnumerable<Organization> entities;
            try
            {
                entities = cache.Get<IEnumerable<Organization>>(Cache.ORGANIZATIONS.ToString()) ?? new List<Organization>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.organizationRepository.GetAll())?.Where(x => x != null).Cast<Organization>().ToList() ?? new List<Organization>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Organization>>(Cache.ORGANIZATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Organization by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<OrganizationDto>
                {
                    Data = new List<OrganizationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Organization> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.DisplayName) && x.DisplayName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.OrganizationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<OrganizationDto>>(pagedEntities);
            return new PagedResult<OrganizationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<OrganizationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Organization> entities = cache.Get<IEnumerable<Organization>>(Cache.ORGANIZATIONS.ToString()) ?? new List<Organization>();
                Organization? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.OrganizationID == ID);
                }
                else
                {
                    match = await this.organizationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<OrganizationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Organization by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrganizationDto> CreateOrganization(OrganizationDto organizationDto, long creatorUserID = 0)
        {
            Organization organization = new Organization();
            IEnumerable<Organization?> checkEntity;
            try
            {
                checkEntity = await this.organizationRepository.Find(x => x.DisplayName!.ToLower().Trim() == organizationDto.DisplayName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    organization = this.mapper.Map<Organization>(organizationDto);
                    organization.OrganizationID = Guid.NewGuid();
                    organization.CapturedDate = DateTime.UtcNow;
                    organization = await organizationRepository.Create(organization) ?? new Organization();
                    await organizationRepository.Save();
                    cache.Remove(Cache.ORGANIZATIONS.ToString());

                    // Automatically enrol the creator as the primary owner member of the new organization.
                    await AddCreatorAsOwnerAsync(organization, creatorUserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Organization. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<OrganizationDto>(organization);
        }

        /// <summary>
        /// Adds the creating user to the newly created organization as the active primary owner. Failures are
        /// logged but do not roll back organization creation.
        /// </summary>
        private async Task AddCreatorAsOwnerAsync(Organization organization, long creatorUserID)
        {
            if (creatorUserID <= 0 || organization.OrganizationID == Guid.Empty)
                return;

            try
            {
                var alreadyMember = await organizationMemberRepository.Any(m =>
                    m.OrganizationID == organization.OrganizationID && m.UserID == creatorUserID);
                if (alreadyMember)
                    return;

                var creator = await userManager.FindByIdAsync(creatorUserID.ToString());
                var creatorName = creator != null
                    ? $"{creator.FirstName} {creator.LastName}".Trim()
                    : creatorUserID.ToString();
                if (string.IsNullOrWhiteSpace(creatorName))
                    creatorName = creatorUserID.ToString();

                var now = DateTime.UtcNow;
                var member = new OrganizationMember
                {
                    OrganizationMemberID = Guid.NewGuid(),
                    OrganizationID = organization.OrganizationID,
                    UserID = creatorUserID,
                    RoleName = "OWNER",
                    Status = "ACTIVE",
                    IsPrimaryOwner = true,
                    InvitedAt = now,
                    AcceptedAt = now,
                    CapturedDate = now,
                    CapturedBy = creatorName
                };

                await organizationMemberRepository.Create(member);
                await organizationMemberRepository.Save();
                cache.Remove(Cache.ORGANIZATIONMEMBERS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while adding the creator as owner for Organization {OrganizationId}. Timestamp: {Timestamp}", organization.OrganizationID, DateTime.UtcNow);
            }
        }

        /// <inheritdoc/>
        public async Task<OrganizationDto?> UpdateOrganization(Guid id, OrganizationDto organizationDto)
        {
            try
            {
                var existing = await this.organizationRepository.GetByID(id);
                if (existing == null)
                    return null;
                Organization organization = this.mapper.Map<Organization>(organizationDto);
                organization = await organizationRepository.Update(organization) ?? new Organization();
                await organizationRepository.Save();
                cache.Remove(Cache.ORGANIZATIONS.ToString());
                organizationDto = this.mapper.Map<OrganizationDto>(organization);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Organization. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return organizationDto;
        }

        /// <inheritdoc/>
        public async Task DeleteOrganization(Guid ID)
        {
            try
            {
                var organization = await this.organizationRepository.GetByID(ID);
                if (organization == null)
                    throw new KeyNotFoundException("Organization with the specified ID was not found.");
                await organizationRepository.Delete(organization);
                await organizationRepository.Save();
                cache.Remove(Cache.ORGANIZATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Organization . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrganizationDto?> UpdateOrganizationStatus(Guid id, string status)
        {
            var organization = await organizationRepository.GetByID(id);
            if (organization == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                organization.Status = "Pending";
            }
            else
            {
                organization.Status = status;
            }

            await organizationRepository.Update(organization);
            await organizationRepository.Save();
            cache.Remove(Cache.ORGANIZATIONS.ToString());
            return this.mapper.Map<OrganizationDto>(organization);
        }
    }
}