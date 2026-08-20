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
    public class CreditReportingConsentAuditService : ICreditReportingConsentAuditService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<CreditReportingConsentAuditService> logger;
        private readonly ICreditReportingConsentAuditRepository creditreportingconsentauditRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public CreditReportingConsentAuditService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<CreditReportingConsentAuditService> logger, ICreditReportingConsentAuditRepository creditreportingconsentauditRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.creditreportingconsentauditRepository = creditreportingconsentauditRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<CreditReportingConsentAuditDto>> GetAll(Paging paging)
        {
            IEnumerable<CreditReportingConsentAudit> entities;
            try
            {
                entities = cache.Get<IEnumerable<CreditReportingConsentAudit>>(Cache.CREDITREPORTINGCONSENTAUDITS.ToString()) ?? new List<CreditReportingConsentAudit>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.creditreportingconsentauditRepository.GetCreditReportingConsentAuditAsync())?.Where(x => x != null) ?? new List<CreditReportingConsentAudit>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<CreditReportingConsentAudit>>(Cache.CREDITREPORTINGCONSENTAUDITS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching CreditReportingConsentAudit by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<CreditReportingConsentAuditDto>
                {
                    Data = new List<CreditReportingConsentAuditDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<CreditReportingConsentAudit> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Action) && x.Action.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ConsentAuditID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<CreditReportingConsentAuditDto>>(pagedEntities);
            return new PagedResult<CreditReportingConsentAuditDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<CreditReportingConsentAuditDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<CreditReportingConsentAudit> entities = cache.Get<IEnumerable<CreditReportingConsentAudit>>(Cache.CREDITREPORTINGCONSENTAUDITS.ToString()) ?? new List<CreditReportingConsentAudit>();
                CreditReportingConsentAudit? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ConsentAuditID == ID);
                }
                else
                {
                    match = await this.creditreportingconsentauditRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<CreditReportingConsentAuditDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching CreditReportingConsentAudit by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<CreditReportingConsentAuditDto> CreateCreditReportingConsentAudit(CreditReportingConsentAuditDto creditreportingconsentauditDto)
        {
            CreditReportingConsentAudit creditReportingConsentAudit = new CreditReportingConsentAudit();
            IEnumerable<CreditReportingConsentAudit?> checkEntity;
            try
            {
                checkEntity = await this.creditreportingconsentauditRepository.Find(x => x.Action!.ToLower().Trim() == creditreportingconsentauditDto.Action!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    creditReportingConsentAudit = this.mapper.Map<CreditReportingConsentAudit>(creditreportingconsentauditDto);
                    creditReportingConsentAudit.ConsentAuditID = Guid.NewGuid();
                    creditReportingConsentAudit.ProviderReferenceID = string.IsNullOrEmpty(creditreportingconsentauditDto.ProviderReferenceID) ? null : creditreportingconsentauditDto.ProviderReferenceID;
                    creditReportingConsentAudit.CapturedDate = DateTime.UtcNow;
                    creditReportingConsentAudit = await creditreportingconsentauditRepository.Create(creditReportingConsentAudit) ?? new CreditReportingConsentAudit();
                    await creditreportingconsentauditRepository.Save();
                    cache.Remove(Cache.CREDITREPORTINGCONSENTAUDITS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating CreditReportingConsentAudit. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<CreditReportingConsentAuditDto>(creditReportingConsentAudit);
        }

        /// <inheritdoc/>
        public async Task<CreditReportingConsentAuditDto?> UpdateCreditReportingConsentAudit(Guid id, CreditReportingConsentAuditDto creditreportingconsentauditDto)
        {
            try
            {
                var existing = await this.creditreportingconsentauditRepository.GetByID(id);
                if (existing == null)
                    return null;
                CreditReportingConsentAudit creditReportingConsentAudit = this.mapper.Map<CreditReportingConsentAudit>(creditreportingconsentauditDto);
                creditReportingConsentAudit = await creditreportingconsentauditRepository.Update(creditReportingConsentAudit) ?? new CreditReportingConsentAudit();
                await creditreportingconsentauditRepository.Save();
                cache.Remove(Cache.CREDITREPORTINGCONSENTAUDITS.ToString());
                creditreportingconsentauditDto = this.mapper.Map<CreditReportingConsentAuditDto>(creditReportingConsentAudit);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating CreditReportingConsentAudit. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return creditreportingconsentauditDto;
        }

        /// <inheritdoc/>
        public async Task DeleteCreditReportingConsentAudit(Guid ID)
        {
            try
            {
                var creditReportingConsentAudit = await this.creditreportingconsentauditRepository.GetByID(ID);
                if (creditReportingConsentAudit == null)
                    throw new KeyNotFoundException("CreditReportingConsentAudit with the specified ID was not found.");
                await creditreportingconsentauditRepository.Delete(creditReportingConsentAudit);
                await creditreportingconsentauditRepository.Save();
                cache.Remove(Cache.CREDITREPORTINGCONSENTAUDITS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting CreditReportingConsentAudit . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}