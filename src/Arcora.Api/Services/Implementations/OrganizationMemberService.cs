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

namespace Arcora.Api.Services.Implementations
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<OrganizationMemberService> logger;
        private readonly IOrganizationMemberRepository organizationmemberRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public OrganizationMemberService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<OrganizationMemberService> logger, IOrganizationMemberRepository organizationmemberRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.organizationmemberRepository = organizationmemberRepository;
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
                    entities = (await this.organizationmemberRepository.GetOrganizationMemberAsync())?.Where(x => x != null) ?? new List<OrganizationMember>();
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
    }
}