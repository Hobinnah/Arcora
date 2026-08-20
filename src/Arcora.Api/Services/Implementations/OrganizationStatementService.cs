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
    public class OrganizationStatementService : IOrganizationStatementService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<OrganizationStatementService> logger;
        private readonly IOrganizationStatementRepository organizationstatementRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public OrganizationStatementService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<OrganizationStatementService> logger, IOrganizationStatementRepository organizationstatementRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.organizationstatementRepository = organizationstatementRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<OrganizationStatementDto>> GetAll(Paging paging)
        {
            IEnumerable<OrganizationStatement> entities;
            try
            {
                entities = cache.Get<IEnumerable<OrganizationStatement>>(Cache.ORGANIZATIONSTATEMENTS.ToString()) ?? new List<OrganizationStatement>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.organizationstatementRepository.GetOrganizationStatementAsync())?.Where(x => x != null) ?? new List<OrganizationStatement>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<OrganizationStatement>>(Cache.ORGANIZATIONSTATEMENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrganizationStatement by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<OrganizationStatementDto>
                {
                    Data = new List<OrganizationStatementDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<OrganizationStatement> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.CapturedBy) && x.CapturedBy.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.OrganizationStatementID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<OrganizationStatementDto>>(pagedEntities);
            return new PagedResult<OrganizationStatementDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<OrganizationStatementDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<OrganizationStatement> entities = cache.Get<IEnumerable<OrganizationStatement>>(Cache.ORGANIZATIONSTATEMENTS.ToString()) ?? new List<OrganizationStatement>();
                OrganizationStatement? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.OrganizationStatementID == ID);
                }
                else
                {
                    match = await this.organizationstatementRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<OrganizationStatementDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrganizationStatement by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrganizationStatementDto> CreateOrganizationStatement(OrganizationStatementDto organizationstatementDto)
        {
            OrganizationStatement organizationStatement = new OrganizationStatement();
            IEnumerable<OrganizationStatement?> checkEntity;
            try
            {
                checkEntity = await this.organizationstatementRepository.Find(x => x.CapturedBy!.ToLower().Trim() == organizationstatementDto.CapturedBy!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    organizationStatement = this.mapper.Map<OrganizationStatement>(organizationstatementDto);
                    organizationStatement.OrganizationStatementID = Guid.NewGuid();
                    organizationStatement.CapturedDate = DateTime.UtcNow;
                    organizationStatement = await organizationstatementRepository.Create(organizationStatement) ?? new OrganizationStatement();
                    await organizationstatementRepository.Save();
                    cache.Remove(Cache.ORGANIZATIONSTATEMENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating OrganizationStatement. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<OrganizationStatementDto>(organizationStatement);
        }

        /// <inheritdoc/>
        public async Task<OrganizationStatementDto?> UpdateOrganizationStatement(Guid id, OrganizationStatementDto organizationstatementDto)
        {
            try
            {
                var existing = await this.organizationstatementRepository.GetByID(id);
                if (existing == null)
                    return null;
                OrganizationStatement organizationStatement = this.mapper.Map<OrganizationStatement>(organizationstatementDto);
                organizationStatement = await organizationstatementRepository.Update(organizationStatement) ?? new OrganizationStatement();
                await organizationstatementRepository.Save();
                cache.Remove(Cache.ORGANIZATIONSTATEMENTS.ToString());
                organizationstatementDto = this.mapper.Map<OrganizationStatementDto>(organizationStatement);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating OrganizationStatement. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return organizationstatementDto;
        }

        /// <inheritdoc/>
        public async Task DeleteOrganizationStatement(Guid ID)
        {
            try
            {
                var organizationStatement = await this.organizationstatementRepository.GetByID(ID);
                if (organizationStatement == null)
                    throw new KeyNotFoundException("OrganizationStatement with the specified ID was not found.");
                await organizationstatementRepository.Delete(organizationStatement);
                await organizationstatementRepository.Save();
                cache.Remove(Cache.ORGANIZATIONSTATEMENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting OrganizationStatement . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}